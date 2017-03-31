using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class QuadraticBezierTool : ToolBase
    {
        public override string Name { get { return "QuadraticBezier"; } }
        public enum QuadraticBezierToolState { StartPoint, Point1, Point2 }
        public QuadraticBezierToolState CurrentState = QuadraticBezierToolState.StartPoint;
        public QuadraticBezierShape QuadraticBezier;

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case QuadraticBezierToolState.StartPoint:
                    var next = context.GetNextPoint(x, y, false, 0.0);
                    QuadraticBezier = new QuadraticBezierShape()
                    {
                        StartPoint = next,
                        Point1 = next.Clone(),
                        Point2 = next.Clone(),
                        Style = context.CurrentStyle
                    };
                    context.CurrentContainer.Shapes.Add(QuadraticBezier);
                    context.Selected.Add(QuadraticBezier);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = QuadraticBezierToolState.Point2;
                    break;
                case QuadraticBezierToolState.Point1:
                    QuadraticBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = QuadraticBezierToolState.StartPoint;
                    context.Selected.Remove(QuadraticBezier);
                    QuadraticBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
                case QuadraticBezierToolState.Point2:
                    QuadraticBezier.Point1.X = x;
                    QuadraticBezier.Point1.Y = y;
                    QuadraticBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = QuadraticBezierToolState.Point1;
                    context.Invalidate();
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case QuadraticBezierToolState.Point1:
                case QuadraticBezierToolState.Point2:
                    CurrentState = QuadraticBezierToolState.StartPoint;
                    context.CurrentContainer.Shapes.Remove(QuadraticBezier);
                    context.Selected.Remove(QuadraticBezier);
                    QuadraticBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case QuadraticBezierToolState.Point1:
                    QuadraticBezier.Point1.X = x;
                    QuadraticBezier.Point1.Y = y;
                    context.Invalidate();
                    break;
                case QuadraticBezierToolState.Point2:
                    QuadraticBezier.Point1.X = x;
                    QuadraticBezier.Point1.Y = y;
                    QuadraticBezier.Point2.X = x;
                    QuadraticBezier.Point2.Y = y;
                    context.Invalidate();
                    break;
            }
        }
    }
}
