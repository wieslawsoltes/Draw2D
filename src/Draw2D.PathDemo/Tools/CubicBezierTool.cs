using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class CubicBezierTool : ToolBase
    {
        private CubicBezierShape _cubicBezier = null;

        public enum CubicBezierToolState { StartPoint, Point1, Point2, Point3 }
        public CubicBezierToolState CurrentState = CubicBezierToolState.StartPoint;

        public override string Name { get { return "CubicBezier"; } }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.StartPoint:
                    var next = context.GetNextPoint(x, y, false, 0.0);
                    _cubicBezier = new CubicBezierShape()
                    {
                        StartPoint = next,
                        Point1 = next.Clone(),
                        Point2 = next.Clone(),
                        Point3 = next.Clone(),
                        Style = context.CurrentStyle
                    };
                    context.CurrentContainer.Shapes.Add(_cubicBezier);
                    context.Selected.Add(_cubicBezier);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = CubicBezierToolState.Point3;
                    break;
                case CubicBezierToolState.Point1:
                    _cubicBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = CubicBezierToolState.StartPoint;
                    context.Selected.Remove(_cubicBezier);
                    _cubicBezier = null;
                    context.Release();
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point2:
                    _cubicBezier.Point1.X = x;
                    _cubicBezier.Point1.Y = y;
                    _cubicBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = CubicBezierToolState.Point1;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point3:
                    _cubicBezier.Point2.X = x;
                    _cubicBezier.Point2.Y = y;
                    _cubicBezier.Point3 = context.GetNextPoint(x, y, false, 0.0);
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
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case CubicBezierToolState.Point1:
                    _cubicBezier.Point1.X = x;
                    _cubicBezier.Point1.Y = y;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point2:
                    _cubicBezier.Point1.X = x;
                    _cubicBezier.Point1.Y = y;
                    _cubicBezier.Point2.X = x;
                    _cubicBezier.Point2.Y = y;
                    context.Invalidate();
                    break;
                case CubicBezierToolState.Point3:
                    _cubicBezier.Point2.X = x;
                    _cubicBezier.Point2.Y = y;
                    _cubicBezier.Point3.X = x;
                    _cubicBezier.Point3.Y = y;
                    context.Invalidate();
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = CubicBezierToolState.StartPoint;
            if (_cubicBezier != null)
            {
                context.CurrentContainer.Shapes.Remove(_cubicBezier);
                context.Selected.Remove(_cubicBezier); 
            }
            _cubicBezier = null;
            context.Release();
            context.Invalidate();
        }
    }
}
