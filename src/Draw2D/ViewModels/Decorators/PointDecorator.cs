// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class PointDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, IPointShape pointShape, double dx, double dy, double scale, DrawMode mode)
        {
            DrawRectangle(dc, renderer, pointShape, 10, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
#if USE_POINT_DECORATOR
            if (shape is IPointShape pointShape)
            {
                Draw(dc, renderer, pointShape, dx, dy, scale, mode);
            }
#endif
        }
    }
}
