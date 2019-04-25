// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class PointDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, PointShape pointShape, double dx, double dy, double zx, double zy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, pointShape, 10, dx, dy, zx, zy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selection, double dx, double dy, double zx, double zy, DrawMode mode)
        {
            if (shape is PointShape pointShape)
            {
                Draw(dc, renderer, pointShape, dx, dy, zx, zy, mode);
            }
        }
    }
}
