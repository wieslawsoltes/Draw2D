﻿using System;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds;

[DataContract(IsReference = true)]
public class TextBounds : ViewModelBase, IBounds
{
    public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        var text = shape as TextShape ?? throw new ArgumentNullException("shape");

        if (text.StartPoint.Bounds?.TryToGetPoint(text.StartPoint, target, radius, hitTest, modifier) != null)
        {
            return text.StartPoint;
        }

        if (text.Point.Bounds?.TryToGetPoint(text.Point, target, radius, hitTest, modifier) != null)
        {
            return text.Point;
        }

        foreach (var point in text.Points)
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
        var text = shape as TextShape ?? throw new ArgumentNullException("shape");

        return Rect2.FromPoints(
            text.StartPoint.X,
            text.StartPoint.Y,
            text.Point.X,
            text.Point.Y).Contains(target) ? shape : null;
    }

    public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        var text = shape as TextShape ?? throw new ArgumentNullException("shape");

        return Rect2.FromPoints(
            text.StartPoint.X,
            text.StartPoint.Y,
            text.Point.X,
            text.Point.Y).IntersectsWith(target) ? shape : null;
    }
}