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
        private bool _drawWorking;
        private double _width;
        private double _height;
        private double _ox;
        private double _oy;

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public CustomDrawOperation(IToolContext context, bool drawWorking, double width, double height, double ox, double oy)
        {
            _ctx = context;
            _drawWorking = drawWorking;
            _width = width;
            _height = height;
            _ox = ox;
            _oy = oy;
            Bounds = new Rect(0, 0, width, height);
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

            if (ctx.CurrentContainer.InputBackground != null)
            {
                using (var brush = SkiaShapeRenderer.ToSKPaintBrush(ctx.CurrentContainer.InputBackground))
                {
                    canvas.DrawRect(SkiaShapeRenderer.ToRect(0, 0, _width, _height), brush);
                }
            }

            if (ctx.CurrentContainer.WorkBackground != null)
            {
                using (var brush = SkiaShapeRenderer.ToSKPaintBrush(ctx.CurrentContainer.WorkBackground))
                {
                    canvas.DrawRect(SkiaShapeRenderer.ToRect(_ox, _oy, ctx.CurrentContainer.Width, ctx.CurrentContainer.Height), brush);
                }
            }

            ctx.Presenter.DrawContainer(canvas, ctx.CurrentContainer, renderer, ox, oy, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(canvas, ctx.CurrentContainer, renderer, ox, oy, DrawMode.Point, null, null);

            if (drawWorking)
            {
                ctx.Presenter.DrawContainer(canvas, ctx.WorkingContainer, renderer, ox, oy, DrawMode.Shape, null, null);
                ctx.Presenter.DrawContainer(canvas, ctx.WorkingContainer, renderer, ox, oy, DrawMode.Point, null, null);
            }

            ctx.Presenter.DrawDecorators(canvas, ctx.CurrentContainer, renderer, ox, oy, DrawMode.Shape);

            if (drawWorking)
            {
                ctx.Presenter.DrawDecorators(canvas, ctx.WorkingContainer, renderer, ox, oy, DrawMode.Shape);
            }
        }

        public void Dispose()
        {
        }
    }
}
