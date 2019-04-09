// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    public interface IBounds
    {
        Type TargetType { get; }
        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest);
        BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest);
        BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest);
    }
}
