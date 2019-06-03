// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class TextBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var text = shape as TextShape ?? throw new ArgumentNullException("shape");

            if (text.TopLeft.Bounds?.TryToGetPoint(text.TopLeft, target, radius, hitTest) != null)
            {
                return text.TopLeft;
            }

            if (text.BottomRight.Bounds?.TryToGetPoint(text.BottomRight, target, radius, hitTest) != null)
            {
                return text.BottomRight;
            }

            foreach (var point in text.Points)
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
            var text = shape as TextShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var text = shape as TextShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
