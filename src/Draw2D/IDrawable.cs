// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Renderer;
using Draw2D.Core.Style;

namespace Draw2D.Core
{
    public interface IDrawable
    {
        ShapeStyle Style { get; set; }
        MatrixObject Transform { get; set; }
        void BeginTransform(object dc, ShapeRenderer r);
        void EndTransform(object dc, ShapeRenderer r);
        void Draw(object dc, ShapeRenderer r, double dx, double dy);
    }
}
