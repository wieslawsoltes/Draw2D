using System.Linq;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public class PointTool : ToolBase
    {
        public override string Name { get { return "Point"; } }

        public PointToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));

            var point = new PointShape(x, y, context.PointShape);

            var shape = context.HitTest.TryToGetShape(context.CurrentContainer.Shapes, new Point2(x, y), Settings.HitTestRadius);
            if (shape != null && Settings.ConnectPoints)
            {
                if (shape is ConnectableShape connectable)
                {
                    connectable.Points.Add(point);
                }
            }
            else
            {
                context.CurrentContainer.Shapes.Add(point);
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            Filters.ForEach(f => f.Clear(context));
        }
    }
}
