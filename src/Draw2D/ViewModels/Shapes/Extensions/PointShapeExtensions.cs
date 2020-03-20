using Spatial;

namespace Draw2D.ViewModels.Shapes
{
    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this IPointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static IPointShape FromPoint2(this Point2 point, IBaseShape template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }

        public static double DistanceTo(this IPointShape point, IPointShape other)
        {
            return point.ToPoint2().DistanceTo(other.ToPoint2());
        }

        public static Rect2 ToRect2(this IPointShape center, double radius)
        {
            return Rect2.FromPoints(
                center.X - radius, 
                center.Y - radius, 
                center.X + radius, 
                center.Y + radius);
        }
    }
}
