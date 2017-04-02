// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Renderers.Helpers
{
    public class QuadraticBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, QuadraticBezierShape quadraticBezier)
        {
            DrawLine(dc, r, quadraticBezier.StartPoint, quadraticBezier.Point1);
            DrawLine(dc, r, quadraticBezier.Point1, quadraticBezier.Point2);
            DrawEllipse(dc, r, quadraticBezier.StartPoint, 4.0);
            DrawEllipse(dc, r, quadraticBezier.Point1, 4.0);
            DrawEllipse(dc, r, quadraticBezier.Point2, 4.0);
        }

        public override void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, r, quadraticBezier);
            }
        }
    }
}
