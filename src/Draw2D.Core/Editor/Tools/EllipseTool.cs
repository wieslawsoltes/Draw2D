using System.Linq;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class EllipseTool : ToolBase
    {
        private enum State { TopLeft, BottomRight };
        private State _state = State.TopLeft;
        private EllipseShape _ellipse = null;

        public override string Name { get { return "Ellipse"; } }

        public EllipseToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.TopLeft:
                    {
                        _ellipse = new EllipseShape(
                            new PointShape(x, y, context.PointShape),
                            new PointShape(x, y, context.PointShape));
                        _ellipse.Style = context.CurrentStyle;
                        context.WorkingContainer.Shapes.Add(_ellipse);
                        context.Selected.Add(_ellipse.TopLeft);
                        context.Selected.Add(_ellipse.BottomRight);
                        _state = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        _state = State.TopLeft;
                        _ellipse.BottomRight.X = x;
                        _ellipse.BottomRight.Y = y;
                        context.WorkingContainer.Shapes.Remove(_ellipse);
                        context.Selected.Remove(_ellipse.TopLeft);
                        context.Selected.Remove(_ellipse.BottomRight);
                        context.CurrentContainer.Shapes.Add(_ellipse);
                        _ellipse = null;
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
                        _ellipse.BottomRight.X = x;
                        _ellipse.BottomRight.Y = y;
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            _state = State.TopLeft;

            Filters.ForEach(f => f.Clear(context));

            if (_ellipse != null)
            {
                context.WorkingContainer.Shapes.Remove(_ellipse);
                context.Selected.Remove(_ellipse.TopLeft);
                context.Selected.Remove(_ellipse.BottomRight);
                _ellipse = null;
            }
        }
    }
}
