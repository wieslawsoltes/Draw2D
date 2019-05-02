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
    internal enum HAlign
    {
        Left,
        Center,
        Right
    }

    internal enum VAlign
    {
        Top,
        Center,
        Bottom
    }

    internal class SkiaHelper
    {
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
                Color = ToSKColor(color),
                TextAlign = SKTextAlign.Left
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

        public static SKMatrix ToMatrixTransform(Matrix2 m)
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

        public static SKPath ToGeometry(ConicShape conic, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(conic.StartPoint, dx, dy));
            geometry.ConicTo(
                ToPoint(conic.Point1, dx, dy),
                ToPoint(conic.Point2, dx, dy),
                (float)conic.Weight);
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
                    else if (shape is ConicShape conic)
                    {
                        if (isFirstShape)
                        {
                            geometry.MoveTo(ToPoint(conic.StartPoint, dx, dy));
                            isFirstShape = false;
                        }
                        geometry.ConicTo(
                            ToPoint(conic.Point1, dx, dy),
                            ToPoint(conic.Point2, dx, dy),
                            (float)conic.Weight);
                    }
                }

                if (!isFirstShape && figure.IsClosed)
                {
                    geometry.Close();
                }
            }

            return geometry;
        }

        public static SKPoint GetTextOrigin(HAlign halignment, VAlign valignment, ref SKRect rect, ref SKRect size)
        {
            double ox, oy;

            switch (halignment)
            {
                case HAlign.Left:
                    ox = rect.Left;
                    break;
                case HAlign.Center:
                default:
                    ox = (rect.Left + rect.Width / 2f) - (size.Width / 2f);
                    break;
                case HAlign.Right:
                    ox = rect.Right - size.Width;
                    break;
            }

            switch (valignment)
            {
                case VAlign.Top:
                    oy = rect.Top;
                    break;
                case VAlign.Center:
                default:
                    oy = (rect.Bottom - rect.Height / 2f) - (size.Height / 2f);
                    break;
                case VAlign.Bottom:
                    oy = rect.Bottom - size.Height;
                    break;
            }

            return new SKPoint((float)ox, (float)oy);
        }
    }

    public class SkiaShapeRenderer : IShapeRenderer
    {
        public double Scale { get; set; } = 1.0;

        public ISelection Selection { get; set; } = null;

        private void DrawTextOnPath(SKCanvas canvas, Text text, SKPath path, SKPaint paint)
        {
            using (var tf = SKTypeface.FromFamilyName("Calibri", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright))
            {
                paint.Typeface = tf;
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.TextSize = (float)(12.0);

                var bounds = new SKRect();
                float baseTextWidth = paint.MeasureText(text.Value, ref bounds);
                SKPathMeasure pathMeasure = new SKPathMeasure(path, false, 1);
                float hOffset = (pathMeasure.Length / 2f) - (baseTextWidth / 2f);

                canvas.DrawTextOnPath(text.Value, path, hOffset, 0f, paint);
            }
        }

        public object PushMatrix(object dc, Matrix2 matrix)
        {
            var canvas = dc as SKCanvas;
            int count = canvas.Save();
            canvas.SetMatrix(SkiaHelper.Multiply(SkiaHelper.ToMatrixTransform(matrix), canvas.TotalMatrix));
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
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
            {
                if (style.IsStroked)
                {
                    canvas.DrawLine(SkiaHelper.ToPoint(line.StartPoint, dx, dy), SkiaHelper.ToPoint(line.Point, dx, dy), pen);
                }
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = SkiaHelper.ToSKPaintBrush(style.Fill))
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
            using (var geometry = SkiaHelper.ToGeometry(cubicBezier, dx, dy))
            {
                if (style.IsFilled)
                {
                    canvas.DrawPath(geometry, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawPath(geometry, pen);
                }
                if (cubicBezier.Text is Text text && !string.IsNullOrEmpty(text.Value))
                {
                    using (var paint = SkiaHelper.ToSKPaintBrush(style.Stroke))
                    {
                        DrawTextOnPath(canvas, text, geometry, paint);
                    }
                }
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = SkiaHelper.ToSKPaintBrush(style.Fill))
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
            using (var geometry = SkiaHelper.ToGeometry(quadraticBezier, dx, dy))
            {
                if (style.IsFilled)
                {
                    canvas.DrawPath(geometry, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawPath(geometry, pen);
                }
                if (quadraticBezier.Text is Text text && !string.IsNullOrEmpty(text.Value))
                {
                    using (var paint = SkiaHelper.ToSKPaintBrush(style.Stroke))
                    {
                        DrawTextOnPath(canvas, text, geometry, paint);
                    }
                }
            }
        }

        public void DrawConic(object dc, ConicShape conic, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = SkiaHelper.ToSKPaintBrush(style.Fill))
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
            using (var geometry = SkiaHelper.ToGeometry(conic, dx, dy))
            {
                if (style.IsFilled)
                {
                    canvas.DrawPath(geometry, brush);
                }
                if (style.IsStroked)
                {
                    canvas.DrawPath(geometry, pen);
                }
                if (conic.Text is Text text && !string.IsNullOrEmpty(text.Value))
                {
                    using (var paint = SkiaHelper.ToSKPaintBrush(style.Stroke))
                    {
                        DrawTextOnPath(canvas, text, geometry, paint);
                    }
                }
            }
        }

        public void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            var canvas = dc as SKCanvas;
            using (var brush = SkiaHelper.ToSKPaintBrush(style.Fill))
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
            using (var geometry = SkiaHelper.ToGeometry(path, dx, dy))
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
            var rect = SkiaHelper.ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            using (var brush = SkiaHelper.ToSKPaintBrush(style.Fill))
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
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
            var rect = SkiaHelper.ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            using (var brush = SkiaHelper.ToSKPaintBrush(style.Fill))
            using (var pen = SkiaHelper.ToSKPaintPen(style, Scale))
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
            var rect = SkiaHelper.ToRect(text.TopLeft, text.BottomRight, dx, dy);
            using (var paint = SkiaHelper.ToSKPaintBrush(style.Stroke))
            using (var tf = SKTypeface.FromFamilyName("Calibri", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright))
            {
                paint.Typeface = tf;
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.TextSize = (float)(12.0);

                var metrics = paint.FontMetrics;
                var mTop = metrics.Top; 
                var mBottom = metrics.Bottom;
                var mLeading = metrics.Leading;
                var mDescent = metrics.Descent;
                var mAscent = metrics.Ascent;

                var lineHeight = mDescent - mAscent;
                var lineOffset = (-mAscent);
                var offset = -mDescent - mAscent;

                var bounds = new SKRect();
                paint.MeasureText(text.Text.Value, ref bounds);
                var origin = SkiaHelper.GetTextOrigin(HAlign.Center, VAlign.Center, ref rect, ref bounds);
                canvas.DrawText(text.Text.Value, origin.X, origin.Y + offset, paint);
                /*
                float y = lineOffset;
                canvas.DrawText($"mTop: {mTop}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"mBottom: {mBottom}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"mLeading: {mLeading}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"mDescent: {mDescent}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"mAscent: {mAscent}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"lineHeight: {lineHeight}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"lineOffset: {lineOffset}", 0f, y, paint);
                y += lineHeight;
                canvas.DrawText($"offset: {offset}", 0f, y, paint);
                //*/
            }
        }
    }
}
