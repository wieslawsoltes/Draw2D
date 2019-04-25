// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Draw2D.ViewModels;
using SkiaSharp;

namespace Draw2D.Editor
{
    struct CustomDrawOperation : ICustomDrawOperation
    {
        private IToolContext _ctx;
        private double _width;
        private double _height;
        private double _dx;
        private double _dy;
        private double _zx;
        private double _zy;

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public CustomDrawOperation(IToolContext context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _ctx = context;
            _width = width;
            _height = height;
            _dx = dx;
            _dy = dy;
            _zx = zx;
            _zy = zy;
            Bounds = new Rect(0, 0, width, height);
        }

        private void Draw(SKCanvas canvas, IToolContext ctx, double width, double height, double dx, double dy, double zx, double zy)
        {
            var renderer = new SkiaShapeRenderer()
            {
                Selection = ctx.Selection
            };

            if (ctx.CurrentContainer.InputBackground != null)
            {
                using (var brush = SkiaShapeRenderer.ToSKPaintBrush(ctx.CurrentContainer.InputBackground))
                {
                    canvas.DrawRect(SkiaShapeRenderer.ToRect(0, 0, width, height), brush);
                }
            }

            if (ctx.CurrentContainer.WorkBackground != null)
            {
                using (var brush = SkiaShapeRenderer.ToSKPaintBrush(ctx.CurrentContainer.WorkBackground))
                {
                    canvas.DrawRect(SkiaShapeRenderer.ToRect(dx * zx, dy * zy, (ctx.CurrentContainer.Width + dx) * zx, (ctx.CurrentContainer.Height + dy) * zy), brush);
                }
            }

            ctx.Presenter.DrawContainer(canvas, ctx.CurrentContainer, renderer, dx, dy, zx, zy, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(canvas, ctx.CurrentContainer, renderer, dx, dy, zx, zy, DrawMode.Point, null, null);

            ctx.Presenter.DrawContainer(canvas, ctx.WorkingContainer, renderer, dx, dy, zx, zy, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(canvas, ctx.WorkingContainer, renderer, dx, dy, zx, zy, DrawMode.Point, null, null);

            ctx.Presenter.DrawDecorators(canvas, ctx.CurrentContainer, renderer, dx, dy, zx, zy, DrawMode.Shape);

            ctx.Presenter.DrawDecorators(canvas, ctx.WorkingContainer, renderer, dx, dy, zx, zy, DrawMode.Shape);
        }

        public void Render(IDrawingContextImpl context)
        {
            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas != null)
            {
                canvas.Save();

                Draw(canvas, _ctx, _width, _height, _dx, _dy, _zx, _zy);

                canvas.Restore();
            }
        }

        public void Dispose()
        {
        }
    }
}
