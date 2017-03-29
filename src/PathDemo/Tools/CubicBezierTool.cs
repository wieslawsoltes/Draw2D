using System;
using System.Windows;
using Draw2D.Models.Shapes;

namespace PathDemo.Tools
{
    public class CubicBezierTool : ToolBase
    {
        public enum CubicBezierToolState { StartPoint, Point1, Point2, Point3 }
        public CubicBezierToolState CurrentState = CubicBezierToolState.StartPoint;
        public CubicBezierShape CubicBezier;

        public override void LeftDown(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.StartPoint:
                    var next = context.GetNextPoint(point.X, point.Y);
                    CubicBezier = new CubicBezierShape()
                    {
                        StartPoint = next,
                        Point1 = next.Clone(),
                        Point2 = next.Clone(),
                        Point3 = next.Clone()
                    };
                    context.Shapes.Add(CubicBezier);
                    context.Selected.Add(CubicBezier);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = CubicBezierToolState.Point3;
                    break;
                case CubicBezierToolState.Point1:
                    CubicBezier.Point1 = context.GetNextPoint(point.X, point.Y);
                    CurrentState = CubicBezierToolState.StartPoint;
                    context.Selected.Remove(CubicBezier);
                    CubicBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point2:
                    CubicBezier.Point1.Update(point);
                    CubicBezier.Point2 = context.GetNextPoint(point.X, point.Y);
                    CurrentState = CubicBezierToolState.Point1;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point3:
                    CubicBezier.Point2.Update(point);
                    CubicBezier.Point3 = context.GetNextPoint(point.X, point.Y);
                    CurrentState = CubicBezierToolState.Point2;
                    context.Invalidate();
                    break;
            }
        }

        public override void RightDown(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.Point1:
                case CubicBezierToolState.Point2:
                case CubicBezierToolState.Point3:
                    CurrentState = CubicBezierToolState.StartPoint;
                    context.Shapes.Remove(CubicBezier);
                    context.Selected.Remove(CubicBezier);
                    CubicBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void Move(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.Point1:
                    CubicBezier.Point1.Update(point);
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point2:
                    CubicBezier.Point1.Update(point);
                    CubicBezier.Point2.Update(point);
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point3:
                    CubicBezier.Point2.Update(point);
                    CubicBezier.Point3.Update(point);
                    context.Invalidate();
                    break;
            }
        }
    }
}
