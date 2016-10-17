using System;
using System.Windows;

namespace PathDemo
{
    public class QuadraticBezierTool : ToolBase
    {
        public enum QuadraticBezierToolState { StartPoint, Point1, Point2 }
        public QuadraticBezierToolState CurrentState = QuadraticBezierToolState.StartPoint;
        public QuadraticBezierShape QuadraticBezier;

        public override void LeftDown(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case QuadraticBezierToolState.StartPoint:
                    var next = context.GetNextPoint(point);
                    QuadraticBezier = new QuadraticBezierShape()
                    {
                        StartPoint = next,
                        Point1 = PointShape.FromPoint(PointShape.ToPoint(next)),
                        Point2 = PointShape.FromPoint(PointShape.ToPoint(next))
                    };
                    context.Shapes.Add(QuadraticBezier);
                    context.Selected.Add(QuadraticBezier);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = QuadraticBezierToolState.Point2;
                    break;
                case QuadraticBezierToolState.Point1:
                    QuadraticBezier.Point1 = context.GetNextPoint(point);
                    CurrentState = QuadraticBezierToolState.StartPoint;
                    context.Selected.Remove(QuadraticBezier);
                    QuadraticBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
                case QuadraticBezierToolState.Point2:
                    QuadraticBezier.Point1.Update(point);
                    QuadraticBezier.Point2 = context.GetNextPoint(point);
                    CurrentState = QuadraticBezierToolState.Point1;
                    context.Invalidate();
                    break;
            }
        }

        public override void RightDown(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case QuadraticBezierToolState.Point1:
                case QuadraticBezierToolState.Point2:
                    CurrentState = QuadraticBezierToolState.StartPoint;
                    context.Shapes.Remove(QuadraticBezier);
                    context.Selected.Remove(QuadraticBezier);
                    QuadraticBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void Move(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case QuadraticBezierToolState.Point1:
                    QuadraticBezier.Point1.Update(point);
                    context.Invalidate();
                    break;
                case QuadraticBezierToolState.Point2:
                    QuadraticBezier.Point1.Update(point);
                    QuadraticBezier.Point2.Update(point);
                    context.Invalidate();
                    break;
            }
        }
    }
}
