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

        private void GetSKPaintStrokeText(TextStyle style, out SKPaint paint)
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

            paint = cached.paint;
        }

#if true
        private void DrawTextSKFontMetrics(SKCanvas canvas, Text text, ShapeStyle style, ref SKRect rect, ref SKRect bounds, ref SKPoint origin)
        {
            (SKPaint paint, SKFontMetrics metrics) cached = _textPaintCache[style.TextStyle];

            var mTop = cached.metrics.Top;
            var mAscent = cached.metrics.Ascent;
            var mBaseline = 0.0f;
            var mDescent = cached.metrics.Descent;
            var mBottom = cached.metrics.Bottom;

            var mLeading = cached.metrics.Leading;
            var mCapHeight = cached.metrics.CapHeight;
            var mLineHeight = cached.metrics.Bottom - cached.metrics.Top;

            using (var boundsPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(255, 255, 255, 255) })
            using (var mTopPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(128, 0, 128, 255) })
            using (var mAscentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(0, 255, 0, 255) })
            using (var mBaselinePen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(255, 0, 0, 255) })
            using (var mDescentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(0, 0, 255, 255) })
            using (var mBottomPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(255, 127, 0, 255) })
            {
                var boundsAdjusted = new SKRect(bounds.Left + rect.Left, bounds.Top + rect.Top, bounds.Right + rect.Left, bounds.Bottom + rect.Top);
                canvas.DrawRect(boundsAdjusted, boundsPen); // Bounds - White
                canvas.DrawLine(new SKPoint(rect.Left, rect.Top + mTop), new SKPoint(rect.Left + rect.Width, rect.Top + mTop), mTopPen); // Top - Purple
                canvas.DrawLine(new SKPoint(rect.Left, rect.Top + mAscent), new SKPoint(rect.Left + rect.Width, rect.Top + mAscent), mAscentPen); // Ascent - Green
                canvas.DrawLine(new SKPoint(rect.Left, rect.Top + mBaseline), new SKPoint(rect.Left + rect.Width, rect.Top + mBaseline), mBaselinePen); // Baseline - Red
                canvas.DrawLine(new SKPoint(rect.Left, rect.Top + mDescent), new SKPoint(rect.Left + rect.Width, rect.Top + mDescent), mDescentPen); // Descent - Blue
                canvas.DrawLine(new SKPoint(rect.Left, rect.Top + mBottom), new SKPoint(rect.Left + rect.Width, rect.Top + mBottom), mBottomPen); // Bottom - Orange
            }

            canvas.DrawText($"{text.Value}", rect.Left, rect.Top, cached.paint); // Text

            int line = 2;

            canvas.DrawText($"mTop: {mTop}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"mAscent: {mAscent}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"mBaseline: {mBaseline}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"mDescent: {mDescent}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"mBottom: {mBottom}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);

            canvas.DrawText($"mLeading: {mLeading}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"mCapHeight: {mCapHeight}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"mLineHeight: {mLineHeight}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);

            canvas.DrawText($"rect: {rect}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"bounds: {bounds}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);
            canvas.DrawText($"origin: {origin}", rect.Left, rect.Top + mLineHeight * line++, cached.paint);

        }
#endif

        private void DrawText(SKCanvas canvas, Text text, PointShape topLeft, PointShape bottomRight, ShapeStyle style, double dx, double dy)
        {
            var rect = SkiaHelper.ToRect(topLeft, bottomRight, dx, dy);
            GetSKPaintStrokeText(style.TextStyle, out var paint);
            var bounds = new SKRect();
            paint.MeasureText(text.Value, ref bounds);
            var origin = SkiaHelper.GetTextOrigin(style.TextStyle.HAlign, style.TextStyle.VAlign, ref rect, ref bounds);
            //canvas.DrawText(text.Value, origin.X - bounds.Left, origin.Y - bounds.Top, paint);
#if true
            DrawTextSKFontMetrics(canvas, text, style, ref rect, ref bounds, ref origin);
#endif
        }

        private void DrawTextOnPath(SKCanvas canvas, SKPath path, Text text, TextStyle style)
        {
            GetSKPaintStrokeText(style, out var paint);
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(line.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, line.Text, style.TextStyle);
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(cubicBezier.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, cubicBezier.Text, style.TextStyle);
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(quadraticBezier.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, quadraticBezier.Text, style.TextStyle);
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(conic.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, conic.Text, style.TextStyle);
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(path.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, path.Text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
#if true
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(rectangle, dx, dy))
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(rectangle.Text?.Value))
                    {
                        DrawText(canvas, rectangle.Text, rectangle.TopLeft, rectangle.BottomRight, style, dx, dy);
                    }
                }
            }
#else
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
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
                if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(rectangle.Text?.Value))
                {
                    DrawText(canvas, rectangle.Text, rectangle.TopLeft, rectangle.BottomRight, style, dx, dy);
                }
            }
#endif
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
#if true
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = SkiaHelper.ToGeometry(ellipse, dx, dy))
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
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(ellipse.Text?.Value))
                    {
                        DrawText(canvas, ellipse.Text, ellipse.TopLeft, ellipse.BottomRight, style, dx, dy);
                    }
                }
            }
#else
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
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
                if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(ellipse.Text?.Value))
                {
                    DrawText(canvas, ellipse.Text, ellipse.TopLeft, ellipse.BottomRight, style, dx, dy);
                }
            }
#endif
        }

        public void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(text.Text?.Value))
            {
                var canvas = dc as SKCanvas;
                DrawText(canvas, text.Text, text.TopLeft, text.BottomRight, style, dx, dy);
            }
        }
    }
}
