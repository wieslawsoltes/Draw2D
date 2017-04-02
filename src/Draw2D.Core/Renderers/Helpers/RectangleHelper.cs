// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Renderers.Helpers
{
    public class RectangleHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, RectangleShape rectangle)
        {
            DrawEllipse(dc, r, rectangle.TopLeft, 4.0);
            DrawEllipse(dc, r, rectangle.BottomRight, 4.0);
        }

        public override void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected)
        {
            if (shape is RectangleShape rectangle)
            {
                Draw(dc, r, rectangle);
            }
        }
    }
}
