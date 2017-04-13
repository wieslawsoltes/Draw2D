// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Editor;

namespace Draw2D.Core.Presenters
{
    public class DefaultShapePresenter : ShapePresenter
    {
        public override void DrawContent(object dc, IToolContext context, double dx, double dy)
        {
            var r = context.Renderer;
            var container = context.CurrentContainer;
            container.Invalidate(r, dx, dy);
            container.Draw(dc, r, dx, dy);
        }

        public override void DrawWorking(object dc, IToolContext context, double dx, double dy)
        {
            var r = context.Renderer;
            var container = context.WorkingContainer;
            container.Invalidate(r, dx, dy);
            container.Draw(dc, r, dx, dy);
        }

        public override void DrawHelpers(object dc, IToolContext context, double dx, double dy)
        {
            DrawHelpers(dc, context, context.CurrentContainer.Shapes, dx, dy);
            DrawHelpers(dc, context, context.WorkingContainer.Shapes, dx, dy);
        }

        public void DrawHelpers(object dc, IToolContext context, IEnumerable<ShapeObject> shapes, double dx, double dy)
        {
            var r = context.Renderer;
            var selected = context.Selected;

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
