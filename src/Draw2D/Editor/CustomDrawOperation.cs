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
        private bool _drawWorking;
        private IToolContext _ctx;

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public CustomDrawOperation(Rect bounds, bool drawWorking, IToolContext context)
        {
            Bounds = bounds;
            _drawWorking = drawWorking;
            _ctx = context;
        }

        public void Render(IDrawingContextImpl context)
        {
            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas != null)
            {
                canvas.Save();

                Draw(canvas, _ctx, _drawWorking);

                canvas.Restore();
            }
        }

        private void Draw(SKCanvas canvas, IToolContext ctx, bool drawWorking)
        {
            var renderer = new SkiaShapeRenderer()
            {
                Selection = ctx.Selection
            };

            if (ctx.CurrentContainer.WorkBackground != null)
            {
                using (var brush = SkiaShapeRenderer.ToSKPaintBrush(ctx.CurrentContainer.WorkBackground))
                {
                    canvas.DrawRect(SkiaShapeRenderer.ToRect(0, 0, Bounds.Width, Bounds.Height), brush);
                }
            }

            ctx.Presenter.DrawContainer(canvas, ctx.CurrentContainer, renderer, 0.0, 0.0, null, null);

            if (drawWorking)
            {
                ctx.Presenter.DrawContainer(canvas, ctx.WorkingContainer, renderer, 0.0, 0.0, null, null);
            }

            ctx.Presenter.DrawDecorators(canvas, ctx.CurrentContainer, renderer, 0.0, 0.0);

            if (drawWorking)
            {
                ctx.Presenter.DrawDecorators(canvas, ctx.WorkingContainer, renderer, 0.0, 0.0);
            }
        }

        public void Dispose()
        {
        }
    }
}
