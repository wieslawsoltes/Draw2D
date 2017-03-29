using System.Windows.Media;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class GeometryConverter
    {
        public static Geometry ToGeometry(PathShape path)
        {
            var geometry = new StreamGeometry()
            {
                FillRule = path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero
            };

            using (var context = geometry.Open())
            {
                foreach (var figure in path.Figures)
                {
                    bool isFirstSegment = true;
                    foreach (var segment in figure.Segments)
                    {
                        if (segment is LineShape line)
                        {
                            if (isFirstSegment)
                            {
                                context.BeginFigure(line.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.LineTo(line.Point.AsPoint(), true, false);
                        }
                        else if (segment is CubicBezierShape cubicBezier)
                        {
                            if (isFirstSegment)
                            {
                                context.BeginFigure(cubicBezier.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.BezierTo(cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint(), cubicBezier.Point3.AsPoint(), true, false);
                        }
                        else if (segment is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstSegment)
                            {
                                context.BeginFigure(quadraticBezier.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.QuadraticBezierTo(quadraticBezier.Point1.AsPoint(), quadraticBezier.Point2.AsPoint(), true, false);
                        }
                    }
                }
            }

            return geometry;
        }
    }
}
