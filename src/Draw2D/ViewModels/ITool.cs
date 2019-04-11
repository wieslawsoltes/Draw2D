// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels
{
    public interface ITool
    {
        string Title { get; }
        IList<PointIntersectionBase> Intersections { get; set; }
        IList<PointFilterBase> Filters { get; set; }
        void LeftDown(IToolContext context, double x, double y, Modifier modifier);
        void LeftUp(IToolContext context, double x, double y, Modifier modifier);
        void RightDown(IToolContext context, double x, double y, Modifier modifier);
        void RightUp(IToolContext context, double x, double y, Modifier modifier);
        void Move(IToolContext context, double x, double y, Modifier modifier);
        void Clean(IToolContext context);
    }
}
