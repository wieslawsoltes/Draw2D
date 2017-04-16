// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Containers;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Presenters
{
    public class DefaultShapePresenter : ShapePresenter
    {
        public override void DrawContainer(object dc, IShapesContainer container, ShapeRenderer r, double dx, double dy)
        {
            container.Invalidate(r, dx, dy);
            container.Draw(dc, r, dx, dy);
        }

        public override void DrawHelpers(object dc, IShapesContainer container, ShapeRenderer r, double dx, double dy)
        {
            var shapes = container.Shapes;
            var selected = r.Selected;

            foreach (var shape in shapes)
            {
                if (selected.Contains(shape))
                {
                    if (Helpers.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, r, shape, selected, dx, dy);
                    }
                }
            }
        }
    }
}
