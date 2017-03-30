using System;
using Draw2D.Editor;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class PathHelper : CommonHelper
    {
        public static void Draw(object dc, ShapeRenderer r, PathShape path, IToolContext context)
        {
            foreach (var figure in path.Figures)
            {
                foreach (var shape in figure.Shapes)
                {
                    if (shape is LineShape line)
                    {
                        if (context.Selected.Contains(line))
                        {
                            LineHelper.Draw(dc, r, line);
                        }
                    }
                    else if (shape is CubicBezierShape cubicBezier)
                    {
                        if (context.Selected.Contains(cubicBezier))
                        {
                            CubiceBezierHelper.Draw(dc, r, cubicBezier);
                        }
                    }
                    else if (shape is QuadraticBezierShape quadraticBezier)
                    {
                        if (context.Selected.Contains(quadraticBezier))
                        {
                            QuadraticBezierHelper.Draw(dc, r, quadraticBezier);
                        }
                    }
                }
            }
        }
    }
}
