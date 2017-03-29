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
                    bool isFirstShape = true;
                    foreach (var shape in figure.Shapes)
                    {
                        if (shape is LineShape line)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(line.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.LineTo(line.Point.AsPoint(), true, false);
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(cubicBezier.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.BezierTo(cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint(), cubicBezier.Point3.AsPoint(), true, false);
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(quadraticBezier.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
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
