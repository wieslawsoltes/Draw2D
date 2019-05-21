// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Editor.Views
{
    public class AvaloniaSkiaView : IDrawContainerView
    {
        private RenderTargetBitmap _renderTarget;
        private IDrawContainerView _editorSkiaView;

        public AvaloniaSkiaView()
        {
            _renderTarget = null;
            _editorSkiaView = new EditorSkiaView();
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

        public void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy)
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

                using (var _drawingContextImpl = _renderTarget.CreateDrawingContext(null))
                {
                    var _skiaDrawingContextImpl = _drawingContextImpl as ISkiaDrawingContextImpl;

                    _editorSkiaView.Draw(view, _skiaDrawingContextImpl.SkCanvas, width, height, dx, dy, zx, zy);

                    drawingContext.DrawImage(_renderTarget, 1.0,
                        new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height),
                        new Rect(0, 0, width, height));
                }
            }
        }
    }
}
