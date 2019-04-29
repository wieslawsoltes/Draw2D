// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    public class TextToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    public class TextTool : ViewModelBase, ITool
    {
        private TextShape _text = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        public State CurrentState { get; set; } = State.TopLeft;

        public string Title => "Text";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public TextToolSettings Settings { get; set; }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _text = new TextShape()
            {
                TopLeft = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                BottomRight = context.GetNextPoint(x, y, false, 0.0),
                Text = new TextObject()
                {
                    Value = "Text"
                },
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_text);
            context.Selection.Selected.Add(_text);
            context.Selection.Selected.Add(_text.TopLeft);
            context.Selection.Selected.Add(_text.BottomRight);

            context.ZoomControl?.Capture?.Invoke();
            context.ZoomControl?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.TopLeft;

            context.Selection.Selected.Remove(_text);
            context.Selection.Selected.Remove(_text.BottomRight);
            _text.BottomRight = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _text.BottomRight.Y = y;
            context.WorkingContainer.Shapes.Remove(_text);
            context.Selection.Selected.Remove(_text.TopLeft);
            context.CurrentContainer.Shapes.Add(_text);
            _text = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ZoomControl?.Release?.Invoke();
            context.ZoomControl?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ZoomControl?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _text.BottomRight.X = x;
            _text.BottomRight.Y = y;

            context.ZoomControl?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            Filters?.ForEach(f => f.Clear(context));

            if (_text != null)
            {
                context.WorkingContainer.Shapes.Remove(_text);
                context.Selection.Selected.Remove(_text);
                context.Selection.Selected.Remove(_text.TopLeft);
                context.Selection.Selected.Remove(_text.BottomRight);
                _text = null;
            }

            context.ZoomControl?.Release?.Invoke();
            context.ZoomControl?.Redraw?.Invoke();
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
