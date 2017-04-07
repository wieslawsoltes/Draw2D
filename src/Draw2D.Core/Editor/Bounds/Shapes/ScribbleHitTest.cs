// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Bounds.Shapes
{
    public class ScribbleHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(ScribbleShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            foreach (var point in scribble.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public override ShapeObject Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];
            var points = scribble.GetPoints();

            foreach (var point in points)
            {
                var result = pointHitTest.Contains(point, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public override ShapeObject Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];
            var points = scribble.GetPoints();

            foreach (var point in points)
            {
                var pointResult = pointHitTest.Overlaps(point, target, radius, hitTest);
                if (pointResult != null)
                {
                    return pointResult;
                }
            }

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }
}
