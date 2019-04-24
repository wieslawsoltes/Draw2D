// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class PathDecorator : CommonDecorator
    {
        private LineDecorator _lineDecorator;
        private CubicBezierDecorator _cubiceBezierDecorator;
        private QuadraticBezierDecorator _quadraticBezierDecorator;

        public PathDecorator()
        {
            _lineDecorator = new LineDecorator();
            _cubiceBezierDecorator = new CubicBezierDecorator();
            _quadraticBezierDecorator = new QuadraticBezierDecorator();
        }

        public void DrawShape(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy, DrawMode mode)
        {
            if (shape is LineShape line)
            {
                if (selection.Selected.Contains(line))
                {
                    _lineDecorator.Draw(dc, line, renderer, selection, dx, dy, mode);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selection.Selected.Contains(cubicBezier))
                {
                    _cubiceBezierDecorator.Draw(dc, cubicBezier, renderer, selection, dx, dy, mode);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selection.Selected.Contains(quadraticBezier))
                {
                    _quadraticBezierDecorator.Draw(dc, quadraticBezier, renderer, selection, dx, dy, mode);
                }
            }
        }

        public void DrawFigure(object dc, IShapeRenderer renderer, FigureShape figure, ISelection selection, double dx, double dy, DrawMode mode)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selection, dx, dy, mode);
            }
        }

        public void Draw(object dc, IShapeRenderer renderer, PathShape path, ISelection selection, double dx, double dy, DrawMode mode)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, renderer, figure, selection, dx, dy, mode);
            }
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelection selection, double dx, double dy, DrawMode mode)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selection, dx, dy, mode);
            }
        }
    }
}
