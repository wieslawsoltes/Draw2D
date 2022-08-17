using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Presenters
{
    public class AvaloniaContainerPresenter : IContainerPresenter
    {
        private readonly IToolContext _context;
        private readonly IContainerView _view;
        private RenderTargetBitmap _renderTarget;
        private double _renderScaling;
        private IContainerPresenter _editorContainerPresenter;

        public AvaloniaContainerPresenter(IToolContext context, IContainerView view)
        {
            _context = context;
            _view = view;
            _renderTarget = null;
            _renderScaling = 1.0;
            _editorContainerPresenter = new SkiaEditorContainerPresenter(_context, _view);
        }

        public void Dispose()
        {
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }

            if (_editorContainerPresenter != null)
            {
                _editorContainerPresenter.Dispose();
                _editorContainerPresenter = null;
            }
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy, double renderScaling)
        {
            if (context is DrawingContext drawingContext && width > 0 && height > 0)
            {
                if (_renderTarget == null)
                {
                    _renderScaling = renderScaling;
                    _renderTarget = new RenderTargetBitmap(
                        new PixelSize((int)(width / _renderScaling), (int)(height / _renderScaling)), 
                        new Vector(96, 96 ));
                }
                else if (_renderTarget.PixelSize.Width != (int)width || _renderTarget.PixelSize.Height != (int)height || Math.Abs(_renderScaling - renderScaling) > double.Epsilon)
                {
                    _renderScaling = renderScaling;
                    _renderTarget.Dispose();
                    _renderTarget = new RenderTargetBitmap(
                        new PixelSize((int)(width / _renderScaling), (int)(height / _renderScaling)), 
                        new Vector(96, 96));
                }

                using var drawingContextImpl = _renderTarget.CreateDrawingContext(null);
                var leaseFeature = drawingContextImpl.GetFeature<ISkiaSharpApiLeaseFeature>();
                if (leaseFeature is null)
                {
                    return;
                }
                using var lease = leaseFeature.Lease();

                if (lease?.SkCanvas is { } skCanvas)
                {
                    var skMatrix = SKMatrix.CreateScale((float)(1.0 / renderScaling), (float)(1.0 / renderScaling));

                    skCanvas.Save();
                    skCanvas.SetMatrix(skMatrix);
 
                    _editorContainerPresenter.Draw(skCanvas, width, height, dx, dy, zx, zy, renderScaling);

                    skCanvas.Restore();
                }

                drawingContext.DrawImage(_renderTarget,
                    new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height),
                    new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height));
            }
        }
    }
}
