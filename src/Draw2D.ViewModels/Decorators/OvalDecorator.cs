// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class OvalDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, OvalShape ovalShape, double dx, double dy, double scale)
        {
            DrawRectangle(dc, renderer, ovalShape.StartPoint, ovalShape.Point, dx, dy, scale);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is OvalShape ovalShape)
            {
                Draw(dc, renderer, ovalShape, dx, dy, scale);
            }
        }
    }
}
