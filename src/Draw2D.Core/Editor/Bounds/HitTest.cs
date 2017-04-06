// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Core.Editor.Bounds
{
    public class HitTest
    {
        public IDictionary<Type, HitTestBase> Registered { get; set; }

        public HitTest()
        {
            Registered = new Dictionary<Type, HitTestBase>();
        }

        public void Register(HitTestBase hitTest)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }

        private HitTestBase GetHitTest(object target)
        {
            return Registered.TryGetValue(target?.GetType(), out var hitTest) ? hitTest : null;
        }

        public PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius)
        {

            return GetHitTest(shape)?.TryToGetPoint(shape, target, radius, Registered);
        }

        public PointShape TryToGetPoint(IEnumerable<ShapeObject> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes)
            {
                var result = TryToGetPoint(shape, target, radius);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public ShapeObject TryToGetShape(IEnumerable<ShapeObject> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes)
            {
                var result = GetHitTest(shape)?.Contains(shape, target, radius, Registered);
                if (result == true)
                {
                    return shape;
                }
            }
            return null;
        }

        public HashSet<ShapeObject> TryToGetShapes(IEnumerable<ShapeObject> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<ShapeObject>();
            foreach (var shape in shapes)
            {
                var result = GetHitTest(shape)?.Overlaps(shape, target, radius, Registered);
                if (result == true)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
