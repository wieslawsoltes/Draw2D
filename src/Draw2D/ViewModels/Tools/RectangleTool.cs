// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    public class RectangleToolSettings : SettingsBase
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

    public class RectangleTool : ViewModelBase, ITool
    {
        private RectangleShape _rectangle = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        public State CurrentState { get; set; } = State.TopLeft;

        public string Title => "Rectangle";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public RectangleToolSettings Settings { get; set; }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _rectangle = new RectangleShape()
            {
                TopLeft = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                BottomRight = context.GetNextPoint(x, y, false, 0.0),
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_rectangle);
            context.Selection.Selected.Add(_rectangle.TopLeft);
            context.Selection.Selected.Add(_rectangle.BottomRight);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.TopLeft;

            context.Selection.Selected.Remove(_rectangle.BottomRight);
            _rectangle.BottomRight = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _rectangle.BottomRight.Y = y;
            context.WorkingContainer.Shapes.Remove(_rectangle);
            context.Selection.Selected.Remove(_rectangle.TopLeft);
            context.CurrentContainer.Shapes.Add(_rectangle);
            _rectangle = null;

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            Filters?.ForEach(f => f.Clear(context));

            if (_rectangle != null)
            {
                context.WorkingContainer.Shapes.Remove(_rectangle);
                context.Selection.Selected.Remove(_rectangle.TopLeft);
                context.Selection.Selected.Remove(_rectangle.BottomRight);
                _rectangle = null;
            }

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
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
