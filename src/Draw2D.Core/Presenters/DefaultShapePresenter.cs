// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Editor;

namespace Draw2D.Core.Presenters
{
    public class DefaultShapePresenter : ShapePresenter
    {
        public override void Draw(object dc, IToolContext context)
        {
            foreach (var shape in context.CurrentContainer.Guides)
            {
                shape.Draw(dc, context.Renderer, 0.0, 0.0);
            }

            foreach (var shape in context.CurrentContainer.Shapes)
            {
                shape.Draw(dc, context.Renderer, 0.0, 0.0);
            }

            foreach (var shape in context.WorkingContainer.Shapes)
            {
                shape.Draw(dc, context.Renderer, 0.0, 0.0);
            }
        }

        public override void DrawHelpers(object dc, IToolContext context)
        {
            DrawHelpers(dc, context, context.CurrentContainer.Shapes, context.Selected);
            DrawHelpers(dc, context, context.WorkingContainer.Shapes, context.Selected);
        }

        public void DrawHelpers(object dc, IToolContext context, IEnumerable<ShapeObject> shapes, ISet<ShapeObject> selected)
        {
            foreach (var shape in shapes)
            {
                if (selected.Contains(shape))
                {
                    if (Helpers.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, context.Renderer, shape, context.Selected);
                    }
                }
            }
        }
    }
}
