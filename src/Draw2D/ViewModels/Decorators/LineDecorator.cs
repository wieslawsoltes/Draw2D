// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class LineDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, LineShape lineShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, lineShape.StartPoint, lineShape.Point, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is LineShape lineShape)
            {
                Draw(dc, renderer, lineShape, dx, dy, scale, mode);
            }
        }
    }
}
