using System;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class PathHelper
    {
        public static void Draw(DrawingContext dc, PathShape path, IToolContext context)
        {
            foreach (var figure in path.Figures)
            {
                foreach (var shape in figure.Shapes)
                {
                    if (shape is LineShape line)
                    {
                        if (context.Selected.Contains(line))
                        {
                            LineHelper.Draw(dc, line);
                        }
                    }
                    else if (shape is CubicBezierShape cubicBezier)
                    {
                        if (context.Selected.Contains(cubicBezier))
                        {
                            CubiceBezierHelper.Draw(dc, cubicBezier);
                        }
                    }
                    else if (shape is QuadraticBezierShape quadraticBezier)
                    {
                        if (context.Selected.Contains(quadraticBezier))
                        {
                            QuadraticBezierHelper.Draw(dc, quadraticBezier);
                        }
                    }
                }
            }
        }
    }
}
