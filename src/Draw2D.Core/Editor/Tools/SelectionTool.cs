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
        private bool _disconnected;
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
            _disconnected = false;

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
                    var shape = SelectionHelper.TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        Settings?.HitTestRadius ?? 7.0);
                    if (shape != null)
                    {
                        HoverShape(context, shape);
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

                if (shape is PointShape source)
                {
                    if (Settings.ConnectPoints && modifier.HasFlag(Modifier.Shift))
                    {
                        Connect(context, source);
                    }

                    if (Settings.DisconnectPoints && modifier.HasFlag(Modifier.Shift))
                    {
                        if (_disconnected == false)
                        {
                            double treshold = Settings.DisconnectTestRadius;
                            double tx = Math.Abs(_originX - source.X);
                            double ty = Math.Abs(_originY - source.Y);
                            if (tx > treshold || ty > treshold)
                            {
                                Disconnect(context, source);
                            }
                        }
                    }
                }

                shape.Move(context.Selected, dx, dy);
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

        private void Connect(IToolContext context, PointShape point)
        {
            var target = context.HitTest.TryToGetPoint(
                context.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                point);
            if (target != point)
            {
                foreach (var item in context.CurrentContainer.Shapes)
                {
                    if (item is ConnectableShape connectable)
                    {
                        if (connectable.Connect(point, target))
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void Disconnect(IToolContext context, PointShape point)
        {
            foreach (var shape in context.CurrentContainer.Shapes)
            {
                if (shape is ConnectableShape connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.Selected.Remove(point);
                            context.Selected.Add(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;

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
