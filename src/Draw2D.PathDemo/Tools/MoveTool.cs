using Draw2D.Editor;

namespace Draw2D.PathDemo.Tools
{
    public class MoveTool : ToolBase
    {
        public override string Name { get { return "Move"; } }

        private readonly PathTool _pathTool;

        public MoveTool(PathTool pathTool)
        {
            _pathTool = pathTool;
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            _pathTool.NewFigure();
        }
    }
}
