// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class TextDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, TextShape textShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, textShape.TopLeft, textShape.BottomRight, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selection, double dx, double dy, DrawMode mode)
        {
            if (shape is TextShape textShape)
            {
                Draw(dc, renderer, textShape, dx, dy, mode);
            }
        }
    }
}
