// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Draw2D.ViewModels.Tools
{
    public class MoveToolSettings : SettingsBase
    {
    }

    public class MoveTool : ViewModelBase, ITool
    {
        public PathTool PathTool { get; set; }

        public string Title => "Move";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public MoveToolSettings Settings { get; set; }

        public MoveTool(PathTool pathTool)
        {
            PathTool = pathTool;
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            PathTool.Move();
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            context.Invalidate?.Invoke();
        }

        public void Clean(IToolContext context)
        {
        }
    }
}
