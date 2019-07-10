// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class CircleBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var circle = shape as CircleShape ?? throw new ArgumentNullException("shape");

            if (circle.StartPoint.Bounds?.TryToGetPoint(circle.StartPoint, target, radius, hitTest, modifier) != null)
            {
                return circle.StartPoint;
            }

            if (circle.Point.Bounds?.TryToGetPoint(circle.Point, target, radius, hitTest, modifier) != null)
            {
                return circle.Point;
            }

            foreach (var point in circle.Points)
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
            var circle = shape as CircleShape ?? throw new ArgumentNullException("shape");
            var distance = circle.StartPoint.DistanceTo(circle.Point);

            return Rect2.FromPoints(
                circle.StartPoint.X - distance,
                circle.StartPoint.Y - distance,
                circle.StartPoint.X + distance,
                circle.StartPoint.Y + distance).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            var circle = shape as CircleShape ?? throw new ArgumentNullException("shape");
            var distance = circle.StartPoint.DistanceTo(circle.Point);

            return Rect2.FromPoints(
                circle.StartPoint.X - distance,
                circle.StartPoint.Y - distance,
                circle.StartPoint.X + distance,
                circle.StartPoint.Y + distance).IntersectsWith(target) ? shape : null;
        }
    }
}
