// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Spatial;

namespace Draw2D.ViewModels
{
    public interface IHitTest
    {
        IPointShape TryToGetPoint(IEnumerable<IBaseShape> shapes, Point2 target, double radius, IPointShape exclude);
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius);
        IBaseShape TryToGetShape(IEnumerable<IBaseShape> shapes, Point2 target, double radius);
        ISet<IBaseShape> TryToGetShapes(IEnumerable<IBaseShape> shapes, Rect2 target, double radius);
    }
}
