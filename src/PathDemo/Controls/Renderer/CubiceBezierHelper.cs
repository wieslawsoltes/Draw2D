using System;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class CubiceBezierHelper : CommonHelper
    {
        public static void Draw(object dc, ShapeRenderer r, CubicBezierShape cubicBezier)
        {
            DrawLineAt(dc, r, cubicBezier.StartPoint, cubicBezier.Point1);
            DrawLineAt(dc, r, cubicBezier.Point3, cubicBezier.Point2);
            DrawLineAt(dc, r, cubicBezier.Point1, cubicBezier.Point2);
            DrawEllipseAt(dc, r, cubicBezier.StartPoint, 4.0);
            DrawEllipseAt(dc, r, cubicBezier.Point1, 4.0);
            DrawEllipseAt(dc, r, cubicBezier.Point2, 4.0);
            DrawEllipseAt(dc, r, cubicBezier.Point3, 4.0);
        }
    }
}
