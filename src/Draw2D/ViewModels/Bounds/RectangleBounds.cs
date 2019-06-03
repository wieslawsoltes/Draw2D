// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class RectangleBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var rectangle = shape as RectangleShape ?? throw new ArgumentNullException("shape");

            if (rectangle.TopLeft.Bounds?.TryToGetPoint(rectangle.TopLeft, target, radius, hitTest) != null)
            {
                return rectangle.TopLeft;
            }

            if (rectangle.BottomRight.Bounds?.TryToGetPoint(rectangle.BottomRight, target, radius, hitTest) != null)
            {
                return rectangle.BottomRight;
            }

            foreach (var point in rectangle.Points)
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
            var rectangle = shape as RectangleShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var rectangle = shape as RectangleShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
