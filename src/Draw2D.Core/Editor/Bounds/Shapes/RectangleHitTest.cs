using System;
using System.Collections.Generic;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class RectangleHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(RectangleShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var rectangle = shape as RectangleShape;
            if (rectangle == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(rectangle.TopLeft, target, radius, registered) != null)
            {
                return rectangle.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(rectangle.BottomRight, target, radius, registered) != null)
            {
                return rectangle.BottomRight;
            }

            foreach (var point in rectangle.Points)
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
            var rectangle = shape as RectangleShape;
            if (rectangle == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).Contains(target);
        }

        public override bool Overlaps(ShapeObject shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var rectangle = shape as RectangleShape;
            if (rectangle == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).IntersectsWith(target);
        }
    }
}
