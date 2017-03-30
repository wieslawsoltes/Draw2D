using System;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class CommonHelper
    {
        public static FillStyle = new DrawStyle(new DrawColor(255, 0, 255, 255), new DrawColor(255, 0, 255, 255), 2.0, false, true);
        public static StrokeStyle = new DrawStyle(new DrawColor(255, 0, 255, 255), new DrawColor(255, 0, 255, 255), 2.0, true, false);

        public static EllipseShape Ellipse = new EllipseShape(new PointShape(0, 0, null), new PointShape(0, 0, null));
        public static LineShape Line = new LineShape(new PointShape(0, 0, null), new PointShape(0, 0, null));

        public static void DrawEllipseAt(object dc, ShapeRenderer r, PointShape s, double radius)
        {
            Ellipse.Style = FillStyle;
            Ellipse.TopLeft.X = s.X - radius;
            Ellipse.TopLeft.Y = s.Y - radius;
            Ellipse.BottomRight.X = s.X + radius;
            Ellipse.BottomRight.Y = s.Y + radius;
            Ellipse.Draw(dc, r, 0.0, 0.0);
        }

        public static void DrawLineAt(object dc, ShapeRenderer r, PointShape a, PointShape b)
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
