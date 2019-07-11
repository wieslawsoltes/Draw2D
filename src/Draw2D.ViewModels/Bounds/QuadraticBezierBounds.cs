// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            if (quadraticBezier.StartPoint.Bounds?.TryToGetPoint(quadraticBezier.StartPoint, target, radius, hitTest, modifier) != null)
            {
                return quadraticBezier.StartPoint;
            }

            if (quadraticBezier.Point1.Bounds?.TryToGetPoint(quadraticBezier.Point1, target, radius, hitTest, modifier) != null)
            {
                return quadraticBezier.Point1;
            }

            if (quadraticBezier.Point2.Bounds?.TryToGetPoint(quadraticBezier.Point2, target, radius, hitTest, modifier) != null)
            {
                return quadraticBezier.Point2;
            }

            foreach (var point in quadraticBezier.Points)
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
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            quadraticBezier.GetPoints(points);

            if (points.Count == 0)
            {
                return null;
            }

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            quadraticBezier.GetPoints(points);

            if (points.Count == 0)
            {
                return null;
            }

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }
}
