// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class FigureBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is FigureShape figure))
            {
                throw new ArgumentNullException("shape");
            }

            if (modifier.HasFlag(Modifier.Shift))
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var result = figureShape.Bounds?.TryToGetPoint(figureShape, target, radius, hitTest, modifier);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else
            {
                foreach (var figurePoint in figure.Points)
                {
                    if (figurePoint.Bounds?.TryToGetPoint(figurePoint, target, radius, hitTest, modifier) != null)
                    {
                        return figurePoint;
                    }
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is FigureShape figure))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figureShape in figure.Shapes)
            {
                var result = figureShape.Bounds?.Contains(figureShape, target, radius, hitTest, modifier);
                if (result != null)
                {
                    if (modifier.HasFlag(Modifier.Shift))
                    {
                        return result;
                    }
                    else
                    {
                        return figure;
                    }
                }
            }
            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
        {
            if (!(shape is FigureShape figure))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figureShape in figure.Shapes)
            {
                var result = figureShape.Bounds?.Overlaps(figureShape, target, radius, hitTest, modifier);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
