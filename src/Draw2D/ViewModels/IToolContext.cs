// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Renderer;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Bounds;

namespace Draw2D.ViewModels
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
