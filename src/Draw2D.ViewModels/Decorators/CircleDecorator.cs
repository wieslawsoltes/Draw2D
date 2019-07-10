// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class CircleDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, CircleShape circleShape, double dx, double dy, double scale)
        {
            var distance = circleShape.StartPoint.DistanceTo(circleShape.Point);

            DrawRectangle(dc, renderer, circleShape.StartPoint.X, circleShape.StartPoint.Y, distance, dx, dy, scale);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is CircleShape circleShape)
            {
                Draw(dc, renderer, circleShape, dx, dy, scale);
            }
        }
    }
}
