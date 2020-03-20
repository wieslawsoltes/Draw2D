using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class LineTool : BaseTool, ITool
    {
        private LineToolSettings _settings;
        private LineShape _line = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public new string Title => "Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public static IList<LineShape> SplitByIntersections(IToolContext context, IEnumerable<IPointIntersection> intersections, LineShape target)
        {
            var points = new List<IPointShape>(intersections.SelectMany(i => i.Intersections));
            points.Insert(0, target.StartPoint);
            points.Insert(points.Count, target.Point);

            var unique = new List<IPointShape>(
                points.Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                      .Select(p => new PointShape(p.X, p.Y, context?.DocumentContainer?.PointTemplate)));

            var lines = new ObservableCollection<LineShape>();
            for (int i = 0; i < unique.Count - 1; i++)
            {
                var startPoint = unique[i];
                var point = unique[i + 1];
                var line = new LineShape(startPoint, point)
                {
                    Points = new ObservableCollection<IPointShape>(),
                    StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
                };
                line.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                line.StartPoint.Owner = line;
                line.Point.Owner = line;
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(line);
                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
                lines.Add(line);
            }

            return lines;
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape startPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);
            IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

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

            CurrentState = State.StartPoint;

            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line.Point);
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_line);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _line.Point = point;
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }

            IntersectionsClear(context);
            IntersectionsFind(context, _line);

            if ((Settings?.SplitIntersections ?? false) && HaveIntersections())
            {
                SplitByIntersections(context, Intersections, _line);
            }
            else
            {
                _line.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_line);
                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _line = null;

            IntersectionsClear(context);
            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
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

            IntersectionsClear(context);
            IntersectionsFind(context, _line);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            IntersectionsClear(context);
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_line);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line.StartPoint);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_line.Point);
                _line = null;
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
