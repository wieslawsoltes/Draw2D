using System;
using System.Collections.Generic;
using System.Windows;
using Draw2D.Models.Shapes;

namespace PathDemo
{
    public static class PointShapeExtensions
    {
        public static PointShape Clone(this PointShape point)
        {
            return new PointShape() { X = point.X, Y = point.Y };
        }

        public static IList<Point> AsPoints(IList<PointShape> points)
        {
            var result = new List<Point>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                result.Add(new Point(point.X, point.Y));
            }
            return result;
        }
    }
}
