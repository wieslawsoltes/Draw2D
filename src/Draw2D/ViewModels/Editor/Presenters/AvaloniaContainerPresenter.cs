using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
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
        private double _renderScaling;
        private IContainerPresenter _editorContainerPresenter;

        public AvaloniaContainerPresenter(IToolContext context, IContainerView view)
        {
            _context = context;
            _view = view;
            _renderScaling = 1.0;
            _editorContainerPresenter = new SkiaEditorContainerPresenter(_context, _view);
        }

        public void Dispose()
        {
            if (_editorContainerPresenter != null)
            {
                _editorContainerPresenter.Dispose();
                _editorContainerPresenter = null;
            }
        }

        private class CustomDrawOp : ICustomDrawOperation
        {
            private readonly Action<SKCanvas> _draw;

            public CustomDrawOp(Action<SKCanvas> draw, Rect bounds)
            {
                _draw = draw;
                Bounds = bounds;
            }
            
            public void Dispose()
            {
            }

            public bool HitTest(Point p)
            {
                return false;
            }

            public void Render(IDrawingContextImpl context)
            {
                using var lease = context.GetFeature<ISkiaSharpApiLeaseFeature>()?.Lease();
                if (lease?.SkCanvas is { } skCanvas)
                {
                    _draw.Invoke(skCanvas);
                }
            }

            public Rect Bounds { get; }

            public bool Equals(ICustomDrawOperation other)
            {
                return false;
            }
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy, double renderScaling)
        {
            if (context is DrawingContext drawingContext && width > 0 && height > 0)
            {
                void DrawLocal(SKCanvas skCanvas)
                {
                    skCanvas.Save();
                    _editorContainerPresenter.Draw(skCanvas, width, height, dx, dy, zx, zy, renderScaling);
                    skCanvas.Restore();
                }

                drawingContext.Custom(new CustomDrawOp(DrawLocal, new Rect(0d, 0d, width, height)));
            }
        }
    }
}
