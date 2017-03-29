using System;
using System.Windows;

namespace PathDemo.Tools
{
    public class MoveTool : ToolBase
    {
        private readonly PathTool _pathTool;

        public MoveTool(PathTool pathTool)
        {
            _pathTool = pathTool;
        }

        public override void LeftDown(IToolContext context, Point point)
        {
            _pathTool.NewFigure();
        }
    }
}
