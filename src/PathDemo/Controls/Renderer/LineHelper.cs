using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class LineHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, LineShape line)
        {
            DrawEllipse(dc, r, line.StartPoint, 4.0);
            DrawEllipse(dc, r, line.Point, 4.0);
        }
    }
}
