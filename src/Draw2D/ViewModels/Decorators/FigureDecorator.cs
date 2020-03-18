// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class FigureDecorator : CommonDecorator
    {
        private readonly LineDecorator _lineDecorator;
        private readonly CubicBezierDecorator _cubiceBezierDecorator;
        private readonly QuadraticBezierDecorator _quadraticBezierDecorator;
        private readonly ConicDecorator _conicDecorator;

        public FigureDecorator()
        {
            _lineDecorator = new LineDecorator();
            _cubiceBezierDecorator = new CubicBezierDecorator();
            _quadraticBezierDecorator = new QuadraticBezierDecorator();
            _conicDecorator = new ConicDecorator();
        }

        public void DrawShape(object dc, IShapeRenderer renderer, IBaseShape shape, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is LineShape line)
            {
                if (selectionState.IsSelected(line))
                {
                    _lineDecorator.Draw(dc, line, renderer, selectionState, dx, dy, scale);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selectionState.IsSelected(cubicBezier))
                {
                    _cubiceBezierDecorator.Draw(dc, cubicBezier, renderer, selectionState, dx, dy, scale);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selectionState.IsSelected(quadraticBezier))
                {
                    _quadraticBezierDecorator.Draw(dc, quadraticBezier, renderer, selectionState, dx, dy, scale);
                }
            }
            else if (shape is ConicShape conicShape)
            {
                if (selectionState.IsSelected(conicShape))
                {
                    _conicDecorator.Draw(dc, conicShape, renderer, selectionState, dx, dy, scale);
                }
            }
        }

        public void DrawFigure(object dc, IShapeRenderer renderer, FigureShape figure, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (selectionState.IsSelected(figure))
            {
                DrawBoxFromPoints(dc, renderer, figure, dx, dy, scale);
            }

            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selectionState, dx, dy, scale);
            }
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is FigureShape figure)
            {
                DrawFigure(dc, renderer, figure, selectionState, dx, dy, scale);
            }
        }
    }
}
