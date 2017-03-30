using System;
using System.Collections.Generic;
using Draw2D.Models;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class QuadraticBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, QuadraticBezierShape quadraticBezier)
        {
            DrawLine(dc, r, quadraticBezier.StartPoint, quadraticBezier.Point1);
            DrawLine(dc, r, quadraticBezier.Point1, quadraticBezier.Point2);
            DrawEllipse(dc, r, quadraticBezier.StartPoint, 4.0);
            DrawEllipse(dc, r, quadraticBezier.Point1, 4.0);
            DrawEllipse(dc, r, quadraticBezier.Point2, 4.0);
        }
    }
}
