// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class ImageBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var image = shape as ImageShape ?? throw new ArgumentNullException("shape");

            if (image.StartPoint.Bounds?.TryToGetPoint(image.StartPoint, target, radius, hitTest) != null)
            {
                return image.StartPoint;
            }

            if (image.Point.Bounds?.TryToGetPoint(image.Point, target, radius, hitTest) != null)
            {
                return image.Point;
            }

            foreach (var point in image.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var image = shape as ImageShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                image.StartPoint.X,
                image.StartPoint.Y,
                image.Point.X,
                image.Point.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var image = shape as ImageShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                image.StartPoint.X,
                image.StartPoint.Y,
                image.Point.X,
                image.Point.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
