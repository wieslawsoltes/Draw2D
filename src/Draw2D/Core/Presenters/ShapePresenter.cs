// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Containers;
using Draw2D.Renderer;

namespace Draw2D.Presenters
{
    public class ShapePresenter
    {
        public Dictionary<Type, ShapeHelper> Helpers { get; set; }

        public virtual void DrawContainer(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            container.Invalidate(renderer, dx, dy);
            container.Draw(dc, renderer, dx, dy, db, r);
        }

        public virtual void DrawHelpers(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy)
        {
            var shapes = container.Shapes;
            var selection = renderer;

            foreach (var shape in shapes)
            {
                if (selection.Selected.Contains(shape))
                {
                    if (Helpers.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, renderer, shape, selection, dx, dy);
                    }
                }
            }
        }
    }
}
