// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Tools
{
    public class CubicBezierTool : ToolBase
    {
        private CubicBezierShape _cubicBezier = null;

        public enum State { StartPoint, Point1, Point2, Point3 }
        public State CurrentState = State.StartPoint;

        public override string Name { get { return "CubicBezier"; } }

        public CubicBezierToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        var next = context.GetNextPoint(x, y, false, 0.0);
                        _cubicBezier = new CubicBezierShape()
                        {
                            StartPoint = next,
                            Point1 = next.Clone(),
                            Point2 = next.Clone(),
                            Point3 = next.Clone(),
                            Style = context.CurrentStyle
                        };
                        context.WorkingContainer.Shapes.Add(_cubicBezier);
                        context.Selected.Add(_cubicBezier);
                        context.Capture();
                        context.Invalidate();
                        CurrentState = State.Point3;
                    }
                    break;
                case State.Point1:
                    {
                        CurrentState = State.StartPoint;

                        _cubicBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);

                        context.WorkingContainer.Shapes.Remove(_cubicBezier);
                        context.Selected.Remove(_cubicBezier);

                        context.CurrentContainer.Shapes.Add(_cubicBezier);

                        _cubicBezier = null;
                        context.Release();
                        context.Invalidate();
                    }
                    break;
                case State.Point2:
                    {
                        _cubicBezier.Point1.X = x;
                        _cubicBezier.Point1.Y = y;
                        _cubicBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
                        CurrentState = State.Point1;
                        context.Invalidate();
                    }
                    break;
                case State.Point3:
                    {
                        _cubicBezier.Point2.X = x;
                        _cubicBezier.Point2.Y = y;
                        _cubicBezier.Point3 = context.GetNextPoint(x, y, false, 0.0);
                        CurrentState = State.Point2;
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
                case State.Point3:
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
                        _cubicBezier.Point1.X = x;
                        _cubicBezier.Point1.Y = y;
                        context.Invalidate();
                    }
                    break;
                case State.Point2:
                    {
                        _cubicBezier.Point1.X = x;
                        _cubicBezier.Point1.Y = y;
                        _cubicBezier.Point2.X = x;
                        _cubicBezier.Point2.Y = y;
                        context.Invalidate();
                    }
                    break;
                case State.Point3:
                    {
                        _cubicBezier.Point2.X = x;
                        _cubicBezier.Point2.Y = y;
                        _cubicBezier.Point3.X = x;
                        _cubicBezier.Point3.Y = y;
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.StartPoint;

            if (_cubicBezier != null)
            {
                context.WorkingContainer.Shapes.Remove(_cubicBezier);
                context.Selected.Remove(_cubicBezier);
                _cubicBezier = null;
            }

            context.Release();
            context.Invalidate();
        }
    }
}
