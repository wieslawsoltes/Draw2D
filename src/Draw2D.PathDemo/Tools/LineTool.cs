using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class LineTool : ToolBase
    {
        public override string Name { get { return "Line"; } }
        public enum LineToolState { StartPoint, Point }
        public LineToolState CurrentState = LineToolState.StartPoint;
        public LineShape Line;

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case LineToolState.StartPoint:
                    var next = context.GetNextPoint(x, y);
                    Line = new LineShape()
                    {
                        StartPoint = context.GetNextPoint(x, y),
                        Point = next.Clone(),
                        Style = context.CurrentStyle
                    };
                    context.CurrentContainer.Shapes.Add(Line);
                    context.Selected.Add(Line);
                    context.Capture();
                    context.Invalidate();
                    CurrentState = LineToolState.Point;
                    break;
                case LineToolState.Point:
                    Line.Point = context.GetNextPoint(x, y);
                    CurrentState = LineToolState.StartPoint;
                    context.Selected.Remove(Line);
                    Line = null;
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
                    context.CurrentContainer.Shapes.Remove(Line);
                    context.Selected.Remove(Line);
                    Line = null;
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
                    Line.Point.X = x;
                    Line.Point.Y = y;
                    context.Invalidate();
                    break;
            }
        }
    }
}
