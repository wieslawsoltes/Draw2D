using System;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class QuadraticBezierTool : ToolBase
    {
        public override string Name { get { return "QuadraticBezier"; } }

        public QuadraticBezierToolSettings Settings { get; set; }
    }
}
