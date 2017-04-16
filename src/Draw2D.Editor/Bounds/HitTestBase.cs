// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Bounds
{
    public abstract class HitTestBase
    {
        public abstract Type TargetType { get; }
        public abstract PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest);
        public abstract ShapeObject Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest);
        public abstract ShapeObject Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest);
    }
}
