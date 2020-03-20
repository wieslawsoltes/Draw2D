using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class SelectionTool : BaseTool, ITool, ISelection
    {
        private SelectionToolSettings _settings;
        private RectangleShape _rectangle;
        private double _originX;
        private double _originY;
        private double _previousX;
        private double _previousY;
        private IList<IBaseShape> _shapesToCopy = null;
        private bool _disconnected = false;
        private bool _copied = false;

        public enum State
        {
            None,
            Selection,
            Move
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.None;

        [IgnoreDataMember]
        public new string Title => "Selection";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private bool IsAcceptedShape(IBaseShape shape)
        {
            return !(shape is IPointShape || shape is FigureShape);
        }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _disconnected = false;
            _copied = false;

            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            FiltersClear(context);
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
            var selected = TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                new Point2(x, y),
                radius,
                scale,
                modifier);
            if (selected == true)
            {
                context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();

                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Settings?.SelectionModifier ?? Modifier.Control))
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        StartPoint = new PointShape(),
                        Point = new PointShape()
                    };
                    _rectangle.StartPoint.Owner = _rectangle;
                    _rectangle.Point.Owner = _rectangle;
                }

                _rectangle.StartPoint.X = x;
                _rectangle.StartPoint.Y = y;
                _rectangle.Point.X = x;
                _rectangle.Point.Y = y;
                _rectangle.StyleId = Settings?.SelectionStyle;
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

                context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.Point.X = x;
            _rectangle.Point.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                _rectangle.ToRect2(),
                radius,
                scale,
                modifier);

            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            _rectangle = null;

            CurrentState = State.None;

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null && !(context.DocumentContainer.ContainerView.SelectionState?.Hovered == null && context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count > 0))
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var previous = context.DocumentContainer?.ContainerView?.SelectionState?.Hovered;
                    var radius = Settings?.HitTestRadius ?? 7.0;
                    var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
                    var target = new Point2(x, y);
                    var shape = TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        radius,
                        scale,
                        modifier);
                    if (shape != null)
                    {
                        if (shape != previous)
                        {
                            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                            context.DocumentContainer?.ContainerView?.SelectionState?.Hover(shape);
                            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.Point.X = x;
            _rectangle.Point.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            double dx = x - _previousX;
            double dy = y - _previousY;

            _previousX = x;
            _previousY = y;

            if (modifier == Modifier.None)
            {
                _disconnected = false;
                _copied = false;
            }

            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                if (context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count == 1)
                {
                    var shape = context.DocumentContainer.ContainerView.SelectionState?.Shapes.FirstOrDefault();

                    if (shape is IPointShape source)
                    {
                        if (Settings.ConnectPoints && modifier == Settings?.ConnectionModifier)
                        {
                            ConnectImpl(context, source, modifier);
                        }

                        if (Settings.DisconnectPoints && modifier == Settings?.ConnectionModifier)
                        {
                            if (_disconnected == false)
                            {
                                var radius = Settings?.DisconnectTestRadius ?? 10.0;
                                var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
                                double treshold = radius / scale;
                                double tx = Math.Abs(_originX - source.X);
                                double ty = Math.Abs(_originY - source.Y);
                                if (tx > treshold || ty > treshold)
                                {
                                    DisconnectImpl(context, source);
                                }
                            }
                        }

                        if (modifier == Settings?.CopyModifier && _copied == false)
                        {
                            Duplicate(context);
                            this.CurrentState = State.Move;
                            _copied = true;
                        }
                    }

                    shape.Move(context.DocumentContainer.ContainerView.SelectionState, dx, dy);
                }
                else
                {
                    if (Settings.DisconnectPoints && modifier == Settings?.ConnectionModifier && _disconnected == false)
                    {
                        var selectedToDisconnect = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                        foreach (var shape in selectedToDisconnect)
                        {
                            if (IsAcceptedShape(shape))
                            {
                                DisconnectImpl(context, shape);
                            }
                        }
                    }

                    if (modifier == Settings?.CopyModifier && _copied == false)
                    {
                        Duplicate(context);
                        this.CurrentState = State.Move;
                        _copied = true;
                    }

                    var selectedToMove = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                    foreach (var shape in selectedToMove)
                    {
                        shape.Move(context.DocumentContainer.ContainerView.SelectionState, dx, dy);
                    }
                }

                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;
            _copied = false;

            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

            if (_rectangle != null)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                context.DocumentContainer?.ContainerView?.SelectionState?.Clear();
            }

            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        LeftDownNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        LeftDownSelectionInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Selection:
                    {
                        LeftUpSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        LeftUpMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Selection:
                    {
                        this.Clean(context);
                    }
                    break;
                case State.Move:
                    {
                        RightDownMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        MoveNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        MoveSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        MoveMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }

        public void Cut(IToolContext context)
        {
            Copy(context);
            DeleteOnlyAccepted(context);
        }

        public void Copy(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    _shapesToCopy = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                }
            }
        }

        public void Paste(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                if (_shapesToCopy != null && _shapesToCopy.Count > 0)
                {
                    lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                    {
                        context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                        context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                        Copy(context.DocumentContainer?.ContainerView?.CurrentContainer, _shapesToCopy, context.DocumentContainer.ContainerView.SelectionState);

                        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        private void DeleteOnlyAccepted(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    Delete(context.DocumentContainer?.ContainerView?.CurrentContainer, context.DocumentContainer.ContainerView.SelectionState, true);

                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Delete(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    Delete(context.DocumentContainer?.ContainerView?.CurrentContainer, context.DocumentContainer.ContainerView.SelectionState, false);

                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Duplicate(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);

                    if (shapes != null && shapes.Count > 0)
                    {
                        context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                        context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                        Copy(context.DocumentContainer?.ContainerView?.CurrentContainer, shapes, context.DocumentContainer.ContainerView.SelectionState);

                        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void CreateGroup(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes.Reverse());
                    if (shapes.Count > 0)
                    {
                        var group = new GroupShape()
                        {
                            Title = "Group",
                            Points = new ObservableCollection<IPointShape>(),
                            Shapes = new ObservableCollection<IBaseShape>()
                        };

                        foreach (var shape in shapes)
                        {
                            if (IsAcceptedShape(shape))
                            {
                                shape.Owner = group;
                                group.Shapes.Add(shape);
                            }
                        }

                        if (group.Shapes.Count > 0)
                        {
                            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

                            DeleteOnlyAccepted(context);

                            group.Select(context.DocumentContainer.ContainerView.SelectionState);
                            group.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                            context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(group);
                            context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);

                            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                        }

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void CreateReference(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                    if (shapes.Count > 0)
                    {
                        context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

                        foreach (var shape in shapes)
                        {
                            if (IsAcceptedShape(shape) && !(shape is ReferenceShape))
                            {
                                context.DocumentContainer?.ContainerView?.Reference(shape);
                            }
                        }

                        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void CreatePath(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes.Reverse());
                    if (shapes.Count > 0)
                    {
                        var paths = new List<PathShape>();

                        foreach (var shape in shapes)
                        {
                            if (IsAcceptedShape(shape))
                            {
                                var path = context?.PathConverter?.ToPathShape(context, shape);
                                if (path != null)
                                {
                                    paths.Add(path);
                                }
                            }
                        }
                        if (paths.Count > 0)
                        {
                            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

                            DeleteOnlyAccepted(context);

                            foreach (var path in paths)
                            {
                                path.Select(context.DocumentContainer.ContainerView.SelectionState);
                                path.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(path);
                                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                            }

                            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                        }

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void CreateStrokePath(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes.Reverse());
                    if (shapes.Count > 0)
                    {
                        var paths = new List<PathShape>();

                        foreach (var shape in shapes)
                        {
                            if (IsAcceptedShape(shape))
                            {
                                var path = context?.PathConverter?.ToStrokePathShape(context, shape);
                                if (path != null)
                                {
                                    paths.Add(path);
                                }
                            }
                        }

                        if (paths.Count > 0)
                        {
                            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

                            DeleteOnlyAccepted(context);

                            foreach (var path in paths)
                            {
                                path.Select(context.DocumentContainer.ContainerView.SelectionState);
                                path.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(path);
                                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                            }

                            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                        }

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void CreateFillPath(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes.Reverse());
                    if (shapes.Count > 0)
                    {
                        var paths = new List<PathShape>();

                        foreach (var shape in shapes)
                        {
                            if (IsAcceptedShape(shape))
                            {
                                var path = context?.PathConverter?.ToFillPathShape(context, shape);
                                if (path != null)
                                {
                                    paths.Add(path);
                                }
                            }
                        }

                        if (paths.Count > 0)
                        {
                            context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();

                            DeleteOnlyAccepted(context);

                            foreach (var path in paths)
                            {
                                path.Select(context.DocumentContainer.ContainerView.SelectionState);
                                path.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(path);
                                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                            }

                            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                        }

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void StackHorizontally(IToolContext context)
        {
            Layout.Stack(context, StackMode.Horizontal);
        }

        public void StackVertically(IToolContext context)
        {
            Layout.Stack(context, StackMode.Vertical);
        }

        public void DistributeHorizontally(IToolContext context)
        {
            Layout.Distribute(context, DistributeMode.Horizontal);
        }

        public void DistributeVertically(IToolContext context)
        {
            Layout.Distribute(context, DistributeMode.Vertical);
        }

        public void AlignLeft(IToolContext context)
        {
            Layout.Align(context, AlignMode.Left);
        }

        public void AlignCentered(IToolContext context)
        {
            Layout.Align(context, AlignMode.Centered);
        }

        public void AlignRight(IToolContext context)
        {
            Layout.Align(context, AlignMode.Right);
        }

        public void AlignTop(IToolContext context)
        {
            Layout.Align(context, AlignMode.Top);
        }

        public void AlignCenter(IToolContext context)
        {
            Layout.Align(context, AlignMode.Center);
        }

        public void AlignBottom(IToolContext context)
        {
            Layout.Align(context, AlignMode.Bottom);
        }

        public void ArangeBringToFront(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);

                for (int i = shapes.Count - 1; i >= 0; i--)
                {
                    var shape = shapes[i];
                    if (IsAcceptedShape(shape))
                    {
                        Layout.BringToFront(context, shape);
                    }
                }

                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void ArangeBringForward(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);

                for (int i = shapes.Count - 1; i >= 0; i--)
                {
                    var shape = shapes[i];
                    if (IsAcceptedShape(shape))
                    {
                        Layout.BringForward(context, shape);
                    }
                }

                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void ArangeSendBackward(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);

                for (int i = 0; i < shapes.Count; i++)
                {
                    var shape = shapes[i];
                    if (IsAcceptedShape(shape))
                    {
                        Layout.SendBackward(context, shape);
                    }
                }

                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void ArangeSendToBack(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);

                for (int i = 0; i < shapes.Count; i++)
                {
                    var shape = shapes[i];
                    if (IsAcceptedShape(shape))
                    {
                        Layout.SendToBack(context, shape);
                    }
                }

                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        private void BreakGroup(IToolContext context, GroupShape group)
        {
            context.DocumentContainer?.ContainerView?.CurrentContainer?.Shapes?.Remove(group);
            context.DocumentContainer?.ContainerView?.CurrentContainer?.MarkAsDirty(true);

            foreach (var shape in group.Shapes)
            {
                if (IsAcceptedShape(shape))
                {
                    shape.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                    context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(shape);
                    context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                    shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                }
            }
        }

        private void BreakReference(IToolContext context, ReferenceShape reference)
        {
            context.DocumentContainer?.ContainerView?.CurrentContainer?.Shapes?.Remove(reference);
            context.DocumentContainer?.ContainerView?.CurrentContainer?.MarkAsDirty(true);

            if (reference.Template is IBaseShape shape)
            {
                var copy = (IBaseShape)shape.Copy(null);
                copy.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(copy);
                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                copy.Select(context.DocumentContainer.ContainerView.SelectionState);
            }
        }

        private void BreakFigure(IToolContext context, FigureShape figure)
        {
            foreach (var shape in figure.Shapes)
            {
                if (!(shape is IPointShape))
                {
                    shape.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                    context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(shape);
                    context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                    shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                }
            }
        }

        private void BreakPath(IToolContext context, PathShape path)
        {
            context.DocumentContainer?.ContainerView?.CurrentContainer?.Shapes?.Remove(path);
            context.DocumentContainer?.ContainerView?.CurrentContainer?.MarkAsDirty(true);

            if (path.Shapes.Count == 1)
            {
                if (path.Shapes[0] is FigureShape figure)
                {
                    BreakFigure(context, figure);
                }
            }
            else
            {
                foreach (var shape in path.Shapes)
                {
                    if (shape is FigureShape figure)
                    {
                        var pathShape = new PathShape()
                        {
                            Points = new ObservableCollection<IPointShape>(),
                            Shapes = new ObservableCollection<IBaseShape>(),
                            FillType = path.FillType,
                            Text = new Text(),
                            StyleId = path.StyleId
                        };

                        figure.Owner = pathShape;
                        pathShape.Shapes.Add(figure);

                        pathShape.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                        context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(pathShape);
                        context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                        pathShape.Select(context.DocumentContainer.ContainerView.SelectionState);
                    }
                }
            }
        }

        public void Break(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes.Reverse());

                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    foreach (var shape in shapes)
                    {
                        if (IsAcceptedShape(shape))
                        {
                            switch (shape)
                            {
                                case PathShape pathShape:
                                    {
                                        BreakPath(context, pathShape);
                                    }
                                    break;
                                case GroupShape groupShape:
                                    {
                                        BreakGroup(context, groupShape);
                                    }
                                    break;
                                case ReferenceShape referenceShape:
                                    {
                                        BreakReference(context, referenceShape);
                                    }
                                    break;
                                default:
                                    {
                                        var path = context?.PathConverter?.ToPathShape(context, shape);
                                        if (path != null)
                                        {
                                            context.DocumentContainer?.ContainerView?.CurrentContainer?.Shapes?.Remove(shape);
                                            context.DocumentContainer?.ContainerView?.CurrentContainer?.MarkAsDirty(true);

                                            BreakPath(context, path);
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void ConnectImpl(IToolContext context, IPointShape point, Modifier modifier)
        {
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
            var target = context.HitTest?.TryToGetPoint(
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                scale,
                modifier,
                point);
            if (target != point)
            {
                foreach (var item in context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes)
                {
                    if (item is IConnectable connectable)
                    {
                        if (connectable.Connect(point, target))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void DisconnectImpl(IToolContext context, IPointShape point)
        {
            foreach (var shape in context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes)
            {
                if (shape is IConnectable connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(point);
                            context.DocumentContainer?.ContainerView?.SelectionState?.Select(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void DisconnectImpl(IToolContext context, IBaseShape shape)
        {
            if (shape is IConnectable connectable)
            {
                if (context.DocumentContainer?.ContainerView?.SelectionState != null)
                {
                    shape.Deselect(context.DocumentContainer.ContainerView.SelectionState);
                }
                _disconnected = connectable.Disconnect();
                if (context.DocumentContainer?.ContainerView?.SelectionState != null)
                {
                    shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                }
            }
        }

        public void Connect(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                if (context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count == 1)
                {
                    var shape = context.DocumentContainer.ContainerView.SelectionState?.Shapes.FirstOrDefault();

                    if (shape is IPointShape source)
                    {
                        ConnectImpl(context, source, Modifier.None);
                        context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                        context.DocumentContainer?.ContainerView?.SelectionState?.Clear();
                        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                    }
                }
                else if (context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count == 2)
                {
                    var first = context.DocumentContainer.ContainerView.SelectionState?.Shapes.FirstOrDefault();
                    var next = context.DocumentContainer.ContainerView.SelectionState?.Shapes.LastOrDefault();

                    if (first is IPointShape point && next is IPointShape target)
                    {
                        if (target != point)
                        {
                            foreach (var item in context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes)
                            {
                                if (item is IConnectable connectable)
                                {
                                    if (connectable.Connect(point, target))
                                    {
                                        context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                                        context.DocumentContainer?.ContainerView?.SelectionState?.Clear();
                                        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Disconnect(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                if (context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count == 1)
                {
                    var shape = context.DocumentContainer.ContainerView.SelectionState?.Shapes.FirstOrDefault();

                    if (shape is IPointShape source)
                    {
                        DisconnectImpl(context, source);

                    }
                }
                else
                {
                    var selectedToDisconnect = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                    foreach (var shape in selectedToDisconnect)
                    {
                        if (!(shape is IPointShape))
                        {
                            DisconnectImpl(context, shape);
                        }
                    }
                }

                context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                context.DocumentContainer?.ContainerView?.SelectionState?.Clear();
                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void SelectAll(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    foreach (var shape in context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes)
                    {
                        shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                    }

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void DeselectAll(IToolContext context)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                lock (context.DocumentContainer.ContainerView.SelectionState?.Shapes)
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        internal Dictionary<object, object> GetPointsCopyDict(IEnumerable<IBaseShape> shapes)
        {
            var copy = new Dictionary<object, object>();

            var points = new List<IPointShape>();

            foreach (var shape in shapes)
            {
                shape.GetPoints(points);
            }

            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                copy[point] = point.Copy(null);
            }

            return copy;
        }

        internal void Copy(ICanvasContainer container, IList<IBaseShape> shapes, ISelectionState selectionState)
        {
            var shared = GetPointsCopyDict(shapes);
            var points = new List<IPointShape>();

            for (int i = shapes.Count - 1; i >= 0; i--)
            {
                var shape = shapes[i];
                if (shape is ICopyable copyable)
                {
                    var copy = (IBaseShape)(copyable.Copy(shared));
                    if (copy != null)
                    {
                        switch (copy)
                        {
                            case IPointShape pointShape:
                                {
                                    // TODO: Copy point shape.
                                }
                                break;
                            case FigureShape figureShape:
                                {
                                    // TODO: Copy figure shape.
                                }
                                break;
                            default:
                                {
                                    copy.GetPoints(points);
                                    copy.Select(selectionState);
                                    copy.Owner = container;
                                    container.Shapes.Add(copy);
                                }
                                break;
                        }
                    }
                }
            }

            foreach (var point in points)
            {
                if (point.Owner != null)
                {
                    if (shared.TryGetValue(point.Owner, out var owner))
                    {
                        point.Owner = owner;
                    }
                }
            }
        }

        internal void Delete(ICanvasContainer container, ISelectionState selectionState, bool onlyAccepted)
        {
            var shapesHash = new HashSet<IBaseShape>(container.Shapes);
            var selected = new List<IBaseShape>(selectionState.Shapes);

            foreach (var shape in selected)
            {
                if (onlyAccepted)
                {
                    if (!IsAcceptedShape(shape))
                    {
                        continue;
                    }
                }

                if (shapesHash.Contains(shape))
                {
                    shape.Deselect(selectionState);
                    container.Shapes.Remove(shape);
                    container.MarkAsDirty(true);
                }
                else
                {
                    switch (shape.Owner)
                    {
                        case CanvasContainer canvasContainer:
                            {
                                if (shape is IPointShape pointShape)
                                {
                                    shape.Deselect(selectionState);
                                    canvasContainer.Points.Remove(pointShape);
                                    canvasContainer.MarkAsDirty(true);
                                }
                                else
                                {
                                    shape.Deselect(selectionState);
                                    canvasContainer.Shapes.Remove(shape);
                                    canvasContainer.MarkAsDirty(true);
                                }
                            }
                            break;
                        case FigureShape figureShape:
                            {
                                if (shape is IPointShape pointShape)
                                {
                                    shape.Deselect(selectionState);
                                    figureShape.Points.Remove(pointShape);
                                    figureShape.MarkAsDirty(true);
                                }
                                else
                                {
                                    shape.Deselect(selectionState);
                                    figureShape.Shapes.Remove(shape);
                                    figureShape.MarkAsDirty(true);
                                }
                            }
                            break;
                        case PathShape pathShape:
                            {
                                if (shape is IPointShape pointShape)
                                {
                                    shape.Deselect(selectionState);
                                    pathShape.Points.Remove(pointShape);
                                    pathShape.MarkAsDirty(true);
                                }
                                else
                                {
                                    shape.Deselect(selectionState);
                                    pathShape.Shapes.Remove(shape);
                                    pathShape.MarkAsDirty(true);
                                }
                            }
                            break;
                        case GroupShape groupShape:
                            {
                                if (shape is IPointShape pointShape)
                                {
                                    shape.Deselect(selectionState);
                                    groupShape.Points.Remove(pointShape);
                                    groupShape.MarkAsDirty(true);
                                }
                                else
                                {
                                    shape.Deselect(selectionState);
                                    groupShape.Shapes.Remove(shape);
                                    groupShape.MarkAsDirty(true);
                                }
                            }
                            break;
                        case IConnectable connectable:
                            {
                                if (shape is IPointShape pointShape)
                                {
                                    shape.Deselect(selectionState);
                                    connectable.Points.Remove(pointShape);
                                    if (shape.Owner is IDirty dirty)
                                    {
                                        dirty.MarkAsDirty(true);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        internal IBaseShape TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius, double scale, Modifier modifier)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes, target, radius, scale, modifier, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes, target, radius, scale, modifier) : null;

            if (shapePoint != null || shape != null)
            {
                if (shapePoint != null)
                {
                    return shapePoint;
                }
                else if (shape != null)
                {
                    return shape;
                }
            }

            return null;
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Point2 point, double radius, double scale, Modifier modifier)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapePoint =
                    mode.HasFlag(SelectionMode.Point)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetPoint(context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes, point, radius, scale, modifier, null) : null;

                var shape =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetShape(context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes, point, radius, scale, modifier) : null;

                if (shapePoint != null || shape != null)
                {
                    bool haveNewSelection =
                        (shapePoint != null && !(context.DocumentContainer.ContainerView.SelectionState?.IsSelected(shapePoint) ?? false))
                        || (shape != null && !(context.DocumentContainer.ContainerView.SelectionState?.IsSelected(shape) ?? false));

                    if (context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count >= 1
                        && !haveNewSelection
                        && !modifier.HasFlag(selectionModifier))
                    {
                        return true;
                    }
                    else
                    {
                        if (shapePoint != null)
                        {
                            if (modifier.HasFlag(selectionModifier))
                            {
                                if (context.DocumentContainer.ContainerView.SelectionState?.IsSelected(shapePoint) ?? false)
                                {
                                    shapePoint.Deselect(context.DocumentContainer.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shapePoint.Select(context.DocumentContainer.ContainerView.SelectionState);
                                }
                                return context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count > 0;
                            }
                            else
                            {
                                context.DocumentContainer.ContainerView.SelectionState?.Clear();
                                shapePoint.Select(context.DocumentContainer.ContainerView.SelectionState);
                                return true;
                            }
                        }
                        else if (shape != null)
                        {
                            if (modifier.HasFlag(selectionModifier))
                            {
                                if (context.DocumentContainer.ContainerView.SelectionState?.IsSelected(shape) ?? false)
                                {
                                    shape.Deselect(context.DocumentContainer.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                }
                                return context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count > 0;
                            }
                            else
                            {
                                context.DocumentContainer.ContainerView.SelectionState?.Clear();
                                shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Rect2 rect, double radius, double scale, Modifier modifier)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetShapes(context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes, rect, radius, scale, modifier) : null;

                if (shapes != null)
                {
                    if (shapes != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            foreach (var shape in shapes)
                            {
                                if (context.DocumentContainer.ContainerView.SelectionState?.IsSelected(shape) ?? false)
                                {
                                    shape.Deselect(context.DocumentContainer.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                }
                            }
                            return context.DocumentContainer.ContainerView.SelectionState?.Shapes.Count > 0;
                        }
                        else
                        {
                            context.DocumentContainer.ContainerView.SelectionState?.Clear();
                            foreach (var shape in shapes)
                            {
                                shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
