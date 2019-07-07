// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using SkiaSharp;

namespace Draw2D.Bounds
{
    public readonly struct ChildNode : IDisposable
    {
        public readonly IBaseShape Shape;
        public readonly string StyleId;
        public readonly double dX;
        public readonly double dY;
        public readonly double Scale;
        public readonly SKPath Geometry;
        public readonly SKRect Bounds;

        public ChildNode(IBaseShape shape, string styleId, double dx, double dy, double scale, SKPath geometry)
        {
            this.Shape = shape;
            this.StyleId = styleId;
            this.dX = dx;
            this.dY = dy;
            this.Scale = scale;
            this.Geometry = geometry;
            geometry.GetBounds(out this.Bounds);
        }

        public bool Contains(float x, float y, ContainsMode mode)
        {
            if (mode == ContainsMode.Geometry)
            {
                return Geometry.Contains(x, y);
            }
            else if (mode == ContainsMode.Bounds)
            {
                return Bounds.Contains(x, y);
            }
            return false;
        }

        public bool Intersects(SKPath geometry)
        {
            using (var result = this.Geometry.Op(geometry, SKPathOp.Intersect))
            {
                return result?.IsEmpty == false;
            }
        }

        public bool Intersects(ref SKRect rect)
        {
            return this.Bounds.IntersectsWith(rect);
        }

        public void Dispose()
        {
            Geometry?.Dispose();
        }
    }
}
