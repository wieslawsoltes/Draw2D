// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Renderers
{
    public class SkiaShapeRenderer : IShapeRenderer
    {
        private readonly double _scale;
    
        public ISelection Selection { get; set; }

        public SkiaShapeRenderer()
        {
            _scale = 1.0;
        }

        public SkiaShapeRenderer(double scale)
        {
            _scale = scale;
        }

        public static SKColor ToSKColor(ArgbColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static SKPaint ToSKPaintPen(ShapeStyle style, double scale)
        {
            return new SKPaint()
            {
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = (float)(style.Thickness / scale),
                Color = ToSKColor(style.Stroke),
                StrokeCap = SKStrokeCap.Butt,
                PathEffect = null
            };
        }

        public static SKPaint ToSKPaintBrush(ArgbColor color)
        {
            return new SKPaint()
            {
                IsAntialias = true,
                IsStroke = false,
                LcdRenderText = true,
                SubpixelText = true,
                Color = ToSKColor(color)
            };
        }

        public static SKPoint ToPoint(PointShape point, double dx, double dy)
        {
            return new SKPoint((float)(point.X + dx), (float)(point.Y + dy));
        }

        public static IEnumerable<SKPoint> ToPoints(IEnumerable<PointShape> points, double dx, double dy)
        {
            return points.Select(point => new SKPoint((float)(point.X + dx), (float)(point.Y + dy)));
        }

        public static SKRect ToRect(double left, double top, double right, double bottom)
        {
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        public static SKRect ToRect(PointShape p1, PointShape p2, double dx, double dy)
        {
            double left = Math.Min(p1.X + dx, p2.X + dx);
            double top = Math.Min(p1.Y + dy, p2.Y + dy);
            double right = left + Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - left);
            double bottom = top + Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - top);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        public static SKMatrix Multiply(SKMatrix value1, SKMatrix value2)
        {
            return ToSKMatrix(
                (value1.ScaleX * value2.ScaleX) + (value1.SkewY * value2.SkewX),
                (value1.ScaleX * value2.SkewY) + (value1.SkewY * value2.ScaleY),
                (value1.SkewX * value2.ScaleX) + (value1.ScaleY * value2.SkewX),
                (value1.SkewX * value2.SkewY) + (value1.ScaleY * value2.ScaleY),
                (value1.TransX * value2.ScaleX) + (value1.TransY * value2.SkewX) + value2.TransX,
                (value1.TransX * value2.SkewY) + (value1.TransY * value2.ScaleY) + value2.TransY);
        }

        public static SKMatrix ToSKMatrix(double m11, double m12, double m21, double m22, double m31, double m32)
        {
            return new SKMatrix
            {
                ScaleX = (float)m11,
                SkewX = (float)m21,
                TransX = (float)m31,
                SkewY = (float)m12,
                ScaleY = (float)m22,
                TransY = (float)m32,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        public static SKMatrix ToMatrixTransform(MatrixObject m)
        {
            return ToSKMatrix(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        public static SKPath ToGeometry(CubicBezierShape cubicBezier, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(cubicBezier.StartPoint, dx, dy));
            geometry.CubicTo(
                ToPoint(cubicBezier.Point1, dx, dy),
                ToPoint(cubicBezier.Point2, dx, dy),
                ToPoint(cubicBezier.Point3, dx, dy));
            return geometry;
        }

        public static SKPath ToGeometry(QuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(quadraticBezier.StartPoint, dx, dy));
            geometry.QuadTo(
                ToPoint(quadraticBezier.Point1, dx, dy),
                ToPoint(quadraticBezier.Point2, dx, dy));
            return geometry;
        }

        public static SKPath ToGeometry(PathShape path, double dx, double dy)
        {
            var geometry = new SKPath
            {
                FillType = path.FillRule == PathFillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            foreach (var figure in path.Figures)
            {
                bool isFirstShape = true;
                foreach (var shape in figure.Shapes)
                {
                    if (shape is LineShape line)
                    {
                        if (isFirstShape)
                        {
                            geometry.MoveTo(ToPoint(line.StartPoint, dx, dy));
                            isFirstShape = false;
                        }
                        geometry.LineTo(ToPoint(line.Point, dx, dy));
                    }
                    else if (shape is CubicBezierShape cubicBezier)
                    {
                        if (isFirstShape)
                        {
                            geometry.MoveTo(ToPoint(cubicBezier.StartPoint, dx, dy));
                            isFirstShape = false;
                        }
                        geometry.CubicTo(
                            ToPoint(cubicBezier.Point1, dx, dy),
                            ToPoint(cubicBezier.Point2, dx, dy),
                            ToPoint(cubicBezier.Point3, dx, dy));
                    }
                    else if (shape is QuadraticBezierShape quadraticBezier)
                    {
                        if (isFirstShape)
                        {
                            geometry.MoveTo(ToPoint(quadraticBezier.StartPoint, dx, dy));
                            isFirstShape = false;
                        }
                        geometry.QuadTo(
                            ToPoint(quadraticBezier.Point1, dx, dy),
                            ToPoint(quadraticBezier.Point2, dx, dy));
                    }
                }

                if (!isFirstShape && figure.IsClosed)
                {
                    geometry.Close();
                }
            }

            return geometry;
        }

        public static SKPoint GetTextOrigin(int halignment, int valignment, ref SKRect rect, ref SKRect size)
        {
            double rwidth = Math.Abs(rect.Right - rect.Left);
            double rheight = Math.Abs(rect.Bottom - rect.Top);
            double swidth = Math.Abs(size.Right - size.Left);
            double sheight = Math.Abs(size.Bottom - size.Top);
            double ox, oy;

            switch (halignment)
            {
                case 0:
                    ox = rect.Left;
                    break;
                case 1:
                    ox = rect.Right - swidth;
                    break;
                case 2:
                default:
                    ox = (rect.Left + rwidth / 2f) - (swidth / 2f);
                    break;
            }

            switch (valignment)
            {
                case 0:
                    oy = rect.Top;
                    break;
                case 1:
                    oy = rect.Bottom - sheight;
                    break;
                case 2:
                default:
                    oy = (rect.Bottom - rheight / 2f) - (sheight / 2f);
                    break;
            }

            return new SKPoint((float)ox, (float)oy);
        }

        public void InvalidateCache(ShapeStyle style)
        {
        }

        public void InvalidateCache(MatrixObject matrix)
        {
        }

        public void InvalidateCache(BaseShape shape, ShapeStyle style, double dx, double dy)
        {
        }

        public object PushMatrix(object dc, MatrixObject matrix)
        {
            var canvas = dc as SKCanvas;
            int count = canvas.Save();
            canvas.SetMatrix(Multiply(ToMatrixTransform(matrix), canvas.TotalMatrix));
            return count;
        }

        public void PopMatrix(object dc, object state)
        {
            var canvas = dc as SKCanvas;
            var count = (int)state;
            canvas.RestoreToCount(count);
        }

        public void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var pen = ToSKPaintPen(style, _scale))
            {
                if (style.IsStroked)
                {
                    canvas.DrawLine(ToPoint(line.StartPoint, dx, dy), ToPoint(line.Point, dx, dy), pen);
                }
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = ToSKPaintBrush(style.Fill))
            using (var pen = ToSKPaintPen(style, _scale))
            using (var geometry = ToGeometry(cubicBezier, dx, dy))
            {
                if (style.IsFilled)
                {
                    canvas.DrawPath(geometry, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawPath(geometry, pen);
                }
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = ToSKPaintBrush(style.Fill))
            using (var pen = ToSKPaintPen(style, _scale))
            using (var geometry = ToGeometry(quadraticBezier, dx, dy))
            {
                if (style.IsFilled)
                {
                    canvas.DrawPath(geometry, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawPath(geometry, pen);
                }
            }
        }

        public void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = ToSKPaintBrush(style.Fill))
            using (var pen = ToSKPaintPen(style, _scale))
            using (var geometry = ToGeometry(path, dx, dy))
            {
                if (style.IsFilled)
                {
                    canvas.DrawPath(geometry, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawPath(geometry, pen);
                }
            }
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            using (var brush = ToSKPaintBrush(style.Fill))
            using (var pen = ToSKPaintPen(style, _scale))
            {
                if (style.IsFilled)
                {
                    canvas.DrawRect(rect, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawRect(rect, pen);
                }
            }
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            using (var brush = ToSKPaintBrush(style.Fill))
            using (var pen = ToSKPaintPen(style, _scale))
            {
                if (style.IsFilled)
                {
                    canvas.DrawOval(rect, brush);
                }

                if (style.IsStroked)
                {
                    canvas.DrawOval(rect, pen);
                }
            }
        }

        public void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);
            using (var paint = ToSKPaintBrush(style.Stroke))
            using (var tf = SKTypeface.FromFamilyName("Calibri", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright))
            {
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.TextSize = (float)(12.0);

                var fm = paint.FontMetrics;
                float offset = -(fm.Top + fm.Bottom);

                var bounds = new SKRect();
                paint.MeasureText(text.Text.Value, ref bounds);
                var origin = GetTextOrigin(2, 2, ref rect, ref bounds);

                canvas.DrawText(text.Text.Value, origin.X, origin.Y + offset, paint);
            }
        }
    }
}
