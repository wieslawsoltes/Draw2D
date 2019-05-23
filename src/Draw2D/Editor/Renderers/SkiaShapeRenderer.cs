// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define DEBUG_DRAW_TEXT
//#define USE_DRAW_LINE
//#define USE_DRAW_RECT
//#define USE_DRAW_OVAL
using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Editor.Renderers
{
    public class SkiaShapeRenderer : IShapeRenderer
    {
        private IStyleLibrary _styleLibrary;
        private Dictionary<string, SKTypeface> _typefaceCache;
        private Dictionary<TextStyle, (SKPaint paint, SKFontMetrics metrics)> _textPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _fillPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _strokePaintCache;

        public double Scale { get; set; } = 1.0;

        public ISelectionState SelectionState { get; set; }

        public SkiaShapeRenderer(IStyleLibrary styleLibrary)
        {
            _styleLibrary = styleLibrary;
            _typefaceCache = new Dictionary<string, SKTypeface>();
            _textPaintCache = new Dictionary<TextStyle, (SKPaint paint, SKFontMetrics metrics)>();
            _fillPaintCache = new Dictionary<ShapeStyle, SKPaint>();
            _strokePaintCache = new Dictionary<ShapeStyle, SKPaint>();
        }

        public void Dispose()
        {
            _styleLibrary = null;

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
                foreach (var cache in _textPaintCache)
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
                Debug.WriteLine($"ToSKTypeface: ctor()");
            }
        }

        private void GetSKPaintFill(ShapeStyle style, out SKPaint brush)
        {
            if (style.Fill.IsDirty == true || !_fillPaintCache.TryGetValue(style, out var brushCached))
            {
                style.Fill.Invalidate();
                brushCached = SkiaHelper.ToSKPaintBrush(style.Fill);
                _fillPaintCache[style] = brushCached;
                Debug.WriteLine($"ToSKPaintBrush: ctor()");
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
                Debug.WriteLine($"ToSKPaintPen: ctor()");
            }
            else
            {
                SkiaHelper.ToSKPaintPenUpdate(penCached, style, scale);
            }

            pen = penCached;
        }

        private void GetSKPaintStrokeText(TextStyle style, out SKPaint paint, out SKFontMetrics metrics)
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
                Debug.WriteLine($"ToSKPaintBrush: ctor()");

                switch (style.HAlign)
                {
                    default:
                    case HAlign.Left:
                        cached.paint.TextAlign = SKTextAlign.Left;
                        break;
                    case HAlign.Center:
                        cached.paint.TextAlign = SKTextAlign.Center;
                        break;
                    case HAlign.Right:
                        cached.paint.TextAlign = SKTextAlign.Right;
                        break;
                }

                cached.metrics = cached.paint.FontMetrics;
                _textPaintCache[style] = cached;
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(cached.paint, style.Stroke);
            }

            paint = cached.paint;
            metrics = cached.metrics;
        }

        private void DrawText(SKCanvas canvas, Text text, PointShape topLeft, PointShape bottomRight, ShapeStyle style, double dx, double dy)
        {
            var rect = SkiaHelper.ToRect(topLeft, bottomRight, dx, dy);
            GetSKPaintStrokeText(style.TextStyle, out var paint, out var metrics);
#if DEBUG_DRAW_TEXT
            var mBaseline = 0.0f;
#endif
            var mTop = metrics.Top;
            var mAscent = metrics.Ascent;
            var mDescent = metrics.Descent;
            var mBottom = metrics.Bottom;
            var mLeading = metrics.Leading;
            var mCapHeight = metrics.CapHeight;
            var mLineHeight = metrics.Bottom - metrics.Top;
            var mXMax = metrics.XMax;
            var mXMin = metrics.XMin;

            float x = rect.Left;
            float y = rect.Top;
            float width = rect.Width;
            float height = rect.Height;

            switch (style.TextStyle.VAlign)
            {
                default:
                case VAlign.Top:
                    y -= mAscent;
                    break;
                case VAlign.Center:
                    y += (height / 2.0f) - (mAscent / 2.0f) - mDescent / 2.0f;
                    break;
                case VAlign.Bottom:
                    y += height - mDescent;
                    break;
            }
#if DEBUG_DRAW_TEXT
            using (var boundsPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(255, 255, 255, 255) })
            using (var mTopPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(128, 0, 128, 255) })
            using (var mAscentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(0, 255, 0, 255) })
            using (var mBaselinePen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(255, 0, 0, 255) })
            using (var mDescentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(0, 0, 255, 255) })
            using (var mBottomPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.Thickness / Scale), Color = new SKColor(255, 127, 0, 255) })
            {
                var bounds = new SKRect();
                paint.MeasureText(text.Value, ref bounds);
                var boundsAdjusted = new SKRect(x + bounds.Left, y + bounds.Top, x + bounds.Right, y + bounds.Bottom);
                canvas.DrawRect(boundsAdjusted, boundsPen);
                canvas.DrawLine(new SKPoint(x, y + mTop), new SKPoint(x + width, y + mTop), mTopPen);
                canvas.DrawLine(new SKPoint(x, y + mAscent), new SKPoint(x + width, y + mAscent), mAscentPen);
                canvas.DrawLine(new SKPoint(x, y + mBaseline), new SKPoint(x + width, y + mBaseline), mBaselinePen);
                canvas.DrawLine(new SKPoint(x, y + mDescent), new SKPoint(x + width, y + mDescent), mDescentPen);
                canvas.DrawLine(new SKPoint(x, y + mBottom), new SKPoint(x + width, y + mBottom), mBottomPen);
            }
#endif
            switch (style.TextStyle.HAlign)
            {
                default:
                case HAlign.Left:
                    // x = x;
                    break;
                case HAlign.Center:
                    x += width / 2.0f;
                    break;
                case HAlign.Right:
                    x += width;
                    break;
            }
#if DEBUG_DRAW_TEXT
            int line = 2;
            canvas.DrawText($"Top: {mTop}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Ascent: {mAscent}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Baseline: {mBaseline}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Descent: {mDescent}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Bottom: {mBottom}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Leading: {mLeading}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"CapHeight: {mCapHeight}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"LineHeight: {mLineHeight}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"XMax: {mXMax}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"XMin: {mXMin}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"x: {x}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"y: {y}", x, y + mLineHeight * line++, paint);
#endif
            canvas.DrawText(text.Value, x, y, paint);
        }

        private void DrawTextOnPath(SKCanvas canvas, SKPath path, Text text, TextStyle style)
        {
            GetSKPaintStrokeText(style, out var paint, out var metrics);
            var bounds = new SKRect();
            float baseTextWidth = paint.MeasureText(text.Value, ref bounds);
            SKPathMeasure pathMeasure = new SKPathMeasure(path, false, 1);
            float hOffset = (pathMeasure.Length / 2f) - (baseTextWidth / 2f);
            canvas.DrawTextOnPath(text.Value, path, hOffset, metrics.Bottom - metrics.Top, paint);
        }

        public void DrawLine(object dc, LineShape line, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
#if USE_DRAW_LINE
            if (style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, Scale);
                    canvas.DrawLine(SkiaHelper.ToPoint(line.StartPoint, dx, dy), SkiaHelper.ToPoint(line.Point, dx, dy), pen);
                }
            }
#else
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
#endif
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
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

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
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

        public void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
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

        public void DrawPath(object dc, PathShape path, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
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

        public void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
#if USE_DRAW_RECT
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
#else
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
#endif
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
#if USE_DRAW_OVAL
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
#else
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
#endif
        }

        public void DrawText(object dc, TextShape text, string styleId, double dx, double dy)
        {
            var style = _styleLibrary.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(text.Text?.Value))
            {
                var canvas = dc as SKCanvas;
                DrawText(canvas, text.Text, text.TopLeft, text.BottomRight, style, dx, dy);
            }
        }
    }
}
