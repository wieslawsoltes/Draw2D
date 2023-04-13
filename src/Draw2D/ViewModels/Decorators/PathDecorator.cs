﻿using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators;

[DataContract(IsReference = true)]
public class PathDecorator : CommonDecorator
{
    public void Draw(object dc, IShapeRenderer renderer, PathShape path, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (selectionState.IsSelected(path))
        {
            DrawBoxFromPoints(dc, renderer, path, dx, dy, scale);
        }

        foreach (var shape in path.Shapes)
        {
            shape.Decorator?.Draw(dc, shape, renderer, selectionState, dx, dy, scale);
        }
    }

    public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (shape is PathShape path)
        {
            Draw(dc, renderer, path, selectionState, dx, dy, scale);
        }
    }
}