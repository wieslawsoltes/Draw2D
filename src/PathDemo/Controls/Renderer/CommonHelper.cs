using System;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class CommonHelper
    {
        public DrawStyle FillStyle;
        public DrawStyle StrokeStyle;
        public EllipseShape Ellipse;
        public LineShape LineShape Line;

        public CommonHelper()
        {
            FillStyle = new DrawStyle(new DrawColor(255, 0, 255, 255), new DrawColor(255, 0, 255, 255), 2.0, false, true);
            StrokeStyle = new DrawStyle(new DrawColor(255, 0, 255, 255), new DrawColor(255, 0, 255, 255), 2.0, true, false);
            Ellipse = new EllipseShape(new PointShape(0, 0, null), new PointShape(0, 0, null));
            Line = new LineShape(new PointShape(0, 0, null), new PointShape(0, 0, null));
        }

        public void DrawEllipseAt(object dc, ShapeRenderer r, PointShape s, double radius)
        {
            Ellipse.Style = FillStyle;
            Ellipse.TopLeft.X = s.X - radius;
            Ellipse.TopLeft.Y = s.Y - radius;
            Ellipse.BottomRight.X = s.X + radius;
            Ellipse.BottomRight.Y = s.Y + radius;
            Ellipse.Draw(dc, r, 0.0, 0.0);
        }

        public void DrawLineAt(object dc, ShapeRenderer r, PointShape a, PointShape b)
        {
            Line.Style = StrokeStyle;
            Line.StartPoint.X = a.X;
            Line.StartPoint.Y = a.Y;
            Line.Point.X = b.X;
            Line.Point.Y = b.Y;
            Line.Draw(dc, r, 0.0, 0.0);
        }
    }
}
