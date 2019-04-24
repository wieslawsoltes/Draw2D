// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Draw2D.ViewModels;

namespace Draw2D.Editor
{
    public class AvaloniaRenderView : Control
    {
        private bool _drawWorking = false;

        public static readonly StyledProperty<bool> CustomDrawProperty =
            AvaloniaProperty.Register<AvaloniaRenderView, bool>(nameof(CustomDraw));

        public bool CustomDraw
        {
            get { return GetValue(CustomDrawProperty); }
            set { SetValue(CustomDrawProperty, value); }
        }

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

            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Point, null, null);

            if (drawWorking)
            {
                ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape, null, null);
                ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Point, null, null);
            }

            ctx.Presenter.DrawDecorators(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape);

            if (drawWorking)
            {
                ctx.Presenter.DrawDecorators(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape);
            }
        }

        public override void Render(DrawingContext context)
        {
            if (this.DataContext is IToolContext ctx)
            {
                if (CustomDraw)
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
