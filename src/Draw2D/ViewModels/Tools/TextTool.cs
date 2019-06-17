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
    public class TextTool : BaseTool, ITool
    {
        private TextToolSettings _settings;
        private TextShape _text = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Text";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TextToolSettings Settings
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

            _text = new TextShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = new Text("Text"),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_text.TopLeft.Owner == null)
            {
                _text.TopLeft.Owner = _text;
            }
            if (_text.BottomRight.Owner == null)
            {
                _text.BottomRight.Owner = _text;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_text);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_text);
            context.ContainerView?.SelectionState?.Select(_text.TopLeft);
            context.ContainerView?.SelectionState?.Select(_text.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_text);
            context.ContainerView?.SelectionState?.Deselect(_text.BottomRight);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale);

            _text.BottomRight = bottomRight;
            _text.BottomRight.Y = y;
            if (_text.BottomRight.Owner == null)
            {
                _text.BottomRight.Owner = _text;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_text);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_text.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_text);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _text = null;

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

            _text.BottomRight.X = x;
            _text.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            FiltersClear(context);

            if (_text != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_text);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_text);
                context.ContainerView?.SelectionState?.Deselect(_text.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_text.BottomRight);
                _text = null;
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
