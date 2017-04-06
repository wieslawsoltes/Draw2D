// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Bounds.Shapes
{
    public class CubicBezierHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(CubicBezierShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var cubicBezier = shape as CubicBezierShape;
            if (cubicBezier == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(cubicBezier.StartPoint, target, radius, hitTest) != null)
            {
                return cubicBezier.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point1, target, radius, hitTest) != null)
            {
                return cubicBezier.Point1;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point2, target, radius, hitTest) != null)
            {
                return cubicBezier.Point2;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point3, target, radius, hitTest) != null)
            {
                return cubicBezier.Point3;
            }

            foreach (var point in cubicBezier.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        private IEnumerable<PointShape> GetPoints(CubicBezierShape cubicBezier)
        {
            yield return cubicBezier.StartPoint;
            yield return cubicBezier.Point1;
            yield return cubicBezier.Point2;
            yield return cubicBezier.Point3;
        }

        public override bool Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var cubicBezier = shape as CubicBezierShape;
            if (cubicBezier == null)
                throw new ArgumentNullException("shape");

            return HitTestHelper.Contains(GetPoints(cubicBezier), target);
        }

        public override bool Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var cubicBezier = shape as CubicBezierShape;
            if (cubicBezier == null)
                throw new ArgumentNullException("shape");

            return HitTestHelper.Overlap(GetPoints(cubicBezier), target);
        }
    }
}
