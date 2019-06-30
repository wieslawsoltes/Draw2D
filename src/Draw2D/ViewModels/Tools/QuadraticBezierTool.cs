// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierTool : BaseTool, ITool
    {
        private QuadraticBezierToolSettings _settings;
        private QuadraticBezierShape _quadraticBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public new string Title => "QuadraticBezier";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public QuadraticBezierToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);
            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0);
            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0);

            _quadraticBezier = new QuadraticBezierShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point1 = point1,
                Point2 = point2,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };
            if (_quadraticBezier.StartPoint.Owner == null)
            {
                _quadraticBezier.StartPoint.Owner = _quadraticBezier;
            }
            if (_quadraticBezier.Point1.Owner == null)
            {
                _quadraticBezier.Point1.Owner = _quadraticBezier;
            }
            if (_quadraticBezier.Point2.Owner == null)
            {
                _quadraticBezier.Point2.Owner = _quadraticBezier;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_quadraticBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.StartPoint);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point1);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point2);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point1);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_quadraticBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);

            _quadraticBezier.Point1 = point1;
            if (_quadraticBezier.Point1.Owner == null)
            {
                _quadraticBezier.Point1.Owner = _quadraticBezier;
            }
            context.ContainerView?.CurrentContainer.Shapes.Add(_quadraticBezier);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _quadraticBezier = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);

            _quadraticBezier.Point2 = point2;
            if (_quadraticBezier.Point2.Owner == null)
            {
                _quadraticBezier.Point2.Owner = _quadraticBezier;
            }
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;
            _quadraticBezier.Point2.X = x;
            _quadraticBezier.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_quadraticBezier != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_quadraticBezier);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point1);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
                _quadraticBezier = null;
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
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }
}
