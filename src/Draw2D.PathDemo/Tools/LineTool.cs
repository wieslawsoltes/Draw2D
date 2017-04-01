using Draw2D.Editor;
using Draw2D.Editor.Tools;
using Draw2D.Models.Shapes;

namespace Draw2D.PathDemo.Tools
{
    public class LineTool : ToolBase
    {
        private LineShape _line = null;

        public enum State { StartPoint, Point }
        public State CurrentState = State.StartPoint;

        public override string Name { get { return "Line"; } }

        public LineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
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
                        CurrentState = State.Point;
                    }
                    break;
                case State.Point:
                    {
                        _line.Point = context.GetNextPoint(x, y, false, 0.0);
                        CurrentState = State.StartPoint;
                        context.Selected.Remove(_line);
                        _line = null;
                        context.Release();
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point:
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
                case State.Point:
                    {
                        _line.Point.X = x;
                        _line.Point.Y = y;
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.StartPoint;
            if (_line != null)
            {
                context.CurrentContainer.Shapes.Remove(_line);
                context.Selected.Remove(_line); 
            }
            _line = null;
            context.Release();
            context.Invalidate();
        }
    }
}
