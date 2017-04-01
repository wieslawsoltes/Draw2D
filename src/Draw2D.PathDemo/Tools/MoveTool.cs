using Draw2D.Editor;
using Draw2D.Editor.Tools;

namespace Draw2D.PathDemo.Tools
{
    public class MoveTool : ToolBase
    {
        private readonly PathTool _pathTool;

        public override string Name { get { return "Move"; } }

        public MoveToolSettings Settings { get; set; }

        public MoveTool(PathTool pathTool)
        {
            _pathTool = pathTool;
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            _pathTool.Move();
        }
    }
}
