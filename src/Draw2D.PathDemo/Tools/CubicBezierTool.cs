using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class CubicBezierTool : ToolBase
    {
        public override string Name { get { return "CubicBezier"; } }
        public enum CubicBezierToolState { StartPoint, Point1, Point2, Point3 }
        public CubicBezierToolState CurrentState = CubicBezierToolState.StartPoint;
        public CubicBezierShape CubicBezier;

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.StartPoint:
                    var next = context.GetNextPoint(x, y, false, 0.0);
                    CubicBezier = new CubicBezierShape()
                    {
                        StartPoint = next,
                        Point1 = next.Clone(),
                        Point2 = next.Clone(),
                        Point3 = next.Clone(),
                        Style = context.CurrentStyle
                    };
                    context.CurrentContainer.Shapes.Add(CubicBezier);
                    context.Selected.Add(CubicBezier);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = CubicBezierToolState.Point3;
                    break;
                case CubicBezierToolState.Point1:
                    CubicBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = CubicBezierToolState.StartPoint;
                    context.Selected.Remove(CubicBezier);
                    CubicBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point2:
                    CubicBezier.Point1.X = x;
                    CubicBezier.Point1.Y = y;
                    CubicBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = CubicBezierToolState.Point1;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point3:
                    CubicBezier.Point2.X = x;
                    CubicBezier.Point2.Y = y;
                    CubicBezier.Point3 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = CubicBezierToolState.Point2;
                    context.Invalidate();
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.Point1:
                case CubicBezierToolState.Point2:
                case CubicBezierToolState.Point3:
                    CurrentState = CubicBezierToolState.StartPoint;
                    context.CurrentContainer.Shapes.Remove(CubicBezier);
                    context.Selected.Remove(CubicBezier);
                    CubicBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.Point1:
                    CubicBezier.Point1.X = x;
                    CubicBezier.Point1.Y = y;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point2:
                    CubicBezier.Point1.X = x;
                    CubicBezier.Point1.Y = y;
                    CubicBezier.Point2.X = x;
                    CubicBezier.Point2.Y = y;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point3:
                    CubicBezier.Point2.X = x;
                    CubicBezier.Point2.Y = y;
                    CubicBezier.Point3.X = x;
                    CubicBezier.Point3.Y = y;
                    context.Invalidate();
                    break;
            }
        }
    }
}
