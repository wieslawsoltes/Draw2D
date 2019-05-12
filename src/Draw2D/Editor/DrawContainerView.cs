// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
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
            // TODO: Properly dispose SKPaint objects.
            _fillSKPaintCache = new Dictionary<ArgbColor, SKPaint>();
            // TODO: Properly dispose Brush objects.
            _fillBrushCache = new Dictionary<ArgbColor, Brush>();
        }

        private void DrawShapesCurrent(IContainerView view, object context, IShapeRenderer renderer)
        {
            view.CurrentContainer.Draw(context, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
        }

        private void DrawShapesWorking(IContainerView view, object context, IShapeRenderer renderer)
        {
            view.WorkingContainer.Draw(context, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
        }

        private void DrawPoints(IContainerView view, object context, IShapeRenderer renderer)
        {
            // NOTE: Drawing only selected points.
            //view.CurrentContainer.Draw(context, renderer, 0.0, 0.0, DrawMode.Point, null, null);
            //view.WorkingContainer.Draw(context, renderer, 0.0, 0.0, DrawMode.Point, null, null);

            var selected = view.Selection.Selected.ToList();

            foreach (var shape in selected)
            {
                if (shape is PointShape point)
                {
                    point.Draw(context, renderer, 0.0, 0.0, DrawMode.Point, null, null);
                }
            }
        }

        private void DrawDecorators(IContainerView view, object context, IShapeRenderer renderer)
        {
            var selected = view.Selection.Selected.ToList();

            foreach (var shape in selected)
            {
                if (Decorators.TryGetValue(shape.GetType(), out var helper))
                {
                    helper.Draw(context, shape, renderer, view.Selection, 0.0, 0.0, DrawMode.Shape);
                }
            }
        }

        private SKPicture RecordPicture(IContainerView view, double scale, IShapeRenderer renderer, Action<IContainerView, object, IShapeRenderer> draw)
        {
            renderer.Scale = scale;
            renderer.Selection = view.Selection;

            var recorder = new SKPictureRecorder();
            var rect = new SKRect(0f, 0f, (float)view.CurrentContainer.Width, (float)view.CurrentContainer.Height);
            var canvas = recorder.BeginRecording(rect);

            draw(view, canvas, renderer);

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

        private bool IsShapeStyleDirty(ShapeStyle style)
        {
            if (style.IsDirty
             || style.Stroke.IsDirty
             || style.Fill.IsDirty
             || style.TextStyle.IsDirty
             || style.TextStyle.Stroke.IsDirty)
            {
                return true;
            }
            return false;
        }

        private bool IsCanvasContainerDirty(CanvasContainer canvasContainer)
        {
            if (canvasContainer.Guides != null)
            {
                foreach (var guide in canvasContainer.Guides)
                {
                    if (guide.IsDirty || IsShapeStyleDirty(guide.Style))
                    {
                        return true;
                    }
                }
            }

            if (canvasContainer.Shapes != null)
            {
                foreach (var shape in canvasContainer.Shapes)
                {
                    if (shape.IsDirty || IsShapeStyleDirty(shape.Style))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPointsDirty(CanvasContainer canvasContainer)
        {
            var points = new List<PointShape>();
            canvasContainer.GetPoints(points);

            foreach (var point in points)
            {
                point.IsDirty = false;
            }

            return false;
        }

        private double _previousZX = double.NaN;
        private double _previousZY = double.NaN;
        private ISet<BaseShape> _previousSelected = null; // TODO: Dispose previous selected.
        private SKPicture _pictureShapesCurrent = null;
        private SKPicture _pictureShapesWorking = null;
        private SKPicture _pictureDecorators = null;
        private SKPicture _picturePoints = null;

        private void DrawSkia(IContainerView view, SKCanvas canvas, double width, double height, double dx, double dy, double zx, double zy)
        {
            bool isCurrentContainerDirty = IsCanvasContainerDirty(view.CurrentContainer);
            bool isWorkingContainerDirty = IsCanvasContainerDirty(view.WorkingContainer);
            bool isPointsCurrentContainerDirty = IsPointsDirty(view.CurrentContainer);
            bool isPointsWorkingContainerDirty = IsPointsDirty(view.WorkingContainer);

            bool isShapesCurrentDirty = isCurrentContainerDirty == true || isPointsCurrentContainerDirty == true || _previousZX != zx || _previousZY != zy;

            if (_pictureShapesCurrent == null || isShapesCurrentDirty == true)
            {
                view.CurrentContainer.Invalidate();

                if (_pictureShapesCurrent != null)
                {
                    _pictureShapesCurrent.Dispose();
                }

                _pictureShapesCurrent = RecordPicture(view, zx, _skiaRenderer, DrawShapesCurrent);
            }

            bool isShapesWorkingDirty = isWorkingContainerDirty == true || isPointsWorkingContainerDirty == true || _previousZX != zx || _previousZY != zy;

            if (_pictureShapesWorking == null || isShapesWorkingDirty == true)
            {
                view.WorkingContainer.Invalidate();

                if (_pictureShapesWorking != null)
                {
                    _pictureShapesWorking.Dispose();
                }

                _pictureShapesWorking = RecordPicture(view, zx, _skiaRenderer, DrawShapesWorking);
            }

            bool isSelectionDirty = view.Selection.Selected != _previousSelected || isShapesCurrentDirty == true || isShapesWorkingDirty == true;

            if (_pictureDecorators == null || isSelectionDirty == true)
            {
                if (_pictureDecorators != null)
                {
                    _pictureDecorators.Dispose();
                }

                _pictureDecorators = RecordPicture(view, zx, _skiaRenderer, DrawDecorators);
            }

            if (_picturePoints == null || isSelectionDirty == true)
            {
                if (_picturePoints != null)
                {
                    _picturePoints.Dispose();
                }

                _picturePoints = RecordPicture(view, zx, _skiaRenderer, DrawPoints);
            }

            _previousZX = zx;
            _previousZY = zy;
            _previousSelected = view.Selection.Selected;

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

            DrawPicture(canvas, _pictureShapesCurrent, dx, dy, zx, zy);
            DrawPicture(canvas, _pictureShapesWorking, dx, dy, zx, zy);
            DrawPicture(canvas, _pictureDecorators, dx, dy, zx, zy);
            DrawPicture(canvas, _picturePoints, dx, dy, zx, zy);

            // TODO: Dispose cached picture.
            //_picturePoints.Dispose();
            //_picturePoints = null;

            // TODO: Dispose cached picture.
            //_pictureDecorators.Dispose();
            //_pictureDecorators = null;

             // TODO: Dispose cached picture.
            //_pictureShapesWorking.Dispose();
            //_pictureShapesWorking = null;

            // TODO: Dispose cached picture.
            //_pictureShapesCurrent.Dispose();
            //_pictureShapesCurrent = null;
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

            DrawShapesCurrent(view, context, _avaloniaRenderer);
            DrawShapesWorking(view, context, _avaloniaRenderer);
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

        public void Dispose()
        {
            _previousZX = double.NaN;
            _previousZY = double.NaN;
            _previousSelected = null;

            if (_skiaRenderer != null)
            {
                _skiaRenderer.Dispose();
            }

            if (_avaloniaRenderer != null)
            {
                _avaloniaRenderer.Dispose();
            }

            if (_fillSKPaintCache != null)
            {
                foreach (var cache in _fillSKPaintCache)
                {
                    cache.Value.Dispose();
                }
                _fillSKPaintCache = null;
            }

            if (_picturePoints != null)
            {
                _picturePoints.Dispose();
                _picturePoints = null;
            }

            if (_pictureDecorators != null)
            {
                _pictureDecorators.Dispose();
                _pictureDecorators = null;
            }

            if (_pictureShapesWorking != null)
            {
                _pictureShapesWorking.Dispose();
                _pictureShapesWorking = null;
            }

            if (_pictureShapesCurrent != null)
            {
                _pictureShapesCurrent.Dispose();
                _pictureShapesCurrent = null;
            }
        }
    }
}
