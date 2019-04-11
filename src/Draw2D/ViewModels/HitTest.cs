// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.ViewModels.Shapes;
using Spatial;
using Spatial.ConvexHull;
using Spatial.Sat;

namespace Draw2D.ViewModels
{
    public static class HitTestHelper
    {
        public static MonotoneChain MC => new MonotoneChain();
        public static SeparatingAxisTheorem SAT => new SeparatingAxisTheorem();

        public static Vector2[] ToSelection(Rect2 rect)
        {
            return new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };
        }

        public static void ToConvexHull(IEnumerable<PointShape> points, out int k, out Vector2[] convexHull)
        {
            Vector2[] vertices = new Vector2[points.Count()];
            int i = 0;
            foreach (var point in points)
            {
                vertices[i] = new Vector2(point.X, point.Y);
                i++;
            }
            MC.ConvexHull(vertices, out convexHull, out k);
        }

        public static bool Contains(IEnumerable<PointShape> points, Point2 point)
        {
            ToConvexHull(points, out int k, out Vector2[] convexHull);
            bool contains = false;
            for (int i = 0, j = k - 2; i < k - 1; j = i++)
            {
                if (((convexHull[i].Y > point.Y) != (convexHull[j].Y > point.Y))
                    && (point.X < (convexHull[j].X - convexHull[i].X) * (point.Y - convexHull[i].Y) / (convexHull[j].Y - convexHull[i].Y) + convexHull[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        public static bool Overlap(IEnumerable<PointShape> points, Vector2[] selection)
        {
            ToConvexHull(points, out int k, out Vector2[] convexHull);
            Vector2[] vertices = convexHull.Take(k).ToArray();
            return SAT.Overlap(selection, vertices);
        }

        public static bool Overlap(IEnumerable<PointShape> points, Rect2 rect)
        {
            return Overlap(points, ToSelection(rect));
        }
    }

    public class HitTest : IHitTest
    {
        public Dictionary<Type, IBounds> Registered { get; set; }

        public HitTest()
        {
            Registered = new Dictionary<Type, IBounds>();
        }

        public void Register(IBounds hitTest)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }

        private IBounds GetHitTest(object target)
        {
            return Registered.TryGetValue(target?.GetType(), out var hitTest) ? hitTest : null;
        }

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius)
        {
            return GetHitTest(shape)?.TryToGetPoint(shape, target, radius, this);
        }

        public PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, PointShape exclude)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = TryToGetPoint(shape, target, radius);
                if (result != null && result != exclude)
                {
                    return result;
                }
            }
            return null;
        }

        public BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = GetHitTest(shape)?.Contains(shape, target, radius, this);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<BaseShape>();
            foreach (var shape in shapes.Reverse())
            {
                var result = GetHitTest(shape)?.Overlaps(shape, target, radius, this);
                if (result != null)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
