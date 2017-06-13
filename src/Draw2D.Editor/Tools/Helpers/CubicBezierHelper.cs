// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderer;
using Draw2D.Core.Shape;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Tools.Helpers
{
    public class CubicBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, CubicBezierShape cubicBezier, double dx, double dy)
        {
            DrawLine(dc, r, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy);
            DrawLine(dc, r, cubicBezier.Point3, cubicBezier.Point2, dx, dy);
            DrawLine(dc, r, cubicBezier.Point1, cubicBezier.Point2, dx, dy);
        }

        public override void Draw(object dc, ShapeRenderer r, BaseShape shape, ISet<BaseShape> selected, double dx, double dy)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, r, cubicBezier, dx, dy);
            }
        }
    }
}
