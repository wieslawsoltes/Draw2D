// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class GroupBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupPoint in group.Points)
            {
                if (groupPoint.Bounds?.TryToGetPoint(groupPoint, target, radius, hitTest) != null)
                {
                    return groupPoint;
                }
            }
#if USE_GROUP_SHAPES
            foreach (var groupShape in group.Shapes)
            {
                var result = groupShape.Bounds?.TryToGetPoint(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }
#endif
            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupShape in group.Shapes)
            {
                var result = groupShape.Bounds?.Contains(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupShape in group.Shapes)
            {
                var result = groupShape.Bounds?.Overlaps(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }
    }
}
