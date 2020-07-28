using System;
using System.Collections.Generic;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Presenters
{
    public class SkiaEditorContainerPresenter : IContainerPresenter
    {
        private readonly IToolContext _context;
        private readonly IContainerView _view;
        private SkiaShapeRenderer _skiaRenderer;
        private Dictionary<IPaint, SKPaint> _paintCache;
        private double _previousZX = double.NaN;
        private double _previousZY = double.NaN;
        private SKPicture _pictureShapesCurrent = null;
        private SKPicture _pictureShapesWorking = null;
        private SKPicture _pictureDecorators = null;
        private SKPicture _picturePoints = null;
        private bool _enablePictureCache = true;
        private CompositeDisposable _disposable;

        public SkiaEditorContainerPresenter(IToolContext context, IContainerView view)
        {
            _context = context;
            _view = view;
            _skiaRenderer = new SkiaShapeRenderer(_context, _view, _view.SelectionState);
            _paintCache = new Dictionary<IPaint, SKPaint>();
            _disposable = new CompositeDisposable();
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

            if (_disposable != null)
            {
                _disposable.Dispose();
                _disposable = null;
            }
        }

        private void GetSKPaintFill(IPaint fillPaint, IPaintEffects effects, double scale, out SKPaint brush)
        {
            if (fillPaint.IsTreeDirty() || !_paintCache.TryGetValue(fillPaint, out var brushCached))
            {
                fillPaint.Invalidate();
                brushCached = SkiaUtil.ToSKPaint(fillPaint, effects, scale, _disposable.Disposables);
                _paintCache[fillPaint] = brushCached;
            }
            else
            {
                SkiaUtil.ToSKPaintUpdate(brushCached, fillPaint, effects, scale, _disposable.Disposables);
            }

            brush = brushCached;
        }

        private void DrawShapesCurrent(IContainerView view, SKCanvas canvas, IShapeRenderer renderer, double scale)
        {
            view.CurrentContainer.Draw(canvas, renderer, 0.0, 0.0, scale, null, null);
        }

        private void DrawShapesWorking(IContainerView view, SKCanvas canvas, IShapeRenderer renderer, double scale)
        {
            view.WorkingContainer.Draw(canvas, renderer, 0.0, 0.0, scale, null, null);
        }

        private void DrawPoints(IContainerView view, SKCanvas canvas, IShapeRenderer renderer, double scale)
        {
            var selected = new List<IBaseShape>(view.SelectionState.Shapes);

            foreach (var shape in selected)
            {
                if (shape is IPointShape point)
                {
                    double s = 1.0 / scale;
                    canvas.Save();
                    double dx = 0.0 - (point.X * s) + point.X;
                    double dy = 0.0 - (point.Y * s) + point.Y;
                    canvas.Translate((float)dx, (float)dy);
                    canvas.Scale((float)s);

                    point.Draw(canvas, renderer, 0.0, 0.0, scale, null, null);

                    canvas.Restore();
                }
            }
        }

        private void DrawDecorators(IContainerView view, SKCanvas canvas, IShapeRenderer renderer, double scale)
        {
            var selected = new List<IBaseShape>(view.SelectionState.Shapes);

            foreach (var shape in selected)
            {
                shape.Decorator?.Draw(canvas, shape, renderer, view.SelectionState, 0.0, 0.0, scale);
            }
        }

        private SKPicture RecordPicture(IContainerView view, IShapeRenderer renderer, double scale, Action<IContainerView, SKCanvas, IShapeRenderer, double> draw)
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

        private void DrawGrid(SKCanvas canvas, double dx, double dy, double zx, double zy)
        {
            float gw = (float)_view.Width;
            float gh = (float)_view.Height;
            float cw = 15.0f;
            float ch = 15.0f;

            canvas.Save();
            canvas.Translate((float)dx, (float)dy);
            canvas.Scale((float)zx, (float)zy);

            var hlattice = SKMatrix.CreateScale(cw, ch);
            hlattice = hlattice.PreConcat(SKMatrix.CreateRotation((float)(Math.PI * 0.0 / 180.0)));

            var vlattice = SKMatrix.CreateScale(cw, ch);
            vlattice = vlattice.PreConcat(SKMatrix.CreateRotation((float)(Math.PI * 90.0 / 180.0)));

            using (var heffect = SKPathEffect.Create2DLine((float)(1.0 / zx), hlattice))
            using (var veffect = SKPathEffect.Create2DLine((float)(1.0 / zx), vlattice))
            using (var hpaint = new SKPaint())
            using (var vpaint = new SKPaint())
            {
                hpaint.IsAntialias = false;
                hpaint.Color = SKColors.LightGray;
                hpaint.PathEffect = heffect;
                canvas.DrawRect(SKRect.Create(0.0f, ch, gw, gh - ch), hpaint);
                vpaint.IsAntialias = false;
                vpaint.Color = SKColors.LightGray;
                vpaint.PathEffect = veffect;
                canvas.DrawRect(SKRect.Create(cw, 0.0f, gw - cw, gh), vpaint);
            }

            using (SKPaint strokePaint = new SKPaint())
            {
                strokePaint.IsAntialias = false;
                strokePaint.StrokeWidth = (float)(1.0 / zx);
                strokePaint.Color = SKColors.Red;
                strokePaint.Style = SKPaintStyle.Stroke;
                canvas.DrawRect(SKRect.Create(0.0f, 0.0f, gw, gh), strokePaint);
            }

            canvas.Restore();
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            bool isStyleLibraryDirty = _context.DocumentContainer.StyleLibrary.IsTreeDirty();
            bool isCurrentContainerDirty = _view.CurrentContainer.IsTreeDirty();
            bool isWorkingContainerDirty = _view.WorkingContainer.IsTreeDirty();
            bool isPointsCurrentContainerDirty = _view.CurrentContainer.IsPointsTreeDirty();
            bool isPointsWorkingContainerDirty = _view.WorkingContainer.IsPointsTreeDirty();
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

            bool isSelectionDirty = _view.SelectionState.IsTreeDirty() == true || isShapesCurrentDirty == true || isShapesWorkingDirty == true;
            if (_view.SelectionState.IsTreeDirty() == true)
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

            if (_view.InputBackground?.Color != null)
            {
                canvas.Clear(SkiaUtil.ToSKColor(_view.InputBackground.Color));
            }
            else
            {
                canvas.Clear();
            }

            if (_view.WorkBackground != null)
            {
                GetSKPaintFill(_view.WorkBackground, null, 1.0, out var brush);
                canvas.Save();
                canvas.Translate((float)dx, (float)dy);
                canvas.Scale((float)zx, (float)zy);
                canvas.DrawRect(SkiaUtil.ToSKRect(0.0, 0.0, _view.Width, _view.Height), brush);
                canvas.Restore();
            }

            // TODO: Fix grid X and Y position.
            //DrawGrid(canvas, dx, dy, zx, zy);

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
                _context.DocumentContainer.StyleLibrary?.Invalidate();
            }
        }
    }
}
