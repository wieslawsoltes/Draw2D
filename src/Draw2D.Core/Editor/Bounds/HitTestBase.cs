// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds
{
    public abstract class HitTestBase
    {
        public abstract Type TargetType { get; }
        public abstract PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered);
        public abstract bool Contains(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered);
        public abstract bool Overlaps(ShapeObject shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered);
    }
}
