using System;
using Draw2D.Spatial;

namespace Draw2D.Models.Shapes
{
    public static class LineShapeExtensions
    {
        public static Line2 ToLine2(this LineShape line, double dx = 0.0, double dy = 0.0)
        {
            return Line2.FromPoints(
                line.Start.X, line.Start.Y,
                line.End.X, line.End.Y,
                dx, dy);
        }

        public static LineShape FromLine2(this Line2 line)
        {
            return new LineShape(line.A.FromPoint2(), line.B.FromPoint2());
        }
    }
}
