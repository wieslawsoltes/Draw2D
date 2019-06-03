// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class ContainerDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ICanvasContainer container, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(container))
            {
                DrawBoxFromPoints(dc, renderer, container, dx, dy, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is ICanvasContainer container)
            {
                Draw(dc, renderer, container, selectionState, dx, dy, scale, mode);
            }
        }
    }
}
