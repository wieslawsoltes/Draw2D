// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Presenters
{
    public class SkiaExportSelectedPresenter : IContainerPresenter
    {
        private readonly IToolContext _context;
        private readonly IContainerView _view;

        public SkiaExportSelectedPresenter(IToolContext context, IContainerView view)
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
            using (var background = SkiaHelper.ToSKPaintBrush(_view.PrintBackground, false))
            {
                var canvas = context as SKCanvas;
                canvas.DrawRect(SkiaHelper.ToSKRect(dx, dy, _view.Width + dx, _view.Height + dy), background);

                var selected = new List<IBaseShape>(_view.SelectionState?.Shapes);
                foreach (var shape in selected)
                {
                    if (!(shape is IPointShape))
                    {
                        shape.Draw(canvas, renderer, dx, dy, zx, null, null);
                    }
                }
            }
        }
    }
}
