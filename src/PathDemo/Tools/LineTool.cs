using System;
using System.Windows;

namespace PathDemo
{
    public class LineTool : ToolBase
    {
        public enum LineToolState { StartPoint, Point }
        public LineToolState CurrentState = LineToolState.StartPoint;
        public LineShape Line;

        public override void LeftDown(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case LineToolState.StartPoint:
                    var next = context.GetNextPoint(point);
                    Line = new LineShape()
                    {
                        StartPoint = context.GetNextPoint(point),
                        Point = PointShape.FromPoint(PointShape.ToPoint(next))
                    };
                    context.Shapes.Add(Line);
                    context.Selected.Add(Line);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = LineToolState.Point;
                    break;
                case LineToolState.Point:
                    Line.Point = context.GetNextPoint(point);
                    CurrentState = LineToolState.StartPoint;
                    context.Selected.Remove(Line);
                    Line = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void RightDown(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case LineToolState.Point:
                    CurrentState = LineToolState.StartPoint;
                    context.Shapes.Remove(Line);
                    context.Selected.Remove(Line);
                    Line = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void Move(IToolContext context, Point point)
        {
            switch (CurrentState)
            {
                case LineToolState.Point:
                    Line.Point.Update(point);
                    context.Invalidate();
                    break;
            }
        }
    }
}
