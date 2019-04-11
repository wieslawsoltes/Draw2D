// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Draw2D.ViewModels.Tools
{
    public class NoneToolSettings : SettingsBase
    {
    }

    public class NoneTool : ViewModelBase, ITool
    {
        public string Title => "None";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public NoneToolSettings Settings { get; set; }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
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
        }

        public void Clean(IToolContext context)
        {
        }
    }
}
