// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class PathDecorator : CommonDecorator
    {
        public PathDecorator()
        {
        }

        public void Draw(object dc, IShapeRenderer renderer, PathShape path, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (selectionState.IsSelected(path))
            {
                DrawBoxFromPoints(dc, renderer, path, dx, dy, scale, mode);
            }

            foreach (var shape in path.Shapes)
            {
                shape.Decorator?.Draw(dc, shape, renderer, selectionState, dx, dy, scale, mode);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selectionState, dx, dy, scale, mode);
            }
        }
    }
}
