// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Editor.Renderers
{
    public class SkiaShapeRenderer : IShapeRenderer
    {
        private Dictionary<string, SKTypeface> _typefaceCache;
        private Dictionary<TextStyle, (SKPaint, SKFontMetrics)> _textPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _fillPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _strokePaintCache;

        public double Scale { get; set; } = 1.0;

        public ISelectionState SelectionState { get; set; } = null;

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
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
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
