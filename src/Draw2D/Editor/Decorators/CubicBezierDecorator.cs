// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Renderer;
using Draw2D.Shapes;

namespace Draw2D.Editor.Decorators
{
    public class CubicBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, CubicBezierShape cubicBezier, double dx, double dy)
        {
            DrawLine(dc, renderer, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy);
            DrawLine(dc, renderer, cubicBezier.Point3, cubicBezier.Point2, dx, dy);
            DrawLine(dc, renderer, cubicBezier.Point1, cubicBezier.Point2, dx, dy);
        }

        public override void Draw(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, renderer, cubicBezier, dx, dy);
            }
        }
    }
}
