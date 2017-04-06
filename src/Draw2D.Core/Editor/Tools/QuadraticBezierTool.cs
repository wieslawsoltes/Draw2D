// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor.Tools
{
    public class QuadraticBezierTool : ToolBase
    {
        public QuadraticBezierShape _quadraticBezier = null;

        public enum State { StartPoint, Point1, Point2 }
        public State CurrentState = State.StartPoint;

        public override string Name { get { return "QuadraticBezier"; } }

        public QuadraticBezierToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        var next = context.GetNextPoint(x, y, false, 0.0);
                        _quadraticBezier = new QuadraticBezierShape()
                        {
                            StartPoint = next,
                            Point1 = next.Clone(),
                            Point2 = next.Clone(),
                            Style = context.CurrentStyle
                        };
                        context.WorkingContainer.Shapes.Add(_quadraticBezier);
                        context.Selected.Add(_quadraticBezier);
                        context.Selected.Add(_quadraticBezier.StartPoint);
                        context.Selected.Add(_quadraticBezier.Point1);
                        context.Selected.Add(_quadraticBezier.Point2);
                        context.Capture();
                        context.Invalidate();
                        CurrentState = State.Point2;
                    }
                    break;
                case State.Point1:
                    {
                        CurrentState = State.StartPoint;

                        context.Selected.Remove(_quadraticBezier);
                        context.Selected.Remove(_quadraticBezier.StartPoint);
                        context.Selected.Remove(_quadraticBezier.Point1);
                        context.Selected.Remove(_quadraticBezier.Point2);
                        context.WorkingContainer.Shapes.Remove(_quadraticBezier);

                        _quadraticBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);

                        context.CurrentContainer.Shapes.Add(_quadraticBezier);

                        _quadraticBezier = null;
                        context.Release();
                        context.Invalidate();
                    }
                    break;
                case State.Point2:
                    {
                        _quadraticBezier.Point1.X = x;
                        _quadraticBezier.Point1.Y = y;

                        context.Selected.Remove(_quadraticBezier.Point2);
                        _quadraticBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
                        context.Selected.Add(_quadraticBezier.Point2);

                        CurrentState = State.Point1;
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
                case State.Point1:
                case State.Point2:
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
                case State.Point1:
                    {
                        _quadraticBezier.Point1.X = x;
                        _quadraticBezier.Point1.Y = y;
                        context.Invalidate();
                    }
                    break;
                case State.Point2:
                    {
                        _quadraticBezier.Point1.X = x;
                        _quadraticBezier.Point1.Y = y;
                        _quadraticBezier.Point2.X = x;
                        _quadraticBezier.Point2.Y = y;
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.StartPoint;

            if (_quadraticBezier != null)
            {
                context.WorkingContainer.Shapes.Remove(_quadraticBezier);
                context.Selected.Remove(_quadraticBezier);
                context.Selected.Remove(_quadraticBezier.StartPoint);
                context.Selected.Remove(_quadraticBezier.Point1);
                context.Selected.Remove(_quadraticBezier.Point2);
                _quadraticBezier = null;
            }

            context.Release();
            context.Invalidate();
        }
    }
}
