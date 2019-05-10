// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Editor
{
    public class DrawContainerView : IDrawContainerView
    {
        private SkiaShapeRenderer _skiaRenderer;
        private AvaloniaShapeRenderer _avaloniaRenderer;
        private Dictionary<ArgbColor, SKPaint> _fillSKPaintCache;
        private Dictionary<ArgbColor, Brush> _fillBrushCache;

        private void GetSKPaintFill(ArgbColor color, out SKPaint brush)
        {
            if (color.IsDirty == true || !_fillSKPaintCache.TryGetValue(color, out var brushCached))
            {
                color.Invalidate();
                brushCached = SkiaHelper.ToSKPaintBrush(color);
                _fillSKPaintCache[color] = brushCached;
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(brushCached, color);
            }

            brush = brushCached;
        }

        private void GetBrushFill(ArgbColor color, out Brush brush)
        {
            if (color.IsDirty == true || !_fillBrushCache.TryGetValue(color, out var brushCached))
            {
                color.Invalidate();
                brushCached = new SolidColorBrush(AvaloniaHelper.ToColor(color));
                _fillBrushCache[color] = brushCached;
            }

            brush = brushCached;
        }

        public DrawContainerView()
        {
            _skiaRenderer = new SkiaShapeRenderer();
            _avaloniaRenderer = new AvaloniaShapeRenderer();
            // FIXME: Properly dispose SKPaint objects.
            _fillSKPaintCache = new Dictionary<ArgbColor, SKPaint>();
            // FIXME: Properly dispose Brush objects.
            _fillBrushCache = new Dictionary<ArgbColor, Brush>();
        }

        private void Draw(IContainerView view, object context, IShapeRenderer renderer)
        {
            view.Presenter.DrawContainer(context, view.CurrentContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            view.Presenter.DrawContainer(context, view.WorkingContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);

            view.Presenter.DrawDecorators(context, view.CurrentContainer, renderer, 0.0, 0.0, DrawMode.Shape);
            view.Presenter.DrawDecorators(context, view.WorkingContainer, renderer, 0.0, 0.0, DrawMode.Shape);

            view.Presenter.DrawContainer(context, view.CurrentContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);
            view.Presenter.DrawContainer(context, view.WorkingContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);
        }

        private void Draw(IContainerView view, DrawingContext context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _avaloniaRenderer.Scale = zx;
            _avaloniaRenderer.Selection = view.Selection;

            if (view.CurrentContainer.InputBackground != null)
            {
                GetBrushFill(view.CurrentContainer.InputBackground, out var brush);
                context.FillRectangle(brush, new Rect(0, 0, width, height));
            }

            var state = context.PushPreTransform(new Matrix(zx, 0.0, 0.0, zy, dx, dy));

            if (view.CurrentContainer.WorkBackground != null)
            {
                GetBrushFill(view.CurrentContainer.WorkBackground, out var brush);
                context.FillRectangle(brush, new Rect(0.0, 0.0, view.CurrentContainer.Width, view.CurrentContainer.Height));
            }

            Draw(view, context, _avaloniaRenderer);

            state.Dispose();
        }

        private SKPicture Record(IContainerView view, double scale)
        {
            var recorder = new SKPictureRecorder();
            var rect = new SKRect(0f, 0f, (float)view.CurrentContainer.Width, (float)view.CurrentContainer.Height);

            var canvas = recorder.BeginRecording(rect);

            _skiaRenderer.Scale = scale;
            _skiaRenderer.Selection = view.Selection;

            if (view.CurrentContainer.WorkBackground != null)
            {
                GetSKPaintFill(view.CurrentContainer.WorkBackground, out var brush);
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, view.CurrentContainer.Width, view.CurrentContainer.Height), brush);
            }

            Draw(view, canvas, _skiaRenderer);

            var picture = recorder.EndRecording();

            canvas.Dispose();

            return picture;
        }

        private void Draw(SKCanvas canvas, SKPicture picture, double dx, double dy, double zx, double zy)
        {
            canvas.Save();
            canvas.Translate((float)dx, (float)dy);
            canvas.Scale((float)zx, (float)zy);
            canvas.DrawPicture(picture);
            canvas.Restore();
        }

        public void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            switch (context)
            {
                case DrawingContext drawingContext:
                    {
                        Draw(view, drawingContext, width, height, dx, dy, zx, zy);
                    }
                    break;
                case SKCanvas canvas:
                    {
                        var picture = Record(view, zx);

                        if (view.CurrentContainer.InputBackground != null)
                        {
                            GetSKPaintFill(view.CurrentContainer.InputBackground, out var brush);
                            canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, width, height), brush);
                        }

                        Draw(canvas, picture, dx, dy, zx, zy);

                        picture.Dispose();
                    }
                    break;
            }
        }
    }
}
