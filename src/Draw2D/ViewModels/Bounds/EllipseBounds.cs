// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class EllipseBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var ellipse = shape as EllipseShape ?? throw new ArgumentNullException("shape");

            if (ellipse.TopLeft.Bounds?.TryToGetPoint(ellipse.TopLeft, target, radius, hitTest) != null)
            {
                return ellipse.TopLeft;
            }

            if (ellipse.BottomRight.Bounds?.TryToGetPoint(ellipse.BottomRight, target, radius, hitTest) != null)
            {
                return ellipse.BottomRight;
            }

            foreach (var point in ellipse.Points)
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
            var ellipse = shape as EllipseShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var ellipse = shape as EllipseShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
