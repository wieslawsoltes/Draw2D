// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class EllipseDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, EllipseShape ellipseShape, double dx, double dy, double scale)
        {
            DrawRectangle(dc, renderer, ellipseShape.StartPoint, ellipseShape.Point, dx, dy, scale);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is EllipseShape ellipseShape)
            {
                Draw(dc, renderer, ellipseShape, dx, dy, scale);
            }
        }
    }
}
