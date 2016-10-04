using System;
using System.Collections.Generic;
using Draw2D.Models;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class GroupHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(GroupShape); } }
        
        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];
            
            foreach (var groupPoint in group.Points)
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, registered) != null)
                {
                    return groupPoint;
                }
            }

            //foreach (var groupSegment in group.Segments)
            //{
            //    var hitTest = registered[groupSegment.GetType()];
            //    var result = hitTest.TryToGetPoint(groupSegment, target, radius, registered);
            //    if (result != null)
            //    {
            //        return result;
            //    }
            //}

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException("shape");

            foreach (var groupShape in group.Segments)
            {
                var hitTest = registered[groupShape.GetType()];
                var result = hitTest.Contains(groupShape, target, radius, registered);
                if (result == true)
                {
                    return true;
                }
            }
            return false;
        }
        
        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException("shape");

            foreach (var groupShape in group.Segments)
            {
                var hitTest = registered[groupShape.GetType()];
                var result = hitTest.Overlaps(groupShape, target, radius, registered);
                if (result == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
