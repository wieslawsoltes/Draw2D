using System;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class QuadraticBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, QuadraticBezierShape quadraticBezier)
        {
            DrawLineAt(dc, r, quadraticBezier.StartPoint, quadraticBezier.Point1);
            DrawLineAt(dc, r, quadraticBezier.Point1, quadraticBezier.Point2);
            DrawEllipseAt(dc, r, quadraticBezier.StartPoint, 4.0);
            DrawEllipseAt(dc, r, quadraticBezier.Point1, 4.0);
            DrawEllipseAt(dc, r, quadraticBezier.Point2, 4.0);
        }
    }
}
