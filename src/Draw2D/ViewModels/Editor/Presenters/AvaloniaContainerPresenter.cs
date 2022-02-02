using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

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
                    _renderTarget = new RenderTargetBitmap(new PixelSize((int)width, (int)height), new Vector(96 * _renderScaling, 96 * _renderScaling));
                }
                else if (_renderTarget.PixelSize.Width != (int)width || _renderTarget.PixelSize.Height != (int)height || Math.Abs(_renderScaling - renderScaling) > double.Epsilon)
                {
                    _renderScaling = renderScaling;
                    _renderTarget.Dispose();
                    _renderTarget = new RenderTargetBitmap(new PixelSize((int)width, (int)height), new Vector(96* _renderScaling, 96 * _renderScaling));
                }

                using var drawingContextImpl = _renderTarget.CreateDrawingContext(null);
                var skiaDrawingContextImpl = drawingContextImpl as ISkiaDrawingContextImpl;

                _editorContainerPresenter.Draw(skiaDrawingContextImpl.SkCanvas, width, height, dx, dy, zx, zy, renderScaling);

                drawingContext.DrawImage(_renderTarget,
                    new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height),
                    new Rect(0, 0, width, height));
            }
        }
    }
}
