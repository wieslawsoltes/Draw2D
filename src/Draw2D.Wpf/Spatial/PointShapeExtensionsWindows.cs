using System.Windows;

namespace Draw2D.Core.Shapes
{
    public static class PointShapeExtensionsWindows
    {
        public static Point AsPoint(this PointShape point)
        {
            return new Point(point.X, point.Y);
        }

        public static void Update(this PointShape target, Point source)
        {
            target.X = source.X;
            target.Y = source.Y;
        }
    }
}
