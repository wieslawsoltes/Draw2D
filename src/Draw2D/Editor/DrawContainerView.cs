// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Media;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using SkiaSharp;

namespace Draw2D.Editor
{
    public class DrawContainerView : IDrawContainerView
    {
        private void Draw(IContainerView view, DrawingContext context, double width, double height, double dx, double dy, double zx, double zy)
        {
            var currentContainer = view.CurrentContainer;
            var workingContainer = view.WorkingContainer;
            var presenter = view.Presenter;
            var renderer = view.Renderer;

            if (currentContainer.InputBackground != null)
            {
                var color = AvaloniaBrushCache.FromDrawColor(currentContainer.InputBackground);
                var brush = new SolidColorBrush(color);
                context.FillRectangle(brush, new Rect(0, 0, width, height));
            }

            var state = context.PushPreTransform(new Matrix(zx, 0.0, 0.0, zy, dx, dy));

            if (currentContainer.WorkBackground != null)
            {
                var color = AvaloniaBrushCache.FromDrawColor(currentContainer.WorkBackground);
                var brush = new SolidColorBrush(color);
                context.FillRectangle(brush, new Rect(0.0, 0.0, currentContainer.Width, currentContainer.Height));
            }

            presenter.DrawContainer(context, currentContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            presenter.DrawContainer(context, workingContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);

            presenter.DrawDecorators(context, currentContainer, renderer, 0.0, 0.0, DrawMode.Shape);
            presenter.DrawDecorators(context, workingContainer, renderer, 0.0, 0.0, DrawMode.Shape);

            presenter.DrawContainer(context, currentContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);
            presenter.DrawContainer(context, workingContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);

            state.Dispose();
        }

        private void Draw(IContainerView view, SKCanvas canvas, double width, double height, double dx, double dy, double zx, double zy)
        {
            var currentContainer = view.CurrentContainer;
            var workingContainer = view.WorkingContainer;
            var presenter = view.Presenter;
            var renderer = new SkiaShapeRenderer(zx)
            {
                Selection = view.Selection
            };

            canvas.Save();

            if (currentContainer.InputBackground != null)
            {
                using (var brush = SkiapHelper.ToSKPaintBrush(currentContainer.InputBackground))
                {
                    canvas.DrawRect(SkiapHelper.ToRect(0.0, 0.0, width, height), brush);
                }
            }

            canvas.Translate((float)dx, (float)dy);
            canvas.Scale((float)zx, (float)zy);

            if (currentContainer.WorkBackground != null)
            {
                using (var brush = SkiapHelper.ToSKPaintBrush(currentContainer.WorkBackground))
                {
                    canvas.DrawRect(SkiapHelper.ToRect(0.0, 0.0, currentContainer.Width, currentContainer.Height), brush);
                }
            }

            presenter.DrawContainer(canvas, currentContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            presenter.DrawContainer(canvas, workingContainer, renderer, 0.0, 0.0, DrawMode.Shape, null, null);

            presenter.DrawDecorators(canvas, currentContainer, renderer, 0.0, 0.0, DrawMode.Shape);
            presenter.DrawDecorators(canvas, workingContainer, renderer, 0.0, 0.0, DrawMode.Shape);

            presenter.DrawContainer(canvas, workingContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);
            presenter.DrawContainer(canvas, currentContainer, renderer, 0.0, 0.0, DrawMode.Point, null, null);

            canvas.Restore();
        }

        public void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            switch (context)
            {
                case DrawingContext drawingContext:
                    Draw(view, drawingContext, width, height, dx, dy, zx, zy);
                    break;
                case SKCanvas canvas:
                    Draw(view, canvas, width, height, dx, dy, zx, zy);
                    break;
            }
        }
    }
}
