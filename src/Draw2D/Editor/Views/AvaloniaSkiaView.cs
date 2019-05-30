// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor.Views
{
    public class AvaloniaSkiaView : IDrawContainerView
    {
        private readonly IToolContext _context;
        private readonly IContainerView _view;
        private RenderTargetBitmap _renderTarget;
        private IDrawContainerView _editorSkiaView;

        public AvaloniaSkiaView(IToolContext context, IContainerView view)
        {
            _context = context;
            _view = view;
            _renderTarget = null;
            _editorSkiaView = new EditorSkiaView(_context, _view);
        }

        public void Dispose()
        {
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }

            if (_editorSkiaView != null)
            {
                _editorSkiaView.Dispose();
                _editorSkiaView = null;
            }
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            if (context is DrawingContext drawingContext && width > 0 && height > 0)
            {
                if (_renderTarget == null)
                {
                    _renderTarget = new RenderTargetBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96));
                }
                else if (_renderTarget.PixelSize.Width != (int)width || _renderTarget.PixelSize.Height != (int)height)
                {
                    _renderTarget.Dispose();
                    _renderTarget = new RenderTargetBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96));
                }

                using (var drawingContextImpl = _renderTarget.CreateDrawingContext(null))
                {
                    var skiaDrawingContextImpl = drawingContextImpl as ISkiaDrawingContextImpl;

                    _editorSkiaView.Draw(skiaDrawingContextImpl.SkCanvas, width, height, dx, dy, zx, zy);

                    drawingContext.DrawImage(_renderTarget, 1.0,
                        new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height),
                        new Rect(0, 0, width, height));
                }
            }
        }
    }
}
