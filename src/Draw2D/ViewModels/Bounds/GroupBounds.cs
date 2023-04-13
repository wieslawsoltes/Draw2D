﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds;

[DataContract(IsReference = true)]
public class GroupBounds : ViewModelBase, IBounds
{
    public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is GroupShape group))
        {
            throw new ArgumentNullException("shape");
        }

        if (modifier.HasFlag(Modifier.Shift))
        {
            foreach (var groupShape in group.Shapes.Reverse())
            {
                var result = groupShape.Bounds?.TryToGetPoint(groupShape, target, radius, hitTest, modifier);
                if (result != null)
                {
                    return result;
                }
            }
        }
        else
        {
            foreach (var groupPoint in group.Points)
            {
                if (groupPoint.Bounds?.TryToGetPoint(groupPoint, target, radius, hitTest, modifier) != null)
                {
                    return groupPoint;
                }
            }
        }

        return null;
    }

    public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is GroupShape group))
        {
            throw new ArgumentNullException("shape");
        }

        foreach (var groupShape in group.Shapes.Reverse())
        {
            var result = groupShape.Bounds?.Contains(groupShape, target, radius, hitTest, modifier);
            if (result != null)
            {
                if (modifier.HasFlag(Modifier.Shift))
                {
                    return result;
                }
                else
                {
                    return group;
                }
            }
        }

        var points = new List<IPointShape>();
        group.GetPoints(points);

        if (points.Count == 0)
        {
            return null;
        }

        return HitTestHelper.Contains(points, target) ? shape : null;
    }

    public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier)
    {
        if (!(shape is GroupShape group))
        {
            throw new ArgumentNullException("shape");
        }

        foreach (var groupShape in group.Shapes)
        {
            var result = groupShape.Bounds?.Overlaps(groupShape, target, radius, hitTest, modifier);
            if (result != null)
            {
                return group;
            }
        }
        return null;
    }
}