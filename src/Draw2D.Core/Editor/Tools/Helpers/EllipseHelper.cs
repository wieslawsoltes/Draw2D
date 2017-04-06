// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor.Tools.Helpers
{
    public class EllipseHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, EllipseShape ellipse)
        {
            DrawEllipse(dc, r, ellipse.TopLeft, 4.0);
            DrawEllipse(dc, r, ellipse.BottomRight, 4.0);
        }

        public override void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected)
        {
            if (shape is EllipseShape ellipse)
            {
                Draw(dc, r, ellipse);
            }
        }
    }
}
