// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.Editor
{
    public abstract class ToolBase : ObservableObject
    {
        public abstract string Title { get; }

        public List<PointIntersection> Intersections { get; set; }

        public List<PointFilter> Filters { get; set; }

        public virtual void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public virtual void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public virtual void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public virtual void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public virtual void Move(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public virtual void Clean(IToolContext context)
        {
        }
    }
}
