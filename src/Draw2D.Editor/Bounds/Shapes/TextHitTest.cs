// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class TextHitTest : HitTestBase
    {
        public override Type TargetType => typeof(TextShape);

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var text = shape as TextShape;
            if (text == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(text.TopLeft, target, radius, hitTest) != null)
            {
                return text.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(text.BottomRight, target, radius, hitTest) != null)
            {
                return text.BottomRight;
            }

            foreach (var point in text.Points)
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
            var text = shape as TextShape;
            if (text == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y).Contains(target) ? shape : null;
        }

        public override ShapeObject Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var text = shape as TextShape;
            if (text == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
