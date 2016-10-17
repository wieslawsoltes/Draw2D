using System;
using System.Collections.Generic;
using System.Windows;

namespace PathDemo
{
    public class PointShape : ShapeBase
    {
        public double X;
        public double Y;

        public void Update(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public static Point ToPoint(PointShape point)
        {
            return new Point(point.X, point.Y);
        }

        public static PointShape FromPoint(Point point)
        {
            return new PointShape() { X = point.X, Y = point.Y };
        }

        public static implicit operator Point(PointShape point)
        {
            return new Point(point.X, point.Y);
        }

        public static List<Point> ToPoints(IList<PointShape> points)
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
