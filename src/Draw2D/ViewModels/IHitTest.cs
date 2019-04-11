// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels
{
    public interface IHitTest
    {
        Dictionary<Type, IBounds> Registered { get; set; }
        void Register(IBounds hitTest);
        PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, PointShape exclude);
        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius);
        BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius);
        ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius);
    }
}
