// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
//using Draw2D.Renderers;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
//using SkiaSharp;
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

        public enum State
        {
            None,
            Selection,
            Move
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.None;

        [IgnoreDataMember]
        public string Title => "Selection";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _disconnected = false;

            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            FiltersClear(context);
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            context.ContainerView?.SelectionState?.Dehover();

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
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
                context.ContainerView?.InputService?.Capture?.Invoke();

                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Settings?.SelectionModifier ?? Modifier.Control))
                {
                    context.ContainerView?.SelectionState?.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        TopLeft = new PointShape(),
                        BottomRight = new PointShape()
                    };
                    _rectangle.TopLeft.Owner = _rectangle;
                    _rectangle.BottomRight.Owner = _rectangle;
                }

                _rectangle.TopLeft.X = x;
                _rectangle.TopLeft.Y = y;
                _rectangle.BottomRight.X = x;
                _rectangle.BottomRight.Y = y;
                _rectangle.StyleId = Settings?.SelectionStyle;
                context.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);

                context.ContainerView?.InputService?.Capture?.Invoke();
                context.ContainerView?.InputService?.Redraw?.Invoke();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.ContainerView?.SelectionState?.Dehover();

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                _rectangle.ToRect2(),
                radius,
                scale,
                modifier);

            context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            _rectangle = null;

            CurrentState = State.None;

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null && !(context.ContainerView.SelectionState?.Hovered == null && context.ContainerView.SelectionState?.Shapes.Count > 0))
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    var previous = context.ContainerView?.SelectionState?.Hovered;
                    var radius = Settings?.HitTestRadius ?? 7.0;
                    var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
                    var target = new Point2(x, y);
                    var shape = TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        radius,
                        scale);
                    if (shape != null)
                    {
                        if (shape != previous)
                        {
                            context.ContainerView?.SelectionState?.Dehover();
                            context.ContainerView?.SelectionState?.Hover(shape);
                            context.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.ContainerView?.SelectionState?.Dehover();
                            context.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            double dx = x - _previousX;
            double dy = y - _previousY;

            _previousX = x;
            _previousY = y;

            if (context.ContainerView?.SelectionState != null)
            {
                if (context.ContainerView.SelectionState?.Shapes.Count == 1)
                {
                    var shape = context.ContainerView.SelectionState?.Shapes.FirstOrDefault();

                    if (shape is IPointShape source)
                    {
                        if (Settings.ConnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            Connect(context, source);
                        }

                        if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            if (_disconnected == false)
                            {
                                double treshold = Settings.DisconnectTestRadius;
                                double tx = Math.Abs(_originX - source.X);
                                double ty = Math.Abs(_originY - source.Y);
                                if (tx > treshold || ty > treshold)
                                {
                                    Disconnect(context, source);
                                }
                            }
                        }
                    }

                    shape.Move(context.ContainerView.SelectionState, dx, dy);
                }
                else
                {
                    var selectedToDisconnect = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);
                    foreach (var shape in selectedToDisconnect)
                    {
                        if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            if (!(shape is IPointShape) && _disconnected == false)
                            {
                                Disconnect(context, shape);
                            }
                        }
                    }

                    var selectedToMove = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);
                    foreach (var shape in selectedToMove)
                    {
                        shape.Move(context.ContainerView.SelectionState, dx, dy);
                    }
                }

                context.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;

            context.ContainerView?.SelectionState?.Dehover();

            if (_rectangle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.ContainerView?.SelectionState?.Dehover();
                context.ContainerView?.SelectionState?.Clear();
            }

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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
            Delete(context);
        }

        public void Copy(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    _shapesToCopy = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);
                }
            }
        }

        public void Paste(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                if (_shapesToCopy != null)
                {
                    lock (context.ContainerView.SelectionState?.Shapes)
                    {
                        context.ContainerView?.SelectionState?.Dehover();
                        context.ContainerView?.SelectionState?.Clear();

                        Copy(context.ContainerView?.CurrentContainer, _shapesToCopy, context.ContainerView.SelectionState);

                        context.ContainerView?.InputService?.Redraw?.Invoke();

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void Delete(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    Delete(context.ContainerView?.CurrentContainer, context.ContainerView.SelectionState);

                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Group(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes.Reverse());

                    Delete(context);

                    var group = new GroupShape()
                    {
                        Title = "Group",
                        Points = new ObservableCollection<IPointShape>(),
                        Shapes = new ObservableCollection<IBaseShape>()
                    };

                    foreach (var shape in shapes)
                    {
                        if (!(shape is IPointShape))
                        {
                            group.Shapes.Add(shape);
                        }
                    }

                    group.Select(context.ContainerView.SelectionState);
                    context.ContainerView?.CurrentContainer.Shapes.Add(group);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        private void BreakGroup(IToolContext context, GroupShape group)
        {
            group.Deselect(context.ContainerView.SelectionState);

            context.ContainerView?.CurrentContainer?.Shapes?.Remove(group);
            context.ContainerView?.CurrentContainer?.MarkAsDirty(true);

            foreach (var shape in group.Shapes)
            {
                if (!(shape is IPointShape))
                {
                    context.ContainerView?.CurrentContainer.Shapes.Add(shape);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                    shape.Select(context.ContainerView.SelectionState);
                }
            }
        }

        private void BreakReference(IToolContext context, ReferenceShape reference)
        {
            reference.Deselect(context.ContainerView.SelectionState);

            context.ContainerView?.CurrentContainer?.Shapes?.Remove(reference);
            context.ContainerView?.CurrentContainer?.MarkAsDirty(true);

            if (reference.Template is IBaseShape shape)
            {
                var copy = (IBaseShape)shape.Copy(null);
                context.ContainerView?.CurrentContainer.Shapes.Add(copy);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                copy.Select(context.ContainerView.SelectionState);
            }
        }

        private void BreakFigure(IToolContext context, FigureShape figure)
        {
            foreach (var shape in figure.Shapes)
            {
                if (!(shape is IPointShape))
                {
                    context.ContainerView?.CurrentContainer.Shapes.Add(shape);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                    shape.Select(context.ContainerView.SelectionState);
                }
            }
        }

        private void BreakPath(IToolContext context, PathShape path)
        {
            path.Deselect(context.ContainerView.SelectionState);

            context.ContainerView?.CurrentContainer?.Shapes?.Remove(path);
            context.ContainerView?.CurrentContainer?.MarkAsDirty(true);

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
                            FillRule = path.FillRule,
                            Text = new Text(),
                            StyleId = path.StyleId
                        };

                        pathShape.Shapes.Add(figure);

                        context.ContainerView?.CurrentContainer.Shapes.Add(pathShape);
                        context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                        pathShape.Select(context.ContainerView.SelectionState);
                    }
                }
            }
        }

        public void Ungroup(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes.Reverse());

                    foreach (var shape in shapes)
                    {
                        if (shape is GroupShape group)
                        {
                            BreakGroup(context, group);
                        }
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Break(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes.Reverse());

                    foreach (var shape in shapes)
                    {
                        if (shape is PathShape path)
                        {
                            BreakPath(context, path);
                        }
                        else if (shape is GroupShape group)
                        {
                            BreakGroup(context, group);
                        }
                        else if (shape is ReferenceShape reference)
                        {
                            BreakReference(context, reference);
                        }
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Reference(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);

                    foreach (var shape in shapes)
                    {
                        if (shape is GroupShape group)
                        {
                            context.ContainerView?.Reference(group);
                        }
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void SelectAll(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    foreach (var shape in context.ContainerView?.CurrentContainer.Shapes)
                    {
                        shape.Select(context.ContainerView.SelectionState);
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void DeselectAll(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Connect(IToolContext context, IPointShape point)
        {
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;
            var target = context.HitTest?.TryToGetPoint(
                context.ContainerView?.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                scale,
                point);
            if (target != point)
            {
                foreach (var item in context.ContainerView?.CurrentContainer.Shapes)
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

        public void Disconnect(IToolContext context, IPointShape point)
        {
            foreach (var shape in context.ContainerView?.CurrentContainer.Shapes)
            {
                if (shape is IConnectable connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.ContainerView?.SelectionState?.Deselect(point);
                            context.ContainerView?.SelectionState?.Select(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, IBaseShape shape)
        {
            if (shape is IConnectable connectable)
            {
                if (context.ContainerView?.SelectionState != null)
                {
                    shape.Deselect(context.ContainerView.SelectionState);
                }
                _disconnected = connectable.Disconnect();
                if (context.ContainerView?.SelectionState != null)
                {
                    shape.Select(context.ContainerView.SelectionState);
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

        internal void Copy(ICanvasContainer container, IEnumerable<IBaseShape> shapes, ISelectionState selectionState)
        {
            var shared = GetPointsCopyDict(shapes);
            var points = new List<IPointShape>();

            foreach (var shape in shapes)
            {
                if (shape is ICopyable copyable)
                {
                    var copy = (IBaseShape)(copyable.Copy(shared));
                    if (copy != null && !(copy is IPointShape))
                    {
                        copy.GetPoints(points);
                        copy.Select(selectionState);
                        container.Shapes.Add(copy);
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
                    else
                    {
#if DEBUG
                        Log.WriteLine($"Failed to find owner shape: {point.Owner} for point: {point}.");
#endif
                    }
                }
            }
        }

        internal void Delete(ICanvasContainer container, ISelectionState selectionState)
        {
            // TODO: Very slow when using Contains.
            //var paths = new List<PathShape>(container.Shapes.OfType<PathShape>());
            //var groups = new List<GroupShape>(container.Shapes.OfType<GroupShape>());
            //var connectables = new List<ConnectableShape>(container.Shapes.OfType<ConnectableShape>());

            var shapesHash = new HashSet<IBaseShape>(container.Shapes);

            foreach (var shape in selectionState.Shapes)
            {
                if (shapesHash.Contains(shape))
                {
                    container.Shapes.Remove(shape);
                    container.MarkAsDirty(true);
                }
                /*
                else
                {
                    if (shape is IPointShape point)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(connectables, point);
                    }

                    if (paths.Count > 0)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(container, paths, shape);
                    }

                    if (groups.Count > 0)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(container, groups, shape);
                    }
                }
                */
            }
        }

        internal bool TryToDelete(IReadOnlyList<BaseShape> connectables, IPointShape point)
        {
            foreach (var connectable in connectables)
            {
                if (connectable.Points.Contains(point))
                {
                    connectable.Points.Remove(point);
                    connectable.MarkAsDirty(true);

                    return true;
                }
            }

            return false;
        }

        internal bool TryToDelete(ICanvasContainer container, IReadOnlyList<PathShape> paths, IBaseShape shape)
        {
            foreach (var path in paths)
            {
                foreach (var pathShape in path.Shapes)
                {
                    if (pathShape is FigureShape figure)
                    {
                        if (figure.Shapes.Contains(shape))
                        {
                            figure.Shapes.Remove(shape);
                            figure.MarkAsDirty(true);

                            if (figure.Shapes.Count <= 0)
                            {
                                path.Shapes.Remove(figure);
                                path.MarkAsDirty(true);

                                if (path.Shapes.Count <= 0)
                                {
                                    container.Shapes.Remove(path);
                                    container.MarkAsDirty(true);
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal bool TryToDelete(ICanvasContainer container, IReadOnlyList<GroupShape> groups, IBaseShape shape)
        {
            foreach (var group in groups)
            {
                if (group.Shapes.Contains(shape))
                {
                    group.Shapes.Remove(shape);
                    group.MarkAsDirty(true);

                    if (group.Shapes.Count <= 0)
                    {
                        container.Shapes.Remove(group);
                        container.MarkAsDirty(true);
                    }

                    return true;
                }
            }

            return false;
        }

        internal IBaseShape TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius, double scale)
        {
            // TODO: Experimental hit-testing.
            /*
            var bounds = new BoundsShapeRenderer(context.ContainerView?.CurrentContainer);

            //if (bounds.Contains((float)target.X, (float)target.Y, ContainsMode.Bounds, out var rootShape, out var childShape))
            //{
            //    bounds.Dispose();
            //    return rootShape;
            //}

            var rect = SKRect.Create((float)(target.X - radius), (float)(target.Y - radius), (float)(radius + radius), (float)(radius + radius));
            if (bounds.Intersects(ref rect, out var rootShape, out var childShape))
            {
                bounds.Dispose();
                return rootShape;
            }

            //var rect = SKRect.Create((float)(target.X - radius), (float)(target.Y - radius), (float)(radius + radius), (float)(radius + radius));
            //var geometry = new SKPath();
            //geometry.AddRect(rect);
            //if (bounds.Intersects(geometry, out var rootShape, out var childShape))
            //{
            //    bounds.Dispose();
            //    return rootShape;
            //}

            bounds.Dispose();

            return null;
            //*/

            ///*
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.ContainerView?.CurrentContainer.Shapes, target, radius, scale, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.ContainerView?.CurrentContainer.Shapes, target, radius, scale) : null;

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
            //*/
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Point2 point, double radius, double scale, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                var shapePoint =
                    mode.HasFlag(SelectionMode.Point)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetPoint(context.ContainerView?.CurrentContainer.Shapes, point, radius, scale, null) : null;

                var shape =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetShape(context.ContainerView?.CurrentContainer.Shapes, point, radius, scale) : null;

                if (shapePoint != null || shape != null)
                {
                    bool haveNewSelection =
                        (shapePoint != null && !(context.ContainerView.SelectionState?.IsSelected(shapePoint) ?? false))
                        || (shape != null && !(context.ContainerView.SelectionState?.IsSelected(shape) ?? false));

                    if (context.ContainerView.SelectionState?.Shapes.Count >= 1
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
                                if (context.ContainerView.SelectionState?.IsSelected(shapePoint) ?? false)
                                {
                                    shapePoint.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shapePoint.Select(context.ContainerView.SelectionState);
                                }
                                return context.ContainerView.SelectionState?.Shapes.Count > 0;
                            }
                            else
                            {
                                context.ContainerView.SelectionState?.Clear();
                                shapePoint.Select(context.ContainerView.SelectionState);
                                return true;
                            }
                        }
                        else if (shape != null)
                        {
                            if (modifier.HasFlag(selectionModifier))
                            {
                                if (context.ContainerView.SelectionState?.IsSelected(shape) ?? false)
                                {
                                    shape.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.ContainerView.SelectionState);
                                }
                                return context.ContainerView.SelectionState?.Shapes.Count > 0;
                            }
                            else
                            {
                                context.ContainerView.SelectionState?.Clear();
                                shape.Select(context.ContainerView.SelectionState);
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
            if (context.ContainerView?.SelectionState != null)
            {
                var shapes =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetShapes(context.ContainerView?.CurrentContainer.Shapes, rect, radius, scale) : null;

                if (shapes != null)
                {
                    if (shapes != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            foreach (var shape in shapes)
                            {
                                if (context.ContainerView.SelectionState?.IsSelected(shape) ?? false)
                                {
                                    shape.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.ContainerView.SelectionState);
                                }
                            }
                            return context.ContainerView.SelectionState?.Shapes.Count > 0;
                        }
                        else
                        {
                            context.ContainerView.SelectionState?.Clear();
                            foreach (var shape in shapes)
                            {
                                shape.Select(context.ContainerView.SelectionState);
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
