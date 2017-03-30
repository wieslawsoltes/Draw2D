namespace Draw2D.Editor.Tools
{
    public class MoveTool : ToolBase
    {
        public override string Name { get { return "Move"; } }

        public MoveToolSettings Settings { get; set; }
    }
}
