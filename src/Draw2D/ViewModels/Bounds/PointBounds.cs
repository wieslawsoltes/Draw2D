// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.Input;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class PointBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is IPointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is IPointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is IPointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target) ? shape : null;
        }
    }
}
