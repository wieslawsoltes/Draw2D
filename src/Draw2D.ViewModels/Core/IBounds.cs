// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Spatial;

namespace Draw2D.ViewModels
{
    public interface IBounds
    {
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest);
        IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest);
        IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest);
    }
}
