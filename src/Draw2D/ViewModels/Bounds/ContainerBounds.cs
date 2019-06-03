// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Containers;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class ContainerBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerPoint in container.Points)
            {
                if (containerPoint.Bounds?.TryToGetPoint(containerPoint, target, radius, hitTest) != null)
                {
                    return containerPoint;
                }
            }
#if USE_CONTAINER_SHAPES
            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.TryToGetPoint(containerShape, target, radius, hitTest);
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
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.Contains(containerShape, target, radius, hitTest);
                if (result != null)
                {
                    return container;
                }
            }
            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.Overlaps(containerShape, target, radius, hitTest);
                if (result != null)
                {
                    return container;
                }
            }
            return null;
        }
    }
}
