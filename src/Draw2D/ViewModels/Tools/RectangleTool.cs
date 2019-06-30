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
    public class RectangleTool : BaseTool, ITool
    {
        private RectangleToolSettings _settings;
        private RectangleShape _rectangle = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "Rectangle";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public RectangleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape topLeft = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);
            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0);

            _rectangle = new RectangleShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = topLeft,
                Point = bottomRight,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };
            if (_rectangle.StartPoint.Owner == null)
            {
                _rectangle.StartPoint.Owner = _rectangle;
            }
            if (_rectangle.Point.Owner == null)
            {
                _rectangle.Point.Owner = _rectangle;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_rectangle);
            context.ContainerView?.SelectionState?.Select(_rectangle.StartPoint);
            context.ContainerView?.SelectionState?.Select(_rectangle.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_rectangle);
            context.ContainerView?.SelectionState?.Deselect(_rectangle.Point);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);

            _rectangle.Point = bottomRight;
            _rectangle.Point.Y = y;
            if (_rectangle.Point.Owner == null)
            {
                _rectangle.Point.Owner = _rectangle;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_rectangle.StartPoint);
            context.ContainerView?.CurrentContainer.Shapes.Add(_rectangle);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _rectangle = null;

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

            _rectangle.Point.X = x;
            _rectangle.Point.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.StartPoint;

            FiltersClear(context);

            if (_rectangle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_rectangle);
                context.ContainerView?.SelectionState?.Deselect(_rectangle.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_rectangle.Point);
                _rectangle = null;
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
