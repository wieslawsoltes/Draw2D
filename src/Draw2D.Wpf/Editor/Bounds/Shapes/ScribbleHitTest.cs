using System;
using System.Collections.Generic;
using Draw2D.Models;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class ScribbleHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(ScribbleShape); } }

        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(scribble.Start, target, radius, registered) != null)
            {
                return scribble.Start;
            }

            foreach (var point in scribble.Points)
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
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.Contains(scribble.Start, target, radius, registered))
            {
                return true;
            }

            foreach (var point in scribble.Points)
            {
                if (pointHitTest.Contains(point, target, radius, registered))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var scribble = shape as ScribbleShape;
            if (scribble == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.Overlaps(scribble.Start, target, radius, registered))
            {
                return true;
            }

            foreach (var point in scribble.Points)
            {
                if (pointHitTest.Overlaps(point, target, radius, registered))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
