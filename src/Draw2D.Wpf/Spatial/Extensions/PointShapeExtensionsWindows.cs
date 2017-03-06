using System;
using System.Windows;

namespace Draw2D.Models.Shapes
{
    public static class PointShapeExtensionsWindows
    {
        public static Point ToPoint(this PointShape point)
        {
            return new Point(point.X, point.Y);
        }
    }
}
