using System.Windows.Media;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace PathDemo.Controls
{
    public class CommonHelper
    {
        public static EllipseShape Ellipse = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
        {
            Style = new DrawStyle(new DrawColor(255, 255, 255, 0), new DrawColor(0, 0, 0, 0), 2.0, false, true)
        };

        public static void DrawEllipseAt(object dc, ShapeRenderer r, PointShape s, double radius)
        {
            Ellipse.TopLeft.X = s.X - radius;
            Ellipse.TopLeft.Y = s.Y - radius;
            Ellipse.BottomRight.X = s.X + radius;
            Ellipse.BottomRight.Y = s.Y + radius;
            Ellipse.Draw(dc, r, 0.0, 0.0);
        }
    }

    public class LineHelper : CommonHelper
    {
        public static void Draw(object dc, ShapeRenderer r, LineShape line)
        {
            DrawEllipseAt(dc, r, line.StartPoint, 2.0);
            DrawEllipseAt(dc, r, line.Point, 2.0);
        }
    }
}
