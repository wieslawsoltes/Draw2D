// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Renderer;
using Draw2D.Shapes;

namespace Draw2D.Editor.Decorators
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

        public void DrawShape(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is LineShape line)
            {
                if (selection.Selected.Contains(line))
                {
                    _lineDecorator.Draw(dc, renderer, line, selection, dx, dy);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selection.Selected.Contains(cubicBezier))
                {
                    _cubiceBezierDecorator.Draw(dc, renderer, cubicBezier, selection, dx, dy);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selection.Selected.Contains(quadraticBezier))
                {
                    _quadraticBezierDecorator.Draw(dc, renderer, quadraticBezier, selection, dx, dy);
                }
            }
        }

        public void DrawFigure(object dc, IShapeRenderer renderer, FigureShape figure, ISelection selection, double dx, double dy)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selection, dx, dy);
            }
        }

        public void Draw(object dc, IShapeRenderer renderer, PathShape path, ISelection selection, double dx, double dy)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, renderer, figure, selection, dx, dy);
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selection, dx, dy);
            }
        }
    }
}
