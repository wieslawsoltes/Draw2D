// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class CubicBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, CubicBezierShape cubicBezier, double dx, double dy, double scale, DrawMode mode)
        {
            DrawLine(dc, renderer, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy, scale, mode);
            DrawLine(dc, renderer, cubicBezier.Point3, cubicBezier.Point2, dx, dy, scale, mode);
            DrawLine(dc, renderer, cubicBezier.Point1, cubicBezier.Point2, dx, dy, scale, mode);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, renderer, cubicBezier, dx, dy, scale, mode);
            }
        }
    }
}
