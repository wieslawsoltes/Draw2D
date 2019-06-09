// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Presenters
{
    public class EditorContainerPresenter : IContainerPresenter
    {
        private readonly IToolContext _context;
        private readonly IContainerView _view;
        private SkiaShapeRenderer _skiaRenderer;
        private Dictionary<ArgbColor, SKPaint> _paintCache;
        private double _previousZX = double.NaN;
        private double _previousZY = double.NaN;
        private SKPicture _pictureShapesCurrent = null;
        private SKPicture _pictureShapesWorking = null;
        private SKPicture _pictureDecorators = null;
        private SKPicture _picturePoints = null;
        private bool _enablePictureCache = true;

        public EditorContainerPresenter(IToolContext context, IContainerView view)
        {
            _context = context;
            _view = view;
            _skiaRenderer = new SkiaShapeRenderer(_context, _view.SelectionState);
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
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsShapeStyleDirty: true");
#endif
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
#if USE_DEBUG_DIRTY
                Log.WriteLine($"styleLibrary.IsDirty: true");
#endif
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
#if USE_DEBUG_DIRTY
                Log.WriteLine($"canvasContainer.IsDirty: true");
#endif
                return true;
            }

            if (canvasContainer.Shapes != null)
            {
                foreach (var shape in canvasContainer.Shapes)
                {
                    if (shape.IsDirty)
                    {
#if USE_DEBUG_DIRTY
                        Log.WriteLine($"shape.IsDirty: true");
#endif
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
#if USE_DEBUG_DIRTY
                    Log.WriteLine($"point.IsDirty: true");
#endif
                    return true;
                }

                if (point.Template != null)
                {
                    if (point.Template.IsDirty)
                    {
#if USE_DEBUG_DIRTY
                        Log.WriteLine($"point.Template.IsDirty: true");
#endif
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

        private void DrawShapesCurrent(IContainerView view, object context, IShapeRenderer renderer, double scale)
        {
            view.CurrentContainer.Draw(context, renderer, 0.0, 0.0, scale, null, null);
        }

        private void DrawShapesWorking(IContainerView view, object context, IShapeRenderer renderer, double scale)
        {
            view.WorkingContainer.Draw(context, renderer, 0.0, 0.0, scale, null, null);
        }

        private void DrawPoints(IContainerView view, object context, IShapeRenderer renderer, double scale)
        {
            var selected = new List<IBaseShape>(view.SelectionState.Shapes);

            foreach (var shape in selected)
            {
                if (shape is IPointShape point)
                {
                    point.Draw(context, renderer, 0.0, 0.0, scale, null, null);
                }
            }
        }

        private void DrawDecorators(IContainerView view, object context, IShapeRenderer renderer, double scale)
        {
            var selected = new List<IBaseShape>(view.SelectionState.Shapes);

            foreach (var shape in selected)
            {
                shape.Decorator?.Draw(context, shape, renderer, view.SelectionState, 0.0, 0.0, scale);
            }
        }

        private SKPicture RecordPicture(IContainerView view, IShapeRenderer renderer, double scale, Action<IContainerView, object, IShapeRenderer, double> draw)
        {
            var recorder = new SKPictureRecorder();
            var rect = new SKRect(0f, 0f, (float)view.Width, (float)view.Height);
            var canvas = recorder.BeginRecording(rect);
            draw(view, canvas, renderer, scale);
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

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            bool isStyleLibraryDirty = IsStyleLibraryDirty(_context.StyleLibrary);
            bool isCurrentContainerDirty = IsCanvasContainerDirty(_view.CurrentContainer);
            bool isWorkingContainerDirty = IsCanvasContainerDirty(_view.WorkingContainer);
            bool isPointsCurrentContainerDirty = IsPointsDirty(_view.CurrentContainer);
            bool isPointsWorkingContainerDirty = IsPointsDirty(_view.WorkingContainer);
            bool isShapesCurrentDirty = isCurrentContainerDirty == true || isPointsCurrentContainerDirty == true || _previousZX != zx || _previousZY != zy;
            bool isShapesWorkingDirty = isWorkingContainerDirty == true || isPointsWorkingContainerDirty == true || _previousZX != zx || _previousZY != zy;

            if (_pictureShapesCurrent == null || isShapesCurrentDirty == true || isStyleLibraryDirty == true)
            {
                if (_pictureShapesCurrent != null)
                {
                    _pictureShapesCurrent.Dispose();
                }

                _pictureShapesCurrent = RecordPicture(_view, _skiaRenderer, zx, DrawShapesCurrent);
            }

            if (_pictureShapesWorking == null || isShapesWorkingDirty == true || isStyleLibraryDirty == true)
            {
                if (_pictureShapesWorking != null)
                {
                    _pictureShapesWorking.Dispose();
                }

                _pictureShapesWorking = RecordPicture(_view, _skiaRenderer, zx, DrawShapesWorking);
            }

            bool isSelectionDirty = _view.SelectionState.IsDirty == true || isShapesCurrentDirty == true || isShapesWorkingDirty == true;
#if USE_DEBUG_DIRTY
            Log.WriteLine(
                $"{nameof(isStyleLibraryDirty)}: {isStyleLibraryDirty}, " +
                $"{nameof(isShapesCurrentDirty)}: {isShapesCurrentDirty}, " +
                $"{nameof(isShapesWorkingDirty)}: {isShapesWorkingDirty}, " +
                $"{nameof(isCurrentContainerDirty)}: {isCurrentContainerDirty}, " +
                $"{nameof(isWorkingContainerDirty)}: {isWorkingContainerDirty}, " +
                $"{nameof(isSelectionDirty)}: {isSelectionDirty}");
#endif
            if (_view.SelectionState.IsDirty == true)
            {
                _view.SelectionState.Invalidate();
            }

            if (_pictureDecorators == null || isSelectionDirty == true || isStyleLibraryDirty == true)
            {
                if (_pictureDecorators != null)
                {
                    _pictureDecorators.Dispose();
                }

                _pictureDecorators = RecordPicture(_view, _skiaRenderer, zx, DrawDecorators);
            }

            if (_picturePoints == null || isSelectionDirty == true || isStyleLibraryDirty == true)
            {
                if (_picturePoints != null)
                {
                    _picturePoints.Dispose();
                }

                _picturePoints = RecordPicture(_view, _skiaRenderer, zx, DrawPoints);
            }

            _previousZX = zx;
            _previousZY = zy;

            var canvas = context as SKCanvas;

            if (_view.InputBackground != null)
            {
                canvas.Clear(SkiaHelper.ToSKColor(_view.InputBackground));
            }
            else
            {
                canvas.Clear();
            }

            if (_view.WorkBackground != null)
            {
                GetSKPaintFill(_view.WorkBackground, out var brush);
                canvas.Save();
                canvas.Translate((float)dx, (float)dy);
                canvas.Scale((float)zx, (float)zy);
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, _view.Width, _view.Height), brush);
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
                _view.CurrentContainer?.Invalidate();
            }

            if (isWorkingContainerDirty == true)
            {
                _view.WorkingContainer?.Invalidate();
            }

            if (isStyleLibraryDirty == true)
            {
                _context.StyleLibrary?.Invalidate();
            }
        }
    }
}
