// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class PointHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(PointShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var point = shape as PointShape;
            if (point == null)
                throw new ArgumentNullException("shape");

            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }

            return null;
        }

        public override ShapeObject Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var point = shape as PointShape;
            if (point == null)
                throw new ArgumentNullException("shape");

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y) ? shape : null;
        }

        public override ShapeObject Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var point = shape as PointShape;
            if (point == null)
                throw new ArgumentNullException("shape");

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target) ? shape : null;
        }
    }
}
