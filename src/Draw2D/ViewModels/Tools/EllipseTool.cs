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
    public class EllipseTool : BaseTool, ITool
    {
        private EllipseToolSettings _settings;
        private EllipseShape _ellipse = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "Ellipse";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EllipseToolSettings Settings
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

            _ellipse = new EllipseShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = topLeft,
                Point = bottomRight,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_ellipse.StartPoint.Owner == null)
            {
                _ellipse.StartPoint.Owner = _ellipse;
            }
            if (_ellipse.Point.Owner == null)
            {
                _ellipse.Point.Owner = _ellipse;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_ellipse);
            context.ContainerView?.SelectionState?.Select(_ellipse.StartPoint);
            context.ContainerView?.SelectionState?.Select(_ellipse.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_ellipse.Point);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);

            _ellipse.Point = bottomRight;
            if (_ellipse.Point.Owner == null)
            {
                _ellipse.Point.Owner = _ellipse;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_ellipse);
            context.ContainerView?.SelectionState?.Deselect(_ellipse.StartPoint);
            context.ContainerView?.CurrentContainer.Shapes.Add(_ellipse);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _ellipse = null;

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

            _ellipse.Point.X = x;
            _ellipse.Point.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.StartPoint;

            FiltersClear(context);

            if (_ellipse != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_ellipse);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.Point);
                _ellipse = null;
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
