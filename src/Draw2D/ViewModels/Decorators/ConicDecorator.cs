// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class ConicDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ConicShape conic, double dx, double dy, double scale, DrawMode mode)
        {
            DrawLine(dc, renderer, conic.StartPoint, conic.Point1, dx, dy, scale, mode);
            DrawLine(dc, renderer, conic.Point1, conic.Point2, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is ConicShape conic)
            {
                Draw(dc, renderer, conic, dx, dy, scale, mode);
            }
        }
    }
}
