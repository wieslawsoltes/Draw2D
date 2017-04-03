// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Core.Editor.Tools
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
