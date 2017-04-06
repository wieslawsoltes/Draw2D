// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor.Tools.Helpers
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

        public void DrawShape(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected)
        {
            if (shape is LineShape line)
            {
                if (selected.Contains(line))
                {
                    _lineHelper.Draw(dc, r, line);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selected.Contains(cubicBezier))
                {
                    _cubiceBezierHelper.Draw(dc, r, cubicBezier);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selected.Contains(quadraticBezier))
                {
                    _quadraticBezierHelper.Draw(dc, r, quadraticBezier);
                }
            }
        }

        public void DrawFigure(object dc, ShapeRenderer r, FigureShape figure, ISet<ShapeObject> selected)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, r, shape, selected);
            }
        }

        public void Draw(object dc, ShapeRenderer r, PathShape path, ISet<ShapeObject> selected)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, r, figure, selected);
            }
        }

        public override void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected)
        {
            if (shape is PathShape path)
            {
                Draw(dc, r, path, selected);
            }
        }
    }
}
