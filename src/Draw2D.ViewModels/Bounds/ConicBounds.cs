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
    public class ConicBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            if (conic.StartPoint.Bounds?.TryToGetPoint(conic.StartPoint, target, radius, hitTest, modifier) != null)
            {
                return conic.StartPoint;
            }

            if (conic.Point1.Bounds?.TryToGetPoint(conic.Point1, target, radius, hitTest, modifier) != null)
            {
                return conic.Point1;
            }

            if (conic.Point2.Bounds?.TryToGetPoint(conic.Point2, target, radius, hitTest, modifier) != null)
            {
                return conic.Point2;
            }

            foreach (var point in conic.Points)
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
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            conic.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            conic.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }
}
