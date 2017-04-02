﻿using Draw2D.Editor;
using Draw2D.Editor.Tools;
using Draw2D.Core.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class LineTool : ToolBase
    {
        private LineShape _line = null;

        public enum State { StartPoint, Point }
        public State CurrentState = State.StartPoint;

        public override string Name { get { return "Line"; } }

        public LineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        var next = context.GetNextPoint(x, y, false, 0.0);
                        _line = new LineShape()
                        {
                            StartPoint = next,
                            Point = next.Clone(),
                            Style = context.CurrentStyle
                        };
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Selected.Add(_line);
                        context.Capture();
                        context.Invalidate();
                        CurrentState = State.Point;
                    }
                    break;
                case State.Point:
                    {
                        CurrentState = State.StartPoint;

                        _line.Point = context.GetNextPoint(x, y, false, 0.0);

                        context.WorkingContainer.Shapes.Remove(_line);
                        context.Selected.Remove(_line);

                        context.CurrentContainer.Shapes.Add(_line);

                        _line = null;
                        context.Release();
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Point:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Point:
                    {
                        _line.Point.X = x;
                        _line.Point.Y = y;
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                context.Selected.Remove(_line);
                _line = null;
            }

            context.Release();
            context.Invalidate();
        }
    }
}