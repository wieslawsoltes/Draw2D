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
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

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

            IPointShape topLeft = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _ellipse = new EllipseShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_ellipse.TopLeft.Owner == null)
            {
                _ellipse.TopLeft.Owner = _ellipse;
            }
            if (_ellipse.BottomRight.Owner == null)
            {
                _ellipse.BottomRight.Owner = _ellipse;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_ellipse);
            context.ContainerView?.SelectionState?.Select(_ellipse.TopLeft);
            context.ContainerView?.SelectionState?.Select(_ellipse.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_ellipse.BottomRight);

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);

            _ellipse.BottomRight = bottomRight;
            if (_ellipse.BottomRight.Owner == null)
            {
                _ellipse.BottomRight.Owner = _ellipse;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_ellipse);
            context.ContainerView?.SelectionState?.Deselect(_ellipse.TopLeft);
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

            _ellipse.BottomRight.X = x;
            _ellipse.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            FiltersClear(context);

            if (_ellipse != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_ellipse);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.BottomRight);
                _ellipse = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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
