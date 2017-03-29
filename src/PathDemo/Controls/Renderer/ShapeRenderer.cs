using System;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class ShapeRenderer
    {
        public void DrawLine(DrawingContext dc, LineShape line)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(line.StartPoint.AsPoint(), false, false);
                context.LineTo(line.Point.AsPoint(), true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        public void DrawCubicBezier(DrawingContext dc, CubicBezierShape cubicBezier)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(cubicBezier.StartPoint.AsPoint(), false, false);
                context.BezierTo(cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint(), cubicBezier.Point3.AsPoint(), true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        public void DrawQuadraticBezier(DrawingContext dc, QuadraticBezierShape quadraticBezier)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(quadraticBezier.StartPoint.AsPoint(), false, false);
                context.QuadraticBezierTo(quadraticBezier.Point1.AsPoint(), quadraticBezier.Point2.AsPoint(), true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        public void DrawPath(DrawingContext dc, PathShape path)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = GeometryConverter.ToGeometry(path);
            dc.DrawGeometry(null, penStroke, geometry);
        }

        public void Draw(DrawingContext dc, IToolContext context)
        {
            foreach (var shape in context.CurrentContainer.Shapes)
            {
                if (shape is LineShape line)
                {
                    DrawLine(dc, line);
                    if (context.Selected.Contains(line))
                    {
                        LineHelper.Draw(dc, line);
                    }
                }
                else if (shape is CubicBezierShape cubicBezier)
                {
                    DrawCubicBezier(dc, cubicBezier);
                    if (context.Selected.Contains(cubicBezier))
                    {
                        CubiceBezierHelper.Draw(dc, cubicBezier);
                    }
                }
                else if (shape is QuadraticBezierShape quadraticBezier)
                {
                    DrawQuadraticBezier(dc, quadraticBezier);
                    if (context.Selected.Contains(quadraticBezier))
                    {
                        QuadraticBezierHelper.Draw(dc, quadraticBezier);
                    }
                }
                else if (shape is PathShape path)
                {
                    DrawPath(dc, path);
                    if (context.Selected.Contains(path))
                    {
                        PathHelper.Draw(dc, path, context);
                    }
                }
            }
        }
    }
}
