using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;
using Draw2D.Spatial.ConvexHull;
using Draw2D.Spatial.Sat;

namespace Draw2D.Editor.Bounds
{
    public static class HitTestHelper
    {
        public static MonotoneChain MC { get; private set; }
        public static SeparatingAxisTheorem SAT { get; private set; }

        static HitTestHelper()
        {
            MC = new MonotoneChain();
            SAT = new SeparatingAxisTheorem();
        }

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

        public static void ToConvexHull(IList<PointShape> points, out int k, out Vector2[] convexHull)
        {
            Vector2[] vertices = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = new Vector2(points[i].X, points[i].Y);
            }
            MC.ConvexHull(vertices, out convexHull, out k);
        }

        public static bool Contains(IList<PointShape> points, Vector2 v)
        {
            int k;
            Vector2[] convexHull;
            ToConvexHull(points, out k, out convexHull);

            bool contains = false;
            for (int i = 0, j = k - 2; i < k - 1; j = i++)
            {
                if (((convexHull[i].Y > v.Y) != (convexHull[j].Y > v.Y))
                    && (v.X < (convexHull[j].X - convexHull[i].X) * (v.Y - convexHull[i].Y) / (convexHull[j].Y - convexHull[i].Y) + convexHull[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        public static bool Overlap(IList<PointShape> points, Vector2[] selection)
        {
            int k;
            Vector2[] convexHull;
            ToConvexHull(points, out k, out convexHull);

            Vector2[] vertices = convexHull.Take(k).ToArray();
            return SAT.Overlap(selection, vertices);
        }
    }
}
