﻿using System;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds;

[DataContract(IsReference = true)]
public class RectangleBounds : ViewModelBase, IBounds
{
    public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        var rectangle = shape as RectangleShape ?? throw new ArgumentNullException("shape");

        if (rectangle.StartPoint.Bounds?.TryToGetPoint(rectangle.StartPoint, target, radius, hitTest, modifier) != null)
        {
            return rectangle.StartPoint;
        }

        if (rectangle.Point.Bounds?.TryToGetPoint(rectangle.Point, target, radius, hitTest, modifier) != null)
        {
            return rectangle.Point;
        }

        foreach (var point in rectangle.Points)
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
        var rectangle = shape as RectangleShape ?? throw new ArgumentNullException("shape");

        return Rect2.FromPoints(
            rectangle.StartPoint.X,
            rectangle.StartPoint.Y,
            rectangle.Point.X,
            rectangle.Point.Y).Contains(target) ? shape : null;
    }

    public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        var rectangle = shape as RectangleShape ?? throw new ArgumentNullException("shape");

        return Rect2.FromPoints(
            rectangle.StartPoint.X,
            rectangle.StartPoint.Y,
            rectangle.Point.X,
            rectangle.Point.Y).IntersectsWith(target) ? shape : null;
    }
}