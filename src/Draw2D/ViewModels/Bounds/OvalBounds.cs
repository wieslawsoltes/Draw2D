using System;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class OvalBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var oval = shape as OvalShape ?? throw new ArgumentNullException("shape");

            if (oval.StartPoint.Bounds?.TryToGetPoint(oval.StartPoint, target, radius, hitTest, modifier) != null)
            {
                return oval.StartPoint;
            }

            if (oval.Point.Bounds?.TryToGetPoint(oval.Point, target, radius, hitTest, modifier) != null)
            {
                return oval.Point;
            }

            foreach (var point in oval.Points)
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
            var oval = shape as OvalShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                oval.StartPoint.X,
                oval.StartPoint.Y,
                oval.Point.X,
                oval.Point.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var oval = shape as OvalShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                oval.StartPoint.X,
                oval.StartPoint.Y,
                oval.Point.X,
                oval.Point.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
