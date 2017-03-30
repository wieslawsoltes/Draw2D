using System.Linq;

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

            // TODO: Implement PointTool.LeftDown().
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));

            // TODO: Implement PointTool.Move().
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            Filters.ForEach(f => f.Clear(context));

            // TODO: Implement PointTool.Clean().
        }
    }
}
