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

            if (pointHitTest.TryToGetPoint(scribble.Start, target, radius, hitTest) != null)
            {
                return scribble.Start;
            }

            foreach (var point in scribble.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public override bool Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.Contains(scribble.Start, target, radius, hitTest))
            {
                return true;
            }

            foreach (var point in scribble.Points)
            {
                if (pointHitTest.Contains(point, target, radius, hitTest))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.Overlaps(scribble.Start, target, radius, hitTest))
            {
                return true;
            }

            foreach (var point in scribble.Points)
            {
                if (pointHitTest.Overlaps(point, target, radius, hitTest))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
