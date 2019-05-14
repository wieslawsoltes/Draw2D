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
    internal class SkiaHelper
    {
        public static SKTypeface ToSKTypeface(string familyName)
        {
            return SKTypeface.FromFamilyName(
                familyName,
                SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright);
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
                Color = ToSKColor(color),
                TextAlign = SKTextAlign.Left
            };
        }

        public static void ToSKPaintPenUpdate(SKPaint paint, ShapeStyle style, double scale)
        {
            paint.StrokeWidth = (float)(style.Thickness / scale);
            paint.Color = ToSKColor(style.Stroke);
        }

        public static void ToSKPaintBrushUpdate(SKPaint paint, ArgbColor color)
        {
            paint.Color = ToSKColor(color);
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

        public static SKPath ToGeometry(LineShape line, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(line.StartPoint, dx, dy));
            geometry.LineTo(ToPoint(line.Point, dx, dy));
            return geometry;
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

        public static SKPoint GetTextOrigin(HAlign hAlign, VAlign vAlign, ref SKRect rect, ref SKRect size)
        {
            double ox, oy;

            switch (hAlign)
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

            switch (vAlign)
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
        private Dictionary<string, SKTypeface> _typefaceCache;
        private Dictionary<TextStyle, (SKPaint, SKFontMetrics)> _textPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _fillPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _strokePaintCache;

        public double Scale { get; set; } = 1.0;

        public ISelection Selection { get; set; } = null;

        public SkiaShapeRenderer()
        {
            // TODO: Properly dispose SKTypeface objects.
            _typefaceCache = new Dictionary<string, SKTypeface>();
            // TODO: Properly dispose SKPaint and SKFontMetrics objects.
            _textPaintCache = new Dictionary<TextStyle, (SKPaint, SKFontMetrics)>();
            // TODO: Properly dispose SKPaint objects.
            _fillPaintCache = new Dictionary<ShapeStyle, SKPaint>();
            // TODO: Properly dispose SKPaint objects.
            _strokePaintCache = new Dictionary<ShapeStyle, SKPaint>();
        }

        public void Dispose()
        {
            if (_typefaceCache != null)
            {
                foreach (var cache in _typefaceCache)
                {
                    cache.Value.Dispose();
                }
                _typefaceCache = null;
            }

            if (_textPaintCache != null)
            {
                foreach (KeyValuePair<TextStyle, (SKPaint paint, SKFontMetrics metrics)> cache in _textPaintCache)
                {
                    cache.Value.paint.Dispose();
                }
                _textPaintCache = null;
            }

            if (_fillPaintCache != null)
            {
                foreach (var cache in _fillPaintCache)
                {
                    cache.Value.Dispose();
                }
                _fillPaintCache = null;
            }

            if (_strokePaintCache != null)
            {
                foreach (var cache in _strokePaintCache)
                {
                    cache.Value.Dispose();
                }
                _strokePaintCache = null;
            }
        }

        private void GetSKTypeface(string familyName, out SKTypeface typeface)
        {
            if (!_typefaceCache.TryGetValue(familyName, out typeface))
            {
                typeface = SkiaHelper.ToSKTypeface(familyName);
                _typefaceCache[familyName] = typeface;
            }
        }

        private void GetSKPaintFill(ShapeStyle style, out SKPaint brush)
        {
            if (style.Fill.IsDirty == true || !_fillPaintCache.TryGetValue(style, out var brushCached))
            {
                style.Fill.Invalidate();
                brushCached = SkiaHelper.ToSKPaintBrush(style.Fill);
                _fillPaintCache[style] = brushCached;
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(brushCached, style.Fill);
            }

            brush = brushCached;
        }

        private void GetSKPaintStroke(ShapeStyle style, out SKPaint pen, double scale)
        {
            if (style.IsDirty == true || style.Stroke.IsDirty == true || !_strokePaintCache.TryGetValue(style, out var penCached))
            {
                style.Invalidate();
                style.Stroke.Invalidate();
                penCached = SkiaHelper.ToSKPaintPen(style, scale);
                _strokePaintCache[style] = penCached;
            }
            else
            {
                SkiaHelper.ToSKPaintPenUpdate(penCached, style, scale);
            }

            pen = penCached;
        }

        private void GetSKPaintStrokeText(TextStyle style, out SKPaint paint, out float offset)
        {
            (SKPaint paint, SKFontMetrics metrics) cached;
            cached.paint = null;
            cached.metrics = default;

            if (style.IsDirty == true || style.Stroke.IsDirty == true || !_textPaintCache.TryGetValue(style, out cached))
            {
                style.Invalidate();
                style.Stroke.Invalidate();
                GetSKTypeface(style.FontFamily, out var typeface);
                cached.paint = SkiaHelper.ToSKPaintBrush(style.Stroke);
                cached.paint.Typeface = typeface;
                cached.paint.TextEncoding = SKTextEncoding.Utf16;
                cached.paint.TextSize = (float)style.FontSize;
                cached.metrics = cached.paint.FontMetrics;
                _textPaintCache[style] = cached;
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(cached.paint, style.Stroke);
            }

            var mTop = cached.metrics.Top;
            var mBottom = cached.metrics.Bottom;
            var mLeading = cached.metrics.Leading;
            var mDescent = cached.metrics.Descent;
            var mAscent = cached.metrics.Ascent;
            var lineHeight = mDescent - mAscent;
            var lineOffset = (-mAscent);

            offset = -mDescent - mAscent;
            paint = cached.paint;
        }

        private void DrawTextOnPath(SKCanvas canvas, SKPath path, Text text, TextStyle style)
        {
            GetSKPaintStrokeText(style, out var paint, out _);
            var bounds = new SKRect();
            float baseTextWidth = paint.MeasureText(text.Value, ref bounds);
            SKPathMeasure pathMeasure = new SKPathMeasure(path, false, 1);
            float hOffset = (pathMeasure.Length / 2f) - (baseTextWidth / 2f);
            canvas.DrawTextOnPath(text.Value, path, hOffset, 0f, paint);
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
#if true
            if (style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(line, dx, dy))
                {
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, Scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && line.Text is Text text && !string.IsNullOrEmpty(text.Value))
                    {
                        DrawTextOnPath(canvas, geometry, text, style.TextStyle);
                    }
                }
            }
#else
            if (style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, Scale);
                    canvas.DrawLine(SkiaHelper.ToPoint(line.StartPoint, dx, dy), SkiaHelper.ToPoint(line.Point, dx, dy), pen);
                }
            }
#endif
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(cubicBezier, dx, dy))
                {
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, Scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && cubicBezier.Text is Text text && !string.IsNullOrEmpty(text.Value))
                    {
                        DrawTextOnPath(canvas, geometry, text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            if(style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(quadraticBezier, dx, dy))
                {
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, Scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && quadraticBezier.Text is Text text && !string.IsNullOrEmpty(text.Value))
                    {
                        DrawTextOnPath(canvas, geometry, text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawConic(object dc, ConicShape conic, ShapeStyle style, double dx, double dy)
        {
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(conic, dx, dy))
                {
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, Scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && conic.Text is Text text && !string.IsNullOrEmpty(text.Value))
                    {
                        DrawTextOnPath(canvas, geometry, text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(path, dx, dy))
                {
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, Scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && path.Text is Text text && !string.IsNullOrEmpty(text.Value))
                    {
                        DrawTextOnPath(canvas, geometry, text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
            if (style.IsFilled || style.IsStroked)
            {
                var canvas = dc as SKCanvas;
                var rect = SkiaHelper.ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
                if (style.IsFilled)
                {
                    GetSKPaintFill(style, out var brush);
                    canvas.DrawRect(rect, brush);
                }
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, Scale);
                    canvas.DrawRect(rect, pen);
                }
            }
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            if (style.IsFilled || style.IsStroked)
            {
                var canvas = dc as SKCanvas;
                var rect = SkiaHelper.ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
                if (style.IsFilled)
                {
                    GetSKPaintFill(style, out var brush);
                    canvas.DrawOval(rect, brush);
                }
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, Scale);
                    canvas.DrawOval(rect, pen);
                }
            }
        }

        public void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            if (style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                var rect = SkiaHelper.ToRect(text.TopLeft, text.BottomRight, dx, dy);
                GetSKPaintStrokeText(style.TextStyle, out var paint, out var offset);
                var bounds = new SKRect();
                paint.MeasureText(text.Text.Value, ref bounds);
                var origin = SkiaHelper.GetTextOrigin(style.TextStyle.HAlign, style.TextStyle.VAlign, ref rect, ref bounds);
                canvas.DrawText(text.Text.Value, origin.X, origin.Y + offset, paint);
            }
        }
    }
}
