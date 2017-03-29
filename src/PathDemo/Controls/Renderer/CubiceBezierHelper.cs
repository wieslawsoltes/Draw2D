using System.Windows.Media;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class CubiceBezierHelper
    {
        public static void Draw(DrawingContext dc, CubicBezierShape cubicBezier)
        {
            var brushLines = Brushes.Cyan;
            var penLines = new Pen(brushLines, 2.0);
            var brushPoints = Brushes.Yellow;
            dc.DrawLine(penLines, cubicBezier.StartPoint.AsPoint(), cubicBezier.Point1.AsPoint());
            dc.DrawLine(penLines, cubicBezier.Point3.AsPoint(), cubicBezier.Point2.AsPoint());
            dc.DrawLine(penLines, cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint());
            dc.DrawEllipse(brushPoints, null, cubicBezier.StartPoint.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point1.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point2.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point3.AsPoint(), 4, 4);
        }
    }
}
