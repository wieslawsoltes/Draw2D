// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class CubicBezierTool : BaseTool, ITool
    {
        private CubicBezierToolSettings _settings;
        private CubicBezierShape _cubicBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2,
            Point3
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public new string Title => "CubicBezier";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public CubicBezierToolSettings Settings
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
            IPointShape point1 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);
            IPointShape point2 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);
            IPointShape point3 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

            _cubicBezier = new CubicBezierShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                Text = new Text(),
                StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
            };
            _cubicBezier.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
            if (_cubicBezier.StartPoint.Owner == null)
            {
                _cubicBezier.StartPoint.Owner = _cubicBezier;
            }
            if (_cubicBezier.Point1.Owner == null)
            {
                _cubicBezier.Point1.Owner = _cubicBezier;
            }
            if (_cubicBezier.Point2.Owner == null)
            {
                _cubicBezier.Point2.Owner = _cubicBezier;
            }
            if (_cubicBezier.Point3.Owner == null)
            {
                _cubicBezier.Point3.Owner = _cubicBezier;
            }
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_cubicBezier);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier.Point1);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier.Point2);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier.Point3);

            context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point3;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point1);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_cubicBezier);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point1 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _cubicBezier.Point1 = point1;
            if (_cubicBezier.Point1.Owner == null)
            {
                _cubicBezier.Point1.Owner = _cubicBezier;
            }
            _cubicBezier.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
            context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_cubicBezier);
            context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _cubicBezier = null;

            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point2 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _cubicBezier.Point2 = point2;
            if (_cubicBezier.Point2.Owner == null)
            {
                _cubicBezier.Point2.Owner = _cubicBezier;
            }
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier.Point2);

            CurrentState = State.Point1;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point3 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _cubicBezier.Point3 = point3;
            if (_cubicBezier.Point3.Owner == null)
            {
                _cubicBezier.Point3.Owner = _cubicBezier;
            }
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_cubicBezier.Point3);

            CurrentState = State.Point2;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;
            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;
            _cubicBezier.Point3.X = x;
            _cubicBezier.Point3.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_cubicBezier != null)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_cubicBezier);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.StartPoint);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point1);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
                _cubicBezier = null;
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
                case State.Point1:
                    {
                        Point1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        Point2Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point3:
                    {
                        Point3Internal(context, x, y, modifier);
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
                case State.Point1:
                case State.Point2:
                case State.Point3:
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
                case State.Point1:
                    {
                        MovePoint1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        MovePoint2Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point3:
                    {
                        MovePoint3Internal(context, x, y, modifier);
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
