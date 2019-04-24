// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class QuadraticBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, QuadraticBezierShape quadraticBezier, double dx, double dy, DrawMode mode)
        {
            DrawLine(dc, renderer, quadraticBezier.StartPoint, quadraticBezier.Point1, dx, dy, mode);
            DrawLine(dc, renderer, quadraticBezier.Point1, quadraticBezier.Point2, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selection, double dx, double dy, DrawMode mode)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, renderer, quadraticBezier, dx, dy, mode);
            }
        }
    }
}
