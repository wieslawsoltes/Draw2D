// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Containers;
using Draw2D.Renderer;
using Draw2D.Shapes;
using Draw2D.Style;
using Draw2D.Editor.Bounds;

namespace Draw2D.Editor
{
    public interface IToolContext
    {
        IShapeRenderer Renderer { get; set; }
        IHitTest HitTest { get; set; }
        CanvasContainer CurrentContainer { get; set; }
        CanvasContainer WorkingContainer { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointShape { get; set; }
        Action Capture { get; set; }
        Action Release { get; set; }
        Action Invalidate { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
    }
}
