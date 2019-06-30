// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class RectangleDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, RectangleShape rectangleShape, double dx, double dy, double scale)
        {
            DrawRectangle(dc, renderer, rectangleShape.StartPoint, rectangleShape.Point, dx, dy, scale);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is RectangleShape rectangleShape)
            {
                Draw(dc, renderer, rectangleShape, dx, dy, scale);
            }
        }
    }
}
