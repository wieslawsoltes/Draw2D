// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class ContainerBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerPoint in container.Points)
            {
                if (containerPoint.Bounds?.TryToGetPoint(containerPoint, target, radius, hitTest, modifier) != null)
                {
                    return containerPoint;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.Contains(containerShape, target, radius, hitTest, modifier);
                if (result != null)
                {
                    return container;
                }
            }

            var points = new List<IPointShape>();
            container.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.Overlaps(containerShape, target, radius, hitTest, modifier);
                if (result != null)
                {
                    return container;
                }
            }
            return null;
        }
    }
}
