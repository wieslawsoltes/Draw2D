// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class ArcDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ArcShape arcShape, double dx, double dy, double scale)
        {
            var distance = arcShape.StartPoint.DistanceTo(arcShape.Point);

            DrawRectangle(dc, renderer, arcShape.StartPoint.X, arcShape.StartPoint.Y, distance, dx, dy, scale);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is ArcShape arcShape)
            {
                Draw(dc, renderer, arcShape, dx, dy, scale);
            }
        }
    }
}
