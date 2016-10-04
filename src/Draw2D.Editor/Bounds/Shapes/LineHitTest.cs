using System;
using System.Collections.Generic;
using Draw2D.Models;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class LineHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(LineShape); } }

        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(line.StartPoint, target, radius, registered) != null)
            {
                return line.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(line.Point, target, radius, registered) != null)
            {
                return line.Point;
            }

            foreach (var point in line.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, registered) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            var nearest = target.NearestOnLine(a, b);
            double distance = target.DistanceTo(nearest);
            return distance < radius;
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            double x0clip;
            double y0clip;
            double x1clip;
            double y1clip;
            return Line2.LineIntersectsWithRect(a, b, target, out x0clip, out y0clip, out x1clip, out y1clip);
        }
    }
}
