using System;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds;

[DataContract(IsReference = true)]
public class ReferenceBounds : ViewModelBase, IBounds
{
    public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is ReferenceShape reference))
        {
            throw new ArgumentNullException("shape");
        }

        foreach (var point in reference.Points)
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
        if (!(shape is ReferenceShape reference))
        {
            throw new ArgumentNullException("shape");
        }

        if (reference.Template?.Bounds != null)
        {
            var adjustedTarget = new Point2(target.X - reference.X, target.Y - reference.Y);
            var result = reference.Template.Bounds.Contains(reference.Template, adjustedTarget, radius, hitTest, modifier);
            if (result != null)
            {
                return reference;
            }
        }

        return null;
    }

    public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is ReferenceShape reference))
        {
            throw new ArgumentNullException("shape");
        }

        if (reference.Template?.Bounds != null)
        {
            var adjustedTarget = new Rect2(target.X - reference.X, target.Y - reference.Y, target.Width, target.Height);
            var result = reference.Template.Bounds.Overlaps(reference.Template, adjustedTarget, radius, hitTest, modifier);
            if (result != null)
            {
                return reference;
            }
        }

        return null;
    }
}