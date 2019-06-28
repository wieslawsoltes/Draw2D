// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public class TwoPointsTool : BaseTool, ITool
    {
        private TwoPointsToolSettings _settings;
        private ITwoPointsShape _shape = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "TwoPoints";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TwoPointsToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private ITwoPointsShape CreateShape(IToolContext context, IPointShape startPoint, IPointShape point, TwoPointsShapeType shapeType)
        {
            switch (shapeType)
            {
                case TwoPointsShapeType.Line:
                    return new LineShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        StartPoint = startPoint,
                        Point = point,
                        Text = new Text(),
                        StyleId = context.StyleLibrary?.CurrentStyle?.Title
                    };
                case TwoPointsShapeType.Rectangle:
                    return new RectangleShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        StartPoint = startPoint,
                        Point = point,
                        Text = new Text(),
                        StyleId = context.StyleLibrary?.CurrentStyle?.Title
                    };
                case TwoPointsShapeType.Ellipse:
                    return new EllipseShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        StartPoint = startPoint,
                        Point = point,
                        Text = new Text(),
                        StyleId = context.StyleLibrary?.CurrentStyle?.Title
                    };
                case TwoPointsShapeType.Text:
                    return new TextShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        StartPoint = startPoint,
                        Point = point,
                        Text = new Text(),
                        StyleId = context.StyleLibrary?.CurrentStyle?.Title
                    };
                default:
                    return null;
            }
        }

        public IList<ITwoPointsShape> SplitByIntersections(IToolContext context, IEnumerable<IPointIntersection> intersections, ITwoPointsShape target)
        {
            var points = new List<IPointShape>(intersections.SelectMany(i => i.Intersections));
            points.Insert(0, target.StartPoint);
            points.Insert(points.Count, target.Point);

            var unique = new List<IPointShape>(
                points.Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                      .Select(p => new PointShape(p.X, p.Y, context.PointTemplate)));

            var shapes = new ObservableCollection<ITwoPointsShape>();
            for (int i = 0; i < unique.Count - 1; i++)
            {
                var startPoint = unique[i];
                var point = unique[i + 1];
                var shape = CreateShape(context, startPoint, point, Settings?.ShapeType ?? TwoPointsShapeType.Line);
                shape.StartPoint.Owner = shape;
                shape.Point.Owner = shape;
                context.ContainerView?.CurrentContainer.Shapes.Add(shape);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                shapes.Add(shape);
            }

            return shapes;
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);
            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0);

            _shape = CreateShape(context, startPoint, point, Settings?.ShapeType ?? TwoPointsShapeType.Line);
            if (_shape.StartPoint.Owner == null)
            {
                _shape.StartPoint.Owner = _shape;
            }
            if (_shape.Point.Owner == null)
            {
                _shape.Point.Owner = _shape;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_shape);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_shape);
            context.ContainerView?.SelectionState?.Select(_shape.StartPoint);
            context.ContainerView?.SelectionState?.Select(_shape.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_shape);
            context.ContainerView?.SelectionState?.Deselect(_shape.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_shape.Point);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_shape);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);

            _shape.Point = point;
            if (_shape.Point.Owner == null)
            {
                _shape.Point.Owner = _shape;
            }

            IntersectionsClear(context);
            IntersectionsFind(context, _shape);

            if ((Settings?.SplitIntersections ?? false) && HaveIntersections())
            {
                SplitByIntersections(context, Intersections, _shape);
            }
            else
            {
                context.ContainerView?.CurrentContainer.Shapes.Add(_shape);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _shape = null;

            IntersectionsClear(context);
            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _shape.Point.X = x;
            _shape.Point.Y = y;

            IntersectionsClear(context);
            IntersectionsFind(context, _shape);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            IntersectionsClear(context);
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_shape != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_shape);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_shape);
                context.ContainerView?.SelectionState?.Deselect(_shape.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_shape.Point);
                _shape = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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
