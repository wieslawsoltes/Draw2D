// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Editor.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Editor.Views
{
    public class ExportSkiaView : IDrawContainerView
    {
        private readonly IToolContext _context;
        private readonly IContainerView _view;

        public ExportSkiaView(IToolContext context, IContainerView view)
        {
            _context = context;
            _view = view;
        }

        public void Dispose()
        {
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            using (var renderer = new SkiaShapeRenderer(_context, _view.SelectionState))
            using (var background = SkiaHelper.ToSKPaintBrush(_view.PrintBackground))
            {
                var canvas = context as SKCanvas;
                canvas.DrawRect(SkiaHelper.ToRect(0.0, 0.0, _view.Width, _view.Height), background);
                _view.CurrentContainer.Draw(canvas, renderer, 0.0, 0.0, zx, null, null);
            }
        }
    }
}
