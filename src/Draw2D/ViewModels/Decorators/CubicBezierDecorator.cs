// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class CubicBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, CubicBezierShape cubicBezier, double dx, double dy, DrawMode mode)
        {
            DrawLine(dc, renderer, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy, mode);
            DrawLine(dc, renderer, cubicBezier.Point3, cubicBezier.Point2, dx, dy, mode);
            DrawLine(dc, renderer, cubicBezier.Point1, cubicBezier.Point2, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selection, double dx, double dy, DrawMode mode)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, renderer, cubicBezier, dx, dy, mode);
            }
        }
    }
}
