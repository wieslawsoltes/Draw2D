// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Containers;
using Draw2D.Core.Renderer;

namespace Draw2D.Core.Presenters
{
    public class DefaultShapePresenter : ShapePresenter
    {
        public override void DrawContainer(object dc, IShapeContainer container, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            container.Invalidate(renderer, dx, dy);
            container.Draw(dc, renderer, dx, dy, db, r);
        }

        public override void DrawHelpers(object dc, IShapeContainer container, ShapeRenderer renderer, double dx, double dy)
        {
            var shapes = container.Shapes;
            var selected = renderer.Selected;

            foreach (var shape in shapes)
            {
                if (selected.Contains(shape))
                {
                    if (Helpers.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, renderer, shape, selected, dx, dy);
                    }
                }
            }
        }
    }
}
