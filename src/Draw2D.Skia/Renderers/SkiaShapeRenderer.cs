// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Renderers
{
    public class SkiaShapeRenderer : IShapeRenderer
    {
        private IToolContext _context;
        private IContainerView _view;
        private ISelectionState _selectionState;
        private Dictionary<Typeface, SKTypeface> _typefaceCache;
        private Dictionary<ITextPaint, (SKPaint paint, SKFontMetrics metrics)> _textPaintCache;
        private Dictionary<IFillPaint, SKPaint> _fillPaintCache;
        private Dictionary<IStrokePaint, SKPaint> _strokePaintCache;
        private Dictionary<string, SKPicture> _pictureCache;
        private CompositeDisposable _pathEffectDisposable;

        public SkiaShapeRenderer(IToolContext context, IContainerView view, ISelectionState selectionState)
        {
            _context = context;
            _view = view;
            _selectionState = selectionState;
            _typefaceCache = new Dictionary<Typeface, SKTypeface>();
            _textPaintCache = new Dictionary<ITextPaint, (SKPaint paint, SKFontMetrics metrics)>();
            _fillPaintCache = new Dictionary<IFillPaint, SKPaint>();
            _strokePaintCache = new Dictionary<IStrokePaint, SKPaint>();
            _pictureCache = new Dictionary<string, SKPicture>();
            _pathEffectDisposable = new CompositeDisposable();
        }

        public void Dispose()
        {
            _context = null;
            _selectionState = null;

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

            if (_pictureCache != null)
            {
                foreach (var cache in _pictureCache)
                {
                    cache.Value.Dispose();
                }
                _pictureCache = null;
            }

            if (_pathEffectDisposable != null)
            {
                _pathEffectDisposable.Dispose();
                _pathEffectDisposable = null;
            }
        }

        private void GetSKTypeface(Typeface style, out SKTypeface typeface)
        {
            if (style.IsDirty || !_typefaceCache.TryGetValue(style, out typeface))
            {
                typeface = SkiaHelper.ToSKTypeface(style);
                _typefaceCache[style] = typeface;
            }
        }

        private void GetSKPaintFill(IFillPaint fillPaint, out SKPaint brush)
        {
            if (fillPaint.IsFillPaintDirty() || !_fillPaintCache.TryGetValue(fillPaint, out var brushCached))
            {
                brushCached = SkiaHelper.ToSKPaintFill(fillPaint, _pathEffectDisposable.Disposables);
                _fillPaintCache[fillPaint] = brushCached;
            }
            else
            {
                SkiaHelper.ToSKPaintFillUpdate(brushCached, fillPaint, _pathEffectDisposable.Disposables);
            }

            brush = brushCached;
        }

        private void GetSKPaintStroke(IStrokePaint strokePaint, out SKPaint pen, double scale)
        {
            if (strokePaint.IsStrokePaintDirty() || !_strokePaintCache.TryGetValue(strokePaint, out var penCached))
            {
                penCached = SkiaHelper.ToSKPaintStroke(strokePaint, scale, _pathEffectDisposable.Disposables);
                _strokePaintCache[strokePaint] = penCached;
            }
            else
            {
                SkiaHelper.ToSKPaintStrokeUpdate(penCached, strokePaint, scale, _pathEffectDisposable.Disposables);
            }

            pen = penCached;
        }

        private void GetSKPaintText(ITextPaint textPaint, out SKPaint paint, out SKFontMetrics metrics)
        {
            (SKPaint paint, SKFontMetrics metrics) cached;
            cached.paint = null;
            cached.metrics = default;

            if (textPaint.IsTextPaintDirty() || !_textPaintCache.TryGetValue(textPaint, out cached))
            {
                GetSKTypeface(textPaint.Typeface, out var typeface);
                cached.paint = SkiaHelper.ToSKPaintText(textPaint, _pathEffectDisposable.Disposables);
                cached.paint.Typeface = typeface;
                cached.paint.TextEncoding = SKTextEncoding.Utf16;
                cached.paint.TextSize = (float)textPaint.FontSize;
                switch (textPaint.HAlign)
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
                _textPaintCache[textPaint] = cached;
            }
            else
            {
                SkiaHelper.ToSKPaintTextUpdate(cached.paint, textPaint, _pathEffectDisposable.Disposables);
            }

            paint = cached.paint;
            metrics = cached.metrics;
        }

        private void GetSKPicture(string path, out SKPicture picture)
        {
            if (!_pictureCache.TryGetValue(path, out picture))
            {
                picture = SkiaHelper.ToSKPicture(path);
                _pictureCache[path] = picture;
            }
        }

        private void DrawText(SKCanvas canvas, Text text, SKRect rect, IShapeStyle shapeStyle, double dx, double dy, double scale)
        {
            if (shapeStyle?.TextPaint == null)
            {
                return;
            }
            GetSKPaintText(shapeStyle.TextPaint, out var paint, out var metrics);
#if false
            var mBaseline = 0.0f;
            var mTop = metrics.Top;
            var mBottom = metrics.Bottom;
            var mLeading = metrics.Leading;
            var mCapHeight = metrics.CapHeight;
            var mLineHeight = metrics.Bottom - metrics.Top;
            var mXMax = metrics.XMax;
            var mXMin = metrics.XMin;
#endif
            var mAscent = metrics.Ascent;
            var mDescent = metrics.Descent;
            float x = rect.Left;
            float y = rect.Top;
            float width = rect.Width;
            float height = rect.Height;

            switch (shapeStyle.TextPaint.VAlign)
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
#if false
            double strokeWidth = 2.0;
            if (style.StrokePaint != null)
            {
                strokeWidth = style.StrokePaint.StrokeWidth;
            }
            using (var boundsPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(strokeWidth / scale), Color = new SKColor(255, 255, 255, 255) })
            using (var mTopPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(strokeWidth / scale), Color = new SKColor(128, 0, 128, 255) })
            using (var mAscentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(strokeWidth / scale), Color = new SKColor(0, 255, 0, 255) })
            using (var mBaselinePen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(strokeWidth / scale), Color = new SKColor(255, 0, 0, 255) })
            using (var mDescentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(strokeWidth / scale), Color = new SKColor(0, 0, 255, 255) })
            using (var mBottomPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(strokeWidth / scale), Color = new SKColor(255, 127, 0, 255) })
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
            switch (shapeStyle.TextPaint.HAlign)
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
#if false
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

        private void DrawTextOnPath(SKCanvas canvas, SKPath path, Text text, ITextPaint textPaint)
        {
            GetSKPaintText(textPaint, out var paint, out var metrics);
            //var bounds = new SKRect();
            //float baseTextWidth = paint.MeasureText(text.Value, ref bounds);
            //SKPathMeasure pathMeasure = new SKPathMeasure(path, false, 1);
            //float hOffset = (pathMeasure.Length / 2f) - (baseTextWidth / 2f);
            //float vOffset = metrics.Bottom - metrics.Top;
            float hOffset = 0.0f;
            float vOffset = 0.0f;
            canvas.DrawTextOnPath(text.Value, path, hOffset, vOffset, paint);
        }

        private void DrawStrokedPath(SKCanvas canvas, SKPath path, IStrokePaint strokePaint, double scale)
        {
            GetSKPaintStroke(strokePaint, out var pen, scale);
            bool isClipPath = SkiaHelper.GetInflateOffset(strokePaint.PathEffect, out var inflateX, out var inflateY);
            if (isClipPath)
            {
                var bounds = path.Bounds;
                var rect = SKRect.Inflate(bounds, (float)inflateX, (float)inflateY);
                canvas.Save();
                canvas.ClipPath(path);
                canvas.DrawRect(rect, pen);
                canvas.Restore();
            }
            else
            {
                canvas.DrawPath(path, pen);
            }
        }

        private void DrawFilledPath(SKCanvas canvas, SKPath path, IFillPaint fillPaint, double scale)
        {
            GetSKPaintFill(fillPaint, out var brush);
            bool isClipPath = SkiaHelper.GetInflateOffset(fillPaint.PathEffect, out var inflateX, out var inflateY);
            if (isClipPath)
            {
                var bounds = path.Bounds;
                var rect = SKRect.Inflate(bounds, (float)inflateX, (float)inflateY);
                canvas.Save();
                canvas.ClipPath(path);
                canvas.DrawRect(rect, brush);
                canvas.Restore();
            }
            else
            {
                canvas.DrawPath(path, brush);
            }
        }

        public void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddLine(null, line, dx, dy, geometry);
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(line.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, line.Text, style.TextPaint);
                    }
                }
            }
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddCubic(null, cubicBezier, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(cubicBezier.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, cubicBezier.Text, style.TextPaint);
                    }
                }
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddQuad(null, quadraticBezier, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(quadraticBezier.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, quadraticBezier.Text, style.TextPaint);
                    }
                }
            }
        }

        public void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddConic(null, conic, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(conic.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, conic.Text, style.TextPaint);
                    }
                }
            }
        }

        public void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SkiaHelper.ToSKPathFillType(path.FillType) })
                {
                    SkiaHelper.AddPath(null, path, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(path.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, path.Text, style.TextPaint);
                    }
                }
            }
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddRect(null, rectangle, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(rectangle.Text?.Value))
                    {
                        var rect = SkiaHelper.ToSKRect(rectangle.StartPoint, rectangle.Point, dx, dy);
                        DrawText(canvas, rectangle.Text, rect, style, dx, dy, scale);
                    }
                }
            }
        }

        public void DrawCircle(object dc, CircleShape circle, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddCircle(null, circle, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(circle.Text?.Value))
                    {
                        var distance = circle.StartPoint.DistanceTo(circle.Point);
                        var rect = SkiaHelper.ToSKRect(circle.StartPoint, distance, dx, dy);
                        DrawText(canvas, circle.Text, rect, style, dx, dy, scale);
                    }
                }
            }
        }

        public void DrawArc(object dc, ArcShape arc, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddArc(null, arc, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(arc.Text?.Value))
                    {
                        var distance = arc.StartPoint.DistanceTo(arc.Point);
                        var rect = SkiaHelper.ToSKRect(arc.StartPoint, distance, dx, dy);
                        DrawText(canvas, arc.Text, rect, style, dx, dy, scale);
                    }
                }
            }
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.IsText)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddOval(null, ellipse, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        DrawFilledPath(canvas, geometry, style.FillPaint, scale);
                    }
                    if (style.IsStroked)
                    {
                        DrawStrokedPath(canvas, geometry, style.StrokePaint, scale);
                    }
                    if (style.IsText && !string.IsNullOrEmpty(ellipse.Text?.Value))
                    {
                        var rect = SkiaHelper.ToSKRect(ellipse.StartPoint, ellipse.Point, dx, dy);
                        DrawText(canvas, ellipse.Text, rect, style, dx, dy, scale);
                    }
                }
            }
        }

        public void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsText && !string.IsNullOrEmpty(text.Text?.Value))
            {
                var canvas = dc as SKCanvas;
                var rect = SkiaHelper.ToSKRect(text.StartPoint, text.Point, dx, dy);
                DrawText(canvas, text.Text, rect, style, dx, dy, scale);
            }
        }

        public void DrawImage(object dc, ImageShape image, string styleId, double dx, double dy, double scale)
        {
            var canvas = dc as SKCanvas;
            var rect = SkiaHelper.ToSKRect(image.StartPoint, image.Point, dx, dy);
            if (!string.IsNullOrEmpty(image.Path))
            {
                GetSKPicture(image.Path, out var picture);
                int count = canvas.Save();
                SkiaHelper.GetStretchModeTransform(image.StretchMode, rect, picture.CullRect, out var ox, out var oy, out var zx, out var zy);
                canvas.Translate((float)ox, (float)oy);
                canvas.Scale((float)zx, (float)zy);
                canvas.DrawPicture(picture);
                canvas.RestoreToCount(count);
            }
            var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
            if (style != null)
            {
                if (style.IsText && !string.IsNullOrEmpty(image.Text?.Value))
                {
                    DrawText(canvas, image.Text, rect, style, dx, dy, scale);
                }
            }
        }
    }
}
