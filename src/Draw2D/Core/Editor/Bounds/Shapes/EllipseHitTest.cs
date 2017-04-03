// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Bounds.Shapes
{
    public class EllipseHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(EllipseShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var ellipse = shape as EllipseShape;
            if (ellipse == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(ellipse.TopLeft, target, radius, registered) != null)
            {
                return ellipse.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(ellipse.BottomRight, target, radius, registered) != null)
            {
                return ellipse.BottomRight;
            }

            foreach (var point in ellipse.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, registered) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public override bool Contains(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var ellipse = shape as EllipseShape;
            if (ellipse == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y).Contains(target);
        }

        public override bool Overlaps(ShapeObject shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var ellipse = shape as EllipseShape;
            if (ellipse == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y).IntersectsWith(target);
        }
    }
}
