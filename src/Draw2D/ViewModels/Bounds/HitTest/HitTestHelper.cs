﻿using System.Collections.Generic;
using System.Linq;
using Spatial;
using Spatial.ConvexHull;
using Spatial.Sat;

namespace Draw2D.ViewModels.Bounds;

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

    public static void ToConvexHull(IEnumerable<IPointShape> points, out int k, out Vector2[] convexHull)
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

    public static bool Contains(IEnumerable<IPointShape> points, Point2 point)
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

    public static bool Overlap(IEnumerable<IPointShape> points, Vector2[] selectionState)
    {
        ToConvexHull(points, out int k, out Vector2[] convexHull);
        Vector2[] vertices = convexHull.Take(k).ToArray();
        return SAT.Overlap(selectionState, vertices);
    }

    public static bool Overlap(IEnumerable<IPointShape> points, Rect2 rect)
    {
        return Overlap(points, ToSelection(rect));
    }
}