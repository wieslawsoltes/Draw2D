// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define USE_DRAW_POINTS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.Editor.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Editor.Views
{
    public class EditorSkiaView : IDrawContainerView
    {
        private IToolContext _context;
        private bool _enablePictureCache = false;
        private SkiaShapeRenderer _skiaRenderer;
        private Dictionary<ArgbColor, SKPaint> _paintCache;
        private double _previousZX = double.NaN;
        private double _previousZY = double.NaN;
        private SKPicture _pictureShapesCurrent = null;
        private SKPicture _pictureShapesWorking = null;
        private SKPicture _pictureDecorators = null;
        private SKPicture _picturePoints = null;

        public EditorSkiaView(IToolContext context)
        {
            _context = context;
            _skiaRenderer = new SkiaShapeRenderer(_context);
            _paintCache = new Dictionary<ArgbColor, SKPaint>();
        }

        public void Dispose()
        {
            _previousZX = double.NaN;
            _previousZY = double.NaN;

            if (_skiaRenderer != null)
            {
                _skiaRenderer.Dispose();
            }

            if (_paintCache != null)
            {
                foreach (var cache in _paintCache)
                {
                    cache.Value.Dispose();
                }
                _paintCache = null;
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

        private bool IsShapeStyleDirty(ShapeStyle style)
        {
            if (style == null)
            {
                return false;
            }

            if (style.IsDirty
             || style.Stroke.IsDirty
             || style.Fill.IsDirty
             || style.TextStyle.IsDirty
             || style.TextStyle.Stroke.IsDirty)
            {
                Debug.WriteLine($"IsShapeStyleDirty: true");
                return true;
            }
            return false;
        }

        private bool IsStyleLibraryDirty(IStyleLibrary styleLibrary)
        {
            if (styleLibrary == null)
            {
                return false;
            }

            if (styleLibrary.IsDirty)
            {
                Debug.WriteLine($"styleLibrary.IsDirty: true");
                return true;
            }

            if (styleLibrary.Styles != null)
            {
                foreach (var style in styleLibrary.Styles)
                {
                    if (IsShapeStyleDirty(style))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsCanvasContainerDirty(ICanvasContainer canvasContainer)
        {
            if (canvasContainer == null)
            {
                return false;
            }

            if (canvasContainer.IsDirty)
            {
                Debug.WriteLine($"canvasContainer.IsDirty: true");
                return true;
            }

            if (canvasContainer.Shapes != null)
            {
                foreach (var shape in canvasContainer.Shapes)
                {
                    if (shape.IsDirty)
                    {
                        Debug.WriteLine($"shape.IsDirty: true");
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPointsDirty(ICanvasContainer canvasContainer)
        {
            if (canvasContainer == null)
            {
                return false;
            }

            var points = new List<IPointShape>();
            canvasContainer.GetPoints(points);

            foreach (var point in points)
            {
                if (point.IsDirty)
                {
                    Debug.WriteLine($"point.IsDirty: true");
                    return true;
                }

                if (point.Template != null)
                {
                    if (point.Template.IsDirty)
                    {
                        Debug.WriteLine($"point.Template.IsDirty: true");
                        return true;
                    }
                }
            }

            return false;
        }

        private void GetSKPaintFill(ArgbColor color, out SKPaint brush)
        {
            if (color.IsDirty == true || !_paintCache.TryGetValue(color, out var brushCached))
            {
                color.Invalidate();
                brushCached = SkiaHelper.ToSKPaintBrush(color);
                _paintCache[color] = brushCached;
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(brushCached, color);
            }

            brush = brushCached;
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
#if USE_DRAW_POINTS
            view.CurrentContainer.Draw(context, renderer, 0.0, 0.0, DrawMode.Point, null, null);
            view.WorkingContainer.Draw(context, renderer, 0.0, 0.0, DrawMode.Point, null, null);
#else
            // NOTE: Draw only selected points.
            var selected = new List<IBaseShape>(view.SelectionState.Shapes);

            foreach (var shape in selected)
            {
                if (shape is IPointShape point)
                {
                    point.Draw(context, renderer, 0.0, 0.0, DrawMode.Point, null, null);
                }
            }
#endif
        }

        private void DrawDecorators(IContainerView view, object context, IShapeRenderer renderer)
        {
            var selected = new List<IBaseShape>(view.SelectionState.Shapes);

            foreach (var shape in selected)
            {
                shape.Decorator?.Draw(context, shape, renderer, view.SelectionState, 0.0, 0.0, DrawMode.Shape);
            }
        }

        private SKPicture RecordPicture(IContainerView view, IShapeRenderer renderer, Action<IContainerView, object, IShapeRenderer> draw)
        {
            var recorder = new SKPictureRecorder();
            var rect = new SKRect(0f, 0f, (float)view.Width, (float)view.Height);
            var canvas = recorder.BeginRecording(rect);
            draw(view, canvas, renderer);
            return recorder.EndRecording();
        }

        private void DrawPicture(SKCanvas canvas, SKPicture picture, double dx, double dy, double zx, double zy)
        {
            canvas.Save();
            canvas.Translate((float)dx, (float)dy);
            canvas.Scale((float)zx, (float)zy);
            canvas.DrawPicture(picture);
            canvas.Restore();
        }

        public void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            bool isStyleLibraryDirty = IsStyleLibraryDirty(_context.StyleLibrary);
            bool isCurrentContainerDirty = IsCanvasContainerDirty(view.CurrentContainer);
            bool isWorkingContainerDirty = IsCanvasContainerDirty(view.WorkingContainer);
            bool isPointsCurrentContainerDirty = IsPointsDirty(view.CurrentContainer);
            bool isPointsWorkingContainerDirty = IsPointsDirty(view.WorkingContainer);
            bool isShapesCurrentDirty = isCurrentContainerDirty == true || isPointsCurrentContainerDirty == true || _previousZX != zx || _previousZY != zy;
            bool isShapesWorkingDirty = isWorkingContainerDirty == true || isPointsWorkingContainerDirty == true || _previousZX != zx || _previousZY != zy;

            _skiaRenderer.Scale = zx;
            _skiaRenderer.SelectionState = view.SelectionState;

            if (_pictureShapesCurrent == null || isShapesCurrentDirty == true || isStyleLibraryDirty == true)
            {
                if (_pictureShapesCurrent != null)
                {
                    _pictureShapesCurrent.Dispose();
                }

                _pictureShapesCurrent = RecordPicture(view, _skiaRenderer, DrawShapesCurrent);
            }

            if (_pictureShapesWorking == null || isShapesWorkingDirty == true || isStyleLibraryDirty == true)
            {
                if (_pictureShapesWorking != null)
                {
                    _pictureShapesWorking.Dispose();
                }

                _pictureShapesWorking = RecordPicture(view, _skiaRenderer, DrawShapesWorking);
            }

            bool isSelectionDirty = view.SelectionState.IsDirty == true || isShapesCurrentDirty == true || isShapesWorkingDirty == true;

            Debug.WriteLine(
                $"{nameof(isStyleLibraryDirty)}: {isStyleLibraryDirty}, " +
                $"{nameof(isShapesCurrentDirty)}: {isShapesCurrentDirty}, " +
                $"{nameof(isShapesWorkingDirty)}: {isShapesWorkingDirty}, " +
                $"{nameof(isCurrentContainerDirty)}: {isCurrentContainerDirty}, " +
                $"{nameof(isWorkingContainerDirty)}: {isWorkingContainerDirty}, " +
                $"{nameof(isSelectionDirty)}: {isSelectionDirty}");

            if (view.SelectionState.IsDirty == true)
            {
                view.SelectionState.Invalidate();
            }

            if (_pictureDecorators == null || isSelectionDirty == true || isStyleLibraryDirty == true)
            {
                if (_pictureDecorators != null)
                {
                    _pictureDecorators.Dispose();
                }

                _pictureDecorators = RecordPicture(view, _skiaRenderer, DrawDecorators);
            }

            if (_picturePoints == null || isSelectionDirty == true || isStyleLibraryDirty == true)
            {
                if (_picturePoints != null)
                {
                    _picturePoints.Dispose();
                }

                _picturePoints = RecordPicture(view, _skiaRenderer, DrawPoints);
            }

            _previousZX = zx;
            _previousZY = zy;

            var canvas = context as SKCanvas;

            if (view.InputBackground != null)
            {
                canvas.Clear(SkiaHelper.ToSKColor(view.InputBackground));
            }
            else
            {
                canvas.Clear();
            }

            if (view.WorkBackground != null)
            {
                GetSKPaintFill(view.WorkBackground, out var brush);
                canvas.Save();
                canvas.Translate((float)dx, (float)dy);
                canvas.Scale((float)zx, (float)zy);
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, view.Width, view.Height), brush);
                canvas.Restore();
            }

            DrawPicture(canvas, _pictureShapesCurrent, dx, dy, zx, zy);
            DrawPicture(canvas, _pictureShapesWorking, dx, dy, zx, zy);
            DrawPicture(canvas, _pictureDecorators, dx, dy, zx, zy);
            DrawPicture(canvas, _picturePoints, dx, dy, zx, zy);

            if (_enablePictureCache == false)
            {
                _picturePoints.Dispose();
                _picturePoints = null;

                _pictureDecorators.Dispose();
                _pictureDecorators = null;

                _pictureShapesWorking.Dispose();
                _pictureShapesWorking = null;

                _pictureShapesCurrent.Dispose();
                _pictureShapesCurrent = null;
            }

            if (isCurrentContainerDirty == true)
            {
                view.CurrentContainer?.Invalidate();
            }

            if (isWorkingContainerDirty == true)
            {
                view.WorkingContainer?.Invalidate();
            }

            if (isStyleLibraryDirty == true)
            {
                _context.StyleLibrary?.Invalidate();
            }
        }
    }
}
