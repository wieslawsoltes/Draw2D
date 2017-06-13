// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Containers;
using Draw2D.Core.Renderer;
using Draw2D.Core.Shape;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;
using Draw2D.Editor.Bounds;

namespace Draw2D.Editor
{
    public interface IToolContext
    {
        ShapeRenderer Renderer { get; set; }
        ISet<BaseShape> Selected { get; set; }
        IHitTest HitTest { get; set; }
        IShapeContainer CurrentContainer { get; set; }
        IShapeContainer WorkingContainer { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointShape { get; set; }
        Action Capture { get; set; }
        Action Release { get; set; }
        Action Invalidate { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
    }
}
