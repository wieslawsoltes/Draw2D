// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public interface IShapeDecorator
    {
        void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selected, double dx, double dy);
    }
}
