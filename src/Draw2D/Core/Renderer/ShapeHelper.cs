// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Shapes;

namespace Draw2D.Renderer
{
    public abstract class ShapeHelper
    {
        public abstract void Draw(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selected, double dx, double dy);
    }
}
