// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class RectangleDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, RectangleShape rectangleShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, rectangleShape.TopLeft, rectangleShape.BottomRight, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selection, double dx, double dy, DrawMode mode)
        {
            if (shape is RectangleShape rectangleShape)
            {
                Draw(dc, renderer, rectangleShape, dx, dy, mode);
            }
        }
    }
}
