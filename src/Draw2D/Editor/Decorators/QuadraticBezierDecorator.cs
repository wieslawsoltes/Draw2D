// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Renderer;
using Draw2D.Shapes;

namespace Draw2D.Editor.Decorators
{
    public class QuadraticBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, QuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            DrawLine(dc, renderer, quadraticBezier.StartPoint, quadraticBezier.Point1, dx, dy);
            DrawLine(dc, renderer, quadraticBezier.Point1, quadraticBezier.Point2, dx, dy);
        }

        public override void Draw(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, renderer, quadraticBezier, dx, dy);
            }
        }
    }
}
