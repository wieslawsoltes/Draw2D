using System;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class ArcBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var arc = shape as ArcShape ?? throw new ArgumentNullException("shape");

            if (arc.StartPoint.Bounds?.TryToGetPoint(arc.StartPoint, target, radius, hitTest, modifier) != null)
            {
                return arc.StartPoint;
            }

            if (arc.Point.Bounds?.TryToGetPoint(arc.Point, target, radius, hitTest, modifier) != null)
            {
                return arc.Point;
            }

            foreach (var point in arc.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest, modifier) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var arc = shape as ArcShape ?? throw new ArgumentNullException("shape");
            var distance = arc.StartPoint.DistanceTo(arc.Point);

            return arc.StartPoint.ToRect2(distance).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var arc = shape as ArcShape ?? throw new ArgumentNullException("shape");
            var distance = arc.StartPoint.DistanceTo(arc.Point);

            return arc.StartPoint.ToRect2(distance).IntersectsWith(target) ? shape : null;
        }
    }
}
