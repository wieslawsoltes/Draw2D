// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Draw2D.Core.Editor.Selection;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Tools
{
    public partial class SelectionTool : ToolBase
    {
        private RectangleShape _rectangle;
        private ShapeObject _hover;
        private double _originX;
        private double _originY;
        private double _previousX;
        private double _previousY;

        public enum State { None, Selection, Move };
        public State CurrentState = State.None;

        public override string Name { get { return "Selection"; } }

        public SelectionToolSettings Settings { get; set; }

        private void HoverShape(IToolContext context, ShapeObject shape)
        {
            if (shape != null)
            {
                _hover = shape;
                _hover.Select(context.Selected);
            }
        }

        private void DeHoverShape(IToolContext context)
        {
            if (_hover != null)
            {
                _hover.Deselect(context.Selected);
                _hover = null;
            }
        }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            DeHoverShape(context);

            var selected = SelectionHelper.TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0,
                modifier);
            if (selected == true)
            {
                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Modifier.Control))
                {
                    context.Selected.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        TopLeft = new PointShape(),
                        BottomRight = new PointShape()
                    };
                }

                _rectangle.TopLeft.X = x;
                _rectangle.TopLeft.Y = y;
                _rectangle.BottomRight.X = x;
                _rectangle.BottomRight.Y = y;
                _rectangle.Style = Settings?.SelectionStyle;
                context.WorkingContainer.Shapes.Add(_rectangle);

                context.Capture();
                context.Invalidate();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Release();
            context.Invalidate();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            DeHoverShape(context);

            SelectionHelper.TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                _rectangle.ToRect2(),
                Settings?.HitTestRadius ?? 7.0,
                modifier);

            context.WorkingContainer.Shapes.Remove(_rectangle);
            _rectangle = null;

            CurrentState = State.None;

            context.Release();
            context.Invalidate();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (!(_hover == null && context.Selected.Count > 0))
            {
                lock (context.Selected)
                {
                    var previous = _hover;

                    DeHoverShape(context);

                    var target = new Point2(x, y);
                    var result = SelectionHelper.TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        Settings?.HitTestRadius ?? 7.0);
                    if (result != null)
                    {
                        HoverShape(context, result);
                        context.Invalidate();
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.Invalidate();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Invalidate();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            double dx = x - _previousX;
            double dy = y - _previousY;
            _previousX = x;
            _previousY = y;

            if (context.Selected.Count == 1)
            {
                var shape = context.Selected.FirstOrDefault();

                shape.Move(context.Selected, dx, dy);

                if (shape is PointShape point)
                {
                    if (Settings.ConnectPoints)
                    {
                        var result = context.HitTest.TryToGetPoint(
                            context.CurrentContainer.Shapes,
                            new Point2(point.X, point.Y),
                            Settings?.ConnectTestRadius ?? 7.0);
                        if (result != point)
                        {
                            // TODO: Connect point.
                        }
                    }

                    if (Settings.DisconnectPoints)
                    {
                        if ((Math.Abs(_originX - point.X) > Settings.DisconnectTestRadius)
                            || (Math.Abs(_originY - point.Y) > Settings.DisconnectTestRadius))
                        {
                            // TODO: Disconnect point.
                        }
                    }
                }
            }
            else
            {
                foreach (var shape in context.Selected)
                {
                    shape.Move(context.Selected, dx, dy);
                }
            }

            context.Invalidate();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            DeHoverShape(context);

            if (_rectangle != null)
            {
                context.WorkingContainer.Shapes.Remove(_rectangle);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.Selected.Clear();
            }

            Filters?.ForEach(f => f.Clear(context));

            context.Release();
            context.Invalidate();
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.None:
                    {
                        LeftDownNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        LeftDownSelectionInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftUp(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Selection:
                    {
                        LeftUpSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        LeftUpMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Selection:
                    {
                        this.Clean(context);
                    }
                    break;
                case State.Move:
                    {
                        RightDownMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.None:
                    {
                        MoveNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        MoveSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        MoveMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
