﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
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

            if (ellipse.StartPoint.Bounds?.TryToGetPoint(ellipse.StartPoint, target, radius, hitTest) != null)
            {
                return ellipse.StartPoint;
            }

            if (ellipse.Point.Bounds?.TryToGetPoint(ellipse.Point, target, radius, hitTest) != null)
            {
                return ellipse.Point;
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
                ellipse.StartPoint.X,
                ellipse.StartPoint.Y,
                ellipse.Point.X,
                ellipse.Point.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var ellipse = shape as EllipseShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                ellipse.StartPoint.X,
                ellipse.StartPoint.Y,
                ellipse.Point.X,
                ellipse.Point.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
