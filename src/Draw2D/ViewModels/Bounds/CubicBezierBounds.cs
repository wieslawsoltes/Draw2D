using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class CubicBezierBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            if (cubicBezier.StartPoint.Bounds?.TryToGetPoint(cubicBezier.StartPoint, target, radius, hitTest, modifier) != null)
            {
                return cubicBezier.StartPoint;
            }

            if (cubicBezier.Point1.Bounds?.TryToGetPoint(cubicBezier.Point1, target, radius, hitTest, modifier) != null)
            {
                return cubicBezier.Point1;
            }

            if (cubicBezier.Point2.Bounds?.TryToGetPoint(cubicBezier.Point2, target, radius, hitTest, modifier) != null)
            {
                return cubicBezier.Point2;
            }

            if (cubicBezier.Point3.Bounds?.TryToGetPoint(cubicBezier.Point3, target, radius, hitTest, modifier) != null)
            {
                return cubicBezier.Point3;
            }

            foreach (var point in cubicBezier.Points)
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
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            cubicBezier.GetPoints(points);

            if (points.Count == 0)
            {
                return null;
            }

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            cubicBezier.GetPoints(points);

            if (points.Count == 0)
            {
                return null;
            }

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }
}
