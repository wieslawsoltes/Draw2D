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

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

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
                        context.Selected.Add(_rectangle.TopLeft);
                        context.Selected.Add(_rectangle.BottomRight);
                        _state = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        _state = State.TopLeft;
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                        context.WorkingContainer.Shapes.Remove(_rectangle);
                        context.Selected.Remove(_rectangle.TopLeft);
                        context.Selected.Remove(_rectangle.BottomRight);
                        context.CurrentContainer.Shapes.Add(_rectangle);
                        _rectangle = null;
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (_state)
            {
                case State.BottomRight:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

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

            _state = State.TopLeft;

            Filters.ForEach(f => f.Clear(context));

            if (_rectangle != null)
            {
                context.WorkingContainer.Shapes.Remove(_rectangle);
                context.Selected.Remove(_rectangle.TopLeft);
                context.Selected.Remove(_rectangle.BottomRight);
                _rectangle = null;
            }
        }
    }
}
