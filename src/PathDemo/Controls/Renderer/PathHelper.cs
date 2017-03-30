using System;
using Draw2D.Editor;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class PathHelper : CommonHelper
    {
        private LineHelper _lineHelper;
        private CubiceBezierHelper _cubiceBezierHelper;
        private QuadraticBezierHelper _quadraticBezierHelper;

        public PathHelper()
        {
            _lineHelper = new LineHelper();
            _cubiceBezierHelper = new CubiceBezierHelper();
            _quadraticBezierHelper = new QuadraticBezierHelper();
        }

        public void DrawShape(object dc, ShapeRenderer r, ShapeObject shape, IToolContext context)
        {
            if (shape is LineShape line)
            {
                if (context.Selected.Contains(line))
                {
                    _lineHelper.Draw(dc, r, line);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (context.Selected.Contains(cubicBezier))
                {
                    _cubiceBezierHelper.Draw(dc, r, cubicBezier);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (context.Selected.Contains(quadraticBezier))
                {
                    _quadraticBezierHelper.Draw(dc, r, quadraticBezier);
                }
            }
        }

        public void DrawFigure(object dc, ShapeRenderer r, FigureShape figure, IToolContext context)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, r, shape, context);
            }
        }

        public void Draw(object dc, ShapeRenderer r, PathShape path, IToolContext context)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, r, figure, context);
            }
        }
    }
}
