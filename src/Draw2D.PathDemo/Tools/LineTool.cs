using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class LineTool : ToolBase
    {
        private LineShape _line = null;

        public enum LineToolState { StartPoint, Point }
        public LineToolState CurrentState = LineToolState.StartPoint;

        public override string Name { get { return "Line"; } }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case LineToolState.StartPoint:
                    var next = context.GetNextPoint(x, y, false, 0.0);
                    _line = new LineShape()
                    {
                        StartPoint = next,
                        Point = next.Clone(),
                        Style = context.CurrentStyle
                    };
                    context.CurrentContainer.Shapes.Add(_line);
                    context.Selected.Add(_line);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = LineToolState.Point;
                    break;
                case LineToolState.Point:
                    _line.Point = context.GetNextPoint(x, y, false, 0.0);
                    CurrentState = LineToolState.StartPoint;
                    context.Selected.Remove(_line);
                    _line = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case LineToolState.Point:
                    CurrentState = LineToolState.StartPoint;
                    context.CurrentContainer.Shapes.Remove(_line);
                    context.Selected.Remove(_line);
                    _line = null;
                    context.Release();
                    context.Invalidate();
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case LineToolState.Point:
                    _line.Point.X = x;
                    _line.Point.Y = y;
                    context.Invalidate();
                    break;
            }
        }
    }
}
