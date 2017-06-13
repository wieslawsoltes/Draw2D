// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderer;
using Draw2D.Core.Shape;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Tools.Helpers
{
    public class PathHelper : CommonHelper
    {
        private LineHelper _lineHelper;
        private CubicBezierHelper _cubiceBezierHelper;
        private QuadraticBezierHelper _quadraticBezierHelper;

        public PathHelper()
        {
            _lineHelper = new LineHelper();
            _cubiceBezierHelper = new CubicBezierHelper();
            _quadraticBezierHelper = new QuadraticBezierHelper();
        }

        public void DrawShape(object dc, ShapeRenderer renderer, BaseShape shape, ISet<BaseShape> selected, double dx, double dy)
        {
            if (shape is LineShape line)
            {
                if (selected.Contains(line))
                {
                    _lineHelper.Draw(dc, renderer, line, selected, dx, dy);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selected.Contains(cubicBezier))
                {
                    _cubiceBezierHelper.Draw(dc, renderer, cubicBezier, selected, dx, dy);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selected.Contains(quadraticBezier))
                {
                    _quadraticBezierHelper.Draw(dc, renderer, quadraticBezier, selected, dx, dy);
                }
            }
        }

        public void DrawFigure(object dc, ShapeRenderer renderer, FigureShape figure, ISet<BaseShape> selected, double dx, double dy)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selected, dx, dy);
            }
        }

        public void Draw(object dc, ShapeRenderer renderer, PathShape path, ISet<BaseShape> selected, double dx, double dy)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, renderer, figure, selected, dx, dy);
            }
        }

        public override void Draw(object dc, ShapeRenderer renderer, BaseShape shape, ISet<BaseShape> selected, double dx, double dy)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selected, dx, dy);
            }
        }
    }
}
