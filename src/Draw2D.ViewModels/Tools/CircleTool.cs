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
    public class CircleTool : BaseTool, ITool
    {
        private CircleToolSettings _settings;
        private CircleShape _circle = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public new string Title => "Circle";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public CircleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);
            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

            _circle = new CircleShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = point,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };
            _circle.Owner = context.ContainerView?.WorkingContainer;
            if (_circle.StartPoint.Owner == null)
            {
                _circle.StartPoint.Owner = _circle;
            }
            if (_circle.Point.Owner == null)
            {
                _circle.Point.Owner = _circle;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_circle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_circle);
            context.ContainerView?.SelectionState?.Select(_circle.StartPoint);
            context.ContainerView?.SelectionState?.Select(_circle.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_circle.Point);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _circle.Point = point;
            if (_circle.Point.Owner == null)
            {
                _circle.Point.Owner = _circle;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_circle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_circle);
            context.ContainerView?.SelectionState?.Deselect(_circle.StartPoint);
            _circle.Owner = context.ContainerView?.CurrentContainer;
            context.ContainerView?.CurrentContainer.Shapes.Add(_circle);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _circle = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _circle.Point.X = x;
            _circle.Point.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.StartPoint;

            FiltersClear(context);

            if (_circle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_circle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_circle);
                context.ContainerView?.SelectionState?.Deselect(_circle.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_circle.Point);
                _circle = null;
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
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        BottomRightInternal(context, x, y, modifier);
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
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MoveBottomRightInternal(context, x, y, modifier);
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
