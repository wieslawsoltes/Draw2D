// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels
{
    public class CanvasPresenter : ICanvasPresenter
    {
        public IDictionary<Type, IShapeDecorator> Decorators { get; set; }

        public void DrawContainer(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            container.Invalidate(renderer, dx, dy);
            container.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public void DrawDecorators(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, DrawMode mode)
        {
            var shapes = container.Shapes;
            var selection = renderer.Selection;

            foreach (var shape in shapes)
            {
                if (selection.Selected.Contains(shape))
                {
                    if (Decorators.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, shape, renderer, selection, dx, dy, mode);
                    }
                }
            }
        }
    }
}
