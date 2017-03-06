using System;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class CubicBezierTool : ToolBase
    {
        public override string Name { get { return "CubicBezier"; } }

        public CubicBezierToolSettings Settings { get; set; }
    }
}
