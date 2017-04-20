// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Spatial;

namespace Draw2D.Editor.Bounds
{
    public interface IHitTest
    {
        IDictionary<Type, HitTestBase> Registered { get; }
        void Register(HitTestBase hitTest);
        PointShape TryToGetPoint(IEnumerable<ShapeObject> shapes, Point2 target, double radius, PointShape exclude);
        PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius);
        ShapeObject TryToGetShape(IEnumerable<ShapeObject> shapes, Point2 target, double radius);
        ISet<ShapeObject> TryToGetShapes(IEnumerable<ShapeObject> shapes, Rect2 target, double radius);
    }
}
