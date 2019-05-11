// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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
        private readonly SkiaShapeRenderer _skiaRenderer;
        private readonly AvaloniaShapeRenderer _avaloniaRenderer;
        private readonly Dictionary<ArgbColor, SKPaint> _fillSKPaintCache;
        private readonly Dictionary<ArgbColor, Brush> _fillBrushCache;

        public IDictionary<Type, IShapeDecorator> Decorators { get; set; }

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

        private void DrawContainer(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            container.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        private void DrawDecorators(object dc, ISelection selection, IShapeRenderer renderer, double dx, double dy, DrawMode mode)
        {
            foreach (var shape in selection.Selected)
            {
                if (Decorators.TryGetValue(shape.GetType(), out var helper))
                {
                    helper.Draw(dc, shape, renderer, selection, dx, dy, mode);
                }
            }
        }

        private void DrawShapes(IContainerView view, object context, IShapeRenderer renderer)
        {

            DrawContainer(context, view.CurrentContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            DrawContainer(context, view.WorkingContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
        }

        private void DrawDecorators(IContainerView view, object context, IShapeRenderer renderer)
        {
            DrawDecorators(context, view.Selection, renderer, 0.0, 0.0, DrawMode.Shape);
        }

        private void DrawPoints(IContainerView view, object context, IShapeRenderer renderer)
        {
            DrawContainer(context, view.CurrentContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);
            DrawContainer(context, view.WorkingContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);
        }

        private SKPicture RecordPicture(IContainerView view, double scale, Action<IContainerView, object, IShapeRenderer> draw)
        {
            _skiaRenderer.Scale = scale;
            _skiaRenderer.Selection = view.Selection;

            var recorder = new SKPictureRecorder();
            var rect = new SKRect(0f, 0f, (float)view.CurrentContainer.Width, (float)view.CurrentContainer.Height);
            var canvas = recorder.BeginRecording(rect);

            draw(view, canvas, _skiaRenderer);

            var picture = recorder.EndRecording();

            canvas.Dispose();

            return picture;
        }

        private void DrawPicture(SKCanvas canvas, SKPicture picture, double dx, double dy, double zx, double zy)
        {
            canvas.Save();
            canvas.Translate((float)dx, (float)dy);
            canvas.Scale((float)zx, (float)zy);
            canvas.DrawPicture(picture);
            canvas.Restore();
        }

        private void DrawSkia(IContainerView view, SKCanvas canvas, double width, double height, double dx, double dy, double zx, double zy)
        {
            view.CurrentContainer.Invalidate();
            view.WorkingContainer.Invalidate();

            var pictureShapes = RecordPicture(view, zx, DrawShapes);
            var pictureDecorators = RecordPicture(view, zx, DrawDecorators);
            var picturePoints = RecordPicture(view, zx, DrawPoints);

            if (view.CurrentContainer.InputBackground != null)
            {
                GetSKPaintFill(view.CurrentContainer.InputBackground, out var brush);
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, width, height), brush);
            }

            if (view.CurrentContainer.WorkBackground != null)
            {
                GetSKPaintFill(view.CurrentContainer.WorkBackground, out var brush);
                canvas.Save();
                canvas.Translate((float)dx, (float)dy);
                canvas.Scale((float)zx, (float)zy);
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, view.CurrentContainer.Width, view.CurrentContainer.Height), brush);
                canvas.Restore();
            }

            DrawPicture(canvas, pictureShapes, dx, dy, zx, zy);
            DrawPicture(canvas, pictureDecorators, dx, dy, zx, zy);
            DrawPicture(canvas, picturePoints, dx, dy, zx, zy);

            picturePoints.Dispose();
            pictureDecorators.Dispose();
            pictureShapes.Dispose();
        }

        private void DrawAvalonia(IContainerView view, DrawingContext context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _avaloniaRenderer.Scale = zx;
            _avaloniaRenderer.Selection = view.Selection;

            view.CurrentContainer.Invalidate();
            view.WorkingContainer.Invalidate();

            if (view.CurrentContainer.InputBackground != null)
            {
                GetBrushFill(view.CurrentContainer.InputBackground, out var brush);
                context.FillRectangle(brush, new Rect(0.0, 0.0, width, height));
            }

            var state = context.PushPreTransform(new Matrix(zx, 0.0, 0.0, zy, dx, dy));

            if (view.CurrentContainer.WorkBackground != null)
            {
                GetBrushFill(view.CurrentContainer.WorkBackground, out var brush);
                context.FillRectangle(brush, new Rect(0.0, 0.0, view.CurrentContainer.Width, view.CurrentContainer.Height));
            }

            DrawShapes(view, context, _avaloniaRenderer);
            DrawDecorators(view, context, _avaloniaRenderer);
            DrawPoints(view, context, _avaloniaRenderer);

            state.Dispose();
        }

        public void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            switch (context)
            {
                case DrawingContext drawingContext:
                    {
                        DrawAvalonia(view, drawingContext, width, height, dx, dy, zx, zy);
                    }
                    break;
                case SKCanvas canvas:
                    {
                        DrawSkia(view, canvas, width, height, dx, dy, zx, zy);
                    }
                    break;
            }
        }
    }
}
