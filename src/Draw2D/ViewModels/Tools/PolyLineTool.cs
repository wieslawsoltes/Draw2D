using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class PolyLineTool : BaseTool, ITool
    {
        private PolyLineToolSettings _settings;
        private LineShape _line = null;
        private IList<IPointShape> _points = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public new string Title => "PolyLine";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PolyLineToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape startPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);
            IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

            _points = new ObservableCollection<IPointShape>();
            _line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = point,
                Text = new Text(),
                StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
            };
            _line.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
            if (_line.StartPoint.Owner == null)
            {
                _line.StartPoint.Owner = _line;
            }
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            _points.Add(_line.StartPoint);
            _points.Add(_line.Point);
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_line);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_line.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_line.Point);

            context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line.Point);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape firstPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _line.Point = firstPoint;
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            _points[_points.Count - 1] = _line.Point;

            if (!context.DocumentContainer?.ContainerView?.SelectionState?.IsSelected(_line.Point) ?? false)
            {
                context.DocumentContainer?.ContainerView?.SelectionState?.Select(_line.Point);
            }

            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_line);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            _line.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
            context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_line);
            context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);

            IPointShape startPoint = _points.Last();
            IPointShape nextPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

            _line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = nextPoint,
                Text = new Text(),
                StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
            };
            _line.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            _points.Add(_line.Point);
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_line);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_line.Point);

            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _line.Point.X = x;
            _line.Point.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_line);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line);
                _line = null;
            }

            if (_points != null)
            {
                foreach (var point in _points)
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(point);
                }
                _points = null;
            }

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point:
                    {
                        this.Clean(context);
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
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }
}
