// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Draw2D.ViewModels;

namespace Draw2D.Editor
{
    struct CustomDrawOperation : ICustomDrawOperation
    {
        private bool _drawWorking;
        private IToolContext _ctx;

        public CustomDrawOperation(Rect bounds, bool drawWorking, IToolContext context)
        {
            Bounds = bounds;
            _drawWorking = drawWorking;
            _ctx = context;
        }

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

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

        private void Draw(SkiaSharp.SKCanvas canvas, IToolContext ctx, bool drawWorking)
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

    public class AvaloniaRenderView : Control
    {
        private bool _drawWorking = false;
        private bool _customDraw = true;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is IToolContext ctx)
            {
                var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
                if (md != null)
                {
                    ctx.Capture = () =>
                    {
                        if (md.Captured == null)
                        {
                            md.Capture(this);
                        }
                    };
                    ctx.Release = () =>
                    {
                        if (md.Captured != null)
                        {
                            md.Capture(null);
                        }
                    };
                    ctx.Invalidate = () => this.InvalidateVisual();
                }
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is IToolContext ctx)
            {
                ctx.Capture = null;
                ctx.Release = null;
                ctx.Invalidate = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            _drawWorking = true;
            this.InvalidateVisual();
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            _drawWorking = false;
            this.InvalidateVisual();
        }

        private void Draw(DrawingContext context, IToolContext ctx, bool drawWorking)
        {
            if (ctx.CurrentContainer.WorkBackground != null)
            {
                var color = AvaloniaBrushCache.FromDrawColor(ctx.CurrentContainer.WorkBackground);
                var brush = new SolidColorBrush(color);
                context.FillRectangle(brush, new Rect(0, 0, Bounds.Width, Bounds.Height));
            }

            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, null, null);

            if (drawWorking)
            {
                ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, null, null);
            }

            ctx.Presenter.DrawDecorators(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0);

            if (drawWorking)
            {
                ctx.Presenter.DrawDecorators(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0);
            }
        }

        public override void Render(DrawingContext context)
        {
            if (this.DataContext is IToolContext ctx)
            {
                if (_customDraw)
                {
                    context.Custom(new CustomDrawOperation(new Rect(0, 0, Bounds.Width, Bounds.Height), _drawWorking, ctx));
                }
                else
                {
                    Draw(context, ctx, _drawWorking);
                }
            }
        }
    }
}
