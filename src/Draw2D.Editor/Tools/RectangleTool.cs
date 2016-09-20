using System;
using System.Linq;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class RectangleTool : ToolBase
    {
        private enum State { TopLeft, BottomRight };
        private State _state = State.TopLeft;
        private RectangleShape _rectangle = null;

        public override string Name { get { return "Rectangle"; } }

        public RectangleToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y)
        {
            base.LeftDown(context, x, y);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.TopLeft:
                    {
                        _rectangle = new RectangleShape(
                            new PointShape(x, y, context.PointShape),
                            new PointShape(x, y, context.PointShape));
                        _rectangle.Style = context.Style;
                        context.WorkingContainer.Shapes.Add(_rectangle);
                        context.Renderer.Selected.Add(_rectangle.TopLeft);
                        context.Renderer.Selected.Add(_rectangle.BottomRight);
                        _state = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        _state = State.TopLeft;
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                        context.WorkingContainer.Shapes.Remove(_rectangle);
                        context.Renderer.Selected.Remove(_rectangle.TopLeft);
                        context.Renderer.Selected.Remove(_rectangle.BottomRight);
                        context.Container.Shapes.Add(_rectangle);
                        _rectangle = null;
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y)
        {
            base.RightDown(context, x, y);

            switch (_state)
            {
                case State.BottomRight:
                    {
                        context.WorkingContainer.Shapes.Remove(_rectangle);
                        context.Renderer.Selected.Remove(_rectangle.TopLeft);
                        context.Renderer.Selected.Remove(_rectangle.BottomRight);
                        _rectangle = null;
                        _state = State.TopLeft;
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y)
        {
            base.Move(context, x, y);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.BottomRight:
                    {
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);
            Filters.ForEach(f => f.Clear(context));
        }
    }
}
