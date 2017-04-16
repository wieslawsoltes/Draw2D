// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Tools.Helpers
{
    public class QuadraticBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, QuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            DrawLine(dc, r, quadraticBezier.StartPoint, quadraticBezier.Point1, dx, dy);
            DrawLine(dc, r, quadraticBezier.Point1, quadraticBezier.Point2, dx, dy);
        }

        public override void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected, double dx, double dy)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, r, quadraticBezier, dx, dy);
            }
        }
    }
}
