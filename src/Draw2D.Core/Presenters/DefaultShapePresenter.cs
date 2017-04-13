// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Editor;

namespace Draw2D.Core.Presenters
{
    public class DefaultShapePresenter : ShapePresenter
    {
        public override void DrawContent(object dc, IToolContext context)
        {
            var r = context.Renderer;

            foreach (var shape in context.CurrentContainer.Guides)
            {
                shape.Invalidate(r);
                shape.Draw(dc, r, 0.0, 0.0);
            }

            foreach (var shape in context.CurrentContainer.Shapes)
            {
                shape.Invalidate(r);
                shape.Draw(dc, r, 0.0, 0.0);
            }
        }

        public override void DrawWorking(object dc, IToolContext context)
        {
            var r = context.Renderer;

            foreach (var shape in context.WorkingContainer.Shapes)
            {
                shape.Invalidate(r);
                shape.Draw(dc, r, 0.0, 0.0);
            }
        }

        public override void DrawHelpers(object dc, IToolContext context)
        {
            DrawHelpers(dc, context, context.CurrentContainer.Shapes);
            DrawHelpers(dc, context, context.WorkingContainer.Shapes);
        }

        public void DrawHelpers(object dc, IToolContext context, IEnumerable<ShapeObject> shapes)
        {
            var r = context.Renderer;
            var selected = context.Selected;

            foreach (var shape in shapes)
            {
                if (selected.Contains(shape))
                {
                    if (Helpers.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, r, shape, selected);
                    }
                }
            }
        }
    }
}
