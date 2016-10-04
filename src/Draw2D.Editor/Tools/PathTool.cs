using System;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class PathTool : ToolBase
    {
        public override string Name { get { return "Path"; } }

        public PathToolSettings Settings { get; set; }
    }
}
