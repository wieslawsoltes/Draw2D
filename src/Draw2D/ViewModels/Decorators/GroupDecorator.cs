// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class GroupDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, GroupShape group, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (selectionState.IsSelected(group))
            {
                DrawBoxFromPoints(dc, renderer, group, dx, dy, scale);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is GroupShape group)
            {
                Draw(dc, renderer, group, selectionState, dx, dy, scale);
            }
        }
    }
}
