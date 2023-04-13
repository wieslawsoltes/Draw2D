using System;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds;

[DataContract(IsReference = true)]
public class LineBounds : ViewModelBase, IBounds
{
    public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is LineShape line))
        {
            throw new ArgumentNullException("shape");
        }

        if (line.StartPoint.Bounds?.TryToGetPoint(line.StartPoint, target, radius, hitTest, modifier) != null)
        {
            return line.StartPoint;
        }

        if (line.Point.Bounds?.TryToGetPoint(line.Point, target, radius, hitTest, modifier) != null)
        {
            return line.Point;
        }

        foreach (var point in line.Points)
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
        if (!(shape is LineShape line))
        {
            throw new ArgumentNullException("shape");
        }

        var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
        var b = new Point2(line.Point.X, line.Point.Y);
        var nearest = target.NearestOnLine(a, b);
        double distance = target.DistanceTo(nearest);
        return distance < radius ? shape : null;
    }

    public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is LineShape line))
        {
            throw new ArgumentNullException("shape");
        }

        var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
        var b = new Point2(line.Point.X, line.Point.Y);
        return Line2.LineIntersectsWithRect(a, b, target, out _, out _, out _, out _) ? shape : null;
    }
}