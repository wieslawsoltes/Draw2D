﻿using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Renderers;

public class SkiaShapeRenderer : IShapeRenderer
{
    private IToolContext _context;
    private IContainerView _view;
    private ISelectionState _selectionState;
    private Dictionary<Typeface, SKTypeface> _typefaceCache;
    private Dictionary<IPaint, (SKPaint paint, SKFontMetrics metrics)> _textPaintCache;
    private Dictionary<IPaint, SKPaint> _fillPaintCache;
    private Dictionary<IPaint, SKPaint> _strokePaintCache;
    private Dictionary<string, SKPicture> _pictureCache;
    private CompositeDisposable _disposable;

    public SkiaShapeRenderer(IToolContext context, IContainerView view, ISelectionState selectionState)
    {
        _context = context;
        _view = view;
        _selectionState = selectionState;
        _typefaceCache = new Dictionary<Typeface, SKTypeface>();
        _textPaintCache = new Dictionary<IPaint, (SKPaint paint, SKFontMetrics metrics)>();
        _fillPaintCache = new Dictionary<IPaint, SKPaint>();
        _strokePaintCache = new Dictionary<IPaint, SKPaint>();
        _pictureCache = new Dictionary<string, SKPicture>();
        _disposable = new CompositeDisposable();
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

        if (_disposable != null)
        {
            _disposable.Dispose();
            _disposable = null;
        }
    }

    private void GetSKTypeface(Typeface style, out SKTypeface typeface)
    {
        if (style.IsTreeDirty() || !_typefaceCache.TryGetValue(style, out typeface))
        {
            typeface = SkiaUtil.ToSKTypeface(style);
            _typefaceCache[style] = typeface;
        }
    }

    private void GetSKPaintFill(IPaint fillPaint, IPaintEffects effects, double scale, out SKPaint brush)
    {
        if (fillPaint.IsTreeDirty() || !_fillPaintCache.TryGetValue(fillPaint, out var brushCached))
        {
            brushCached = SkiaUtil.ToSKPaint(fillPaint, effects, scale, _disposable.Disposables);
            _fillPaintCache[fillPaint] = brushCached;
        }
        else
        {
            SkiaUtil.ToSKPaintUpdate(brushCached, fillPaint, effects, scale, _disposable.Disposables);
        }

        brush = brushCached;
    }

    private void GetSKPaintStroke(IPaint strokePaint, IPaintEffects effects, double scale, out SKPaint pen)
    {
        if (strokePaint.IsTreeDirty() || !_strokePaintCache.TryGetValue(strokePaint, out var penCached))
        {
            penCached = SkiaUtil.ToSKPaint(strokePaint, effects, scale, _disposable.Disposables);
            _strokePaintCache[strokePaint] = penCached;
        }
        else
        {
            SkiaUtil.ToSKPaintUpdate(penCached, strokePaint, effects, scale, _disposable.Disposables);
        }

        pen = penCached;
    }

    private void GetSKPaintText(IPaint textPaint, IPaintEffects effects, double scale, out SKPaint paint, out SKFontMetrics metrics)
    {
        (SKPaint paint, SKFontMetrics metrics) cached;
        cached.paint = null;
        cached.metrics = default;

        if (textPaint.IsTreeDirty() || !_textPaintCache.TryGetValue(textPaint, out cached))
        {
            GetSKTypeface(textPaint.Typeface, out var typeface);
            cached.paint = SkiaUtil.ToSKPaint(textPaint, effects, scale, _disposable.Disposables);
            cached.paint.Typeface = typeface;
            cached.metrics = cached.paint.FontMetrics;
            _textPaintCache[textPaint] = cached;
        }
        else
        {
            SkiaUtil.ToSKPaintUpdate(cached.paint, textPaint, effects, scale, _disposable.Disposables);
        }

        paint = cached.paint;
        metrics = cached.metrics;
    }

    private void GetSKPicture(string path, out SKPicture picture)
    {
        if (!_pictureCache.TryGetValue(path, out picture))
        {
            picture = SkiaUtil.ToSKPicture(path);
            _pictureCache[path] = picture;
        }
    }

    private void DrawText(SKCanvas canvas, Text text, SKRect rect, IShapeStyle shapeStyle, IPaintEffects effects, double dx, double dy, double scale)
    {
        if (shapeStyle?.TextPaint == null)
        {
            return;
        }
        GetSKPaintText(shapeStyle.TextPaint, effects, scale, out var paint, out var metrics);
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

    private void DrawTextOnPath(SKCanvas canvas, SKPath path, Text text, IPaint textPaint, IPaintEffects effects, double scale)
    {
        GetSKPaintText(textPaint, effects, scale, out var paint, out var metrics);
        //var bounds = new SKRect();
        //float baseTextWidth = paint.MeasureText(text.Value, ref bounds);
        //SKPathMeasure pathMeasure = new SKPathMeasure(path, false, 1);
        //float hOffset = (pathMeasure.Length / 2f) - (baseTextWidth / 2f);
        //float vOffset = metrics.Bottom - metrics.Top;
        float hOffset = 0.0f;
        float vOffset = 0.0f;
        canvas.DrawTextOnPath(text.Value, path, hOffset, vOffset, paint);
    }

    private void DrawStrokedPath(SKCanvas canvas, SKPath path, IPaint strokePaint, IPaintEffects effects, double scale)
    {
        GetSKPaintStroke(strokePaint, effects, scale, out var pen);
        bool isClipPath = SkiaUtil.GetInflateOffset(effects?.PathEffect != null ? effects.PathEffect : strokePaint.Effects?.PathEffect, out var inflateX, out var inflateY);
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

    private void DrawFilledPath(SKCanvas canvas, SKPath path, IPaint fillPaint, IPaintEffects effects, double scale)
    {
        GetSKPaintFill(fillPaint, effects, scale, out var brush);
        bool isClipPath = SkiaUtil.GetInflateOffset(effects?.PathEffect != null ? effects.PathEffect : fillPaint.Effects?.PathEffect, out var inflateX, out var inflateY);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddLine(null, line, dx, dy, geometry);
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, line.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(line.Text?.Value))
            {
                DrawTextOnPath(canvas, geometry, line.Text, style.TextPaint, line.Effects, scale);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddCubic(null, cubicBezier, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, cubicBezier.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, cubicBezier.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(cubicBezier.Text?.Value))
            {
                DrawTextOnPath(canvas, geometry, cubicBezier.Text, style.TextPaint, cubicBezier.Effects, scale);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddQuad(null, quadraticBezier, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, quadraticBezier.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, quadraticBezier.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(quadraticBezier.Text?.Value))
            {
                DrawTextOnPath(canvas, geometry, quadraticBezier.Text, style.TextPaint, quadraticBezier.Effects, scale);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddConic(null, conic, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, conic.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, conic.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(conic.Text?.Value))
            {
                DrawTextOnPath(canvas, geometry, conic.Text, style.TextPaint, conic.Effects, scale);
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
            using var geometry = new SKPath() { FillType = SkiaUtil.ToSKPathFillType(path.FillType) };
            SkiaUtil.AddPath(null, path, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, path.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, path.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(path.Text?.Value))
            {
                DrawTextOnPath(canvas, geometry, path.Text, style.TextPaint, path.Effects, scale);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddRect(null, rectangle, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, rectangle.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, rectangle.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(rectangle.Text?.Value))
            {
                var rect = SkiaUtil.ToSKRect(rectangle.StartPoint, rectangle.Point, dx, dy);
                DrawText(canvas, rectangle.Text, rect, style, rectangle.Effects, dx, dy, scale);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddCircle(null, circle, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, circle.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, circle.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(circle.Text?.Value))
            {
                var distance = circle.StartPoint.DistanceTo(circle.Point);
                var rect = SkiaUtil.ToSKRect(circle.StartPoint, distance, dx, dy);
                DrawText(canvas, circle.Text, rect, style, circle.Effects, dx, dy, scale);
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
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddArc(null, arc, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, arc.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, arc.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(arc.Text?.Value))
            {
                var distance = arc.StartPoint.DistanceTo(arc.Point);
                var rect = SkiaUtil.ToSKRect(arc.StartPoint, distance, dx, dy);
                DrawText(canvas, arc.Text, rect, style, arc.Effects, dx, dy, scale);
            }
        }
    }

    public void DrawOval(object dc, OvalShape oval, string styleId, double dx, double dy, double scale)
    {
        var style = _context?.DocumentContainer?.StyleLibrary?.Get(styleId);
        if (style == null)
        {
            return;
        }
        if (style.IsFilled || style.IsStroked || style.IsText)
        {
            var canvas = dc as SKCanvas;
            using var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaUtil.AddOval(null, oval, dx, dy, geometry);
            if (style.IsFilled)
            {
                DrawFilledPath(canvas, geometry, style.FillPaint, oval.Effects, scale);
            }
            if (style.IsStroked)
            {
                DrawStrokedPath(canvas, geometry, style.StrokePaint, oval.Effects, scale);
            }
            if (style.IsText && !string.IsNullOrEmpty(oval.Text?.Value))
            {
                var rect = SkiaUtil.ToSKRect(oval.StartPoint, oval.Point, dx, dy);
                DrawText(canvas, oval.Text, rect, style, oval.Effects, dx, dy, scale);
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
            var rect = SkiaUtil.ToSKRect(text.StartPoint, text.Point, dx, dy);
            DrawText(canvas, text.Text, rect, style, text.Effects, dx, dy, scale);
        }
    }

    public void DrawImage(object dc, ImageShape image, string styleId, double dx, double dy, double scale)
    {
        var canvas = dc as SKCanvas;
        var rect = SkiaUtil.ToSKRect(image.StartPoint, image.Point, dx, dy);
        if (!string.IsNullOrEmpty(image.Path))
        {
            GetSKPicture(image.Path, out var picture);
            int count = canvas.Save();
            SkiaUtil.GetStretchModeTransform(image.StretchMode, rect, picture.CullRect, out var ox, out var oy, out var zx, out var zy);
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
                DrawText(canvas, image.Text, rect, style, image.Effects, dx, dy, scale);
            }
        }
    }
}