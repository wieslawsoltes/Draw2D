using System.Windows.Media;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class QuadraticBezierHelper
    {
        public static void Draw(DrawingContext dc, QuadraticBezierShape quadraticBezier)
        {
            var brushLines = Brushes.Cyan;
            var penLines = new Pen(brushLines, 2.0);
            var brushPoints = Brushes.Yellow;
            dc.DrawLine(penLines, quadraticBezier.StartPoint.AsPoint(), quadraticBezier.Point1.AsPoint());
            dc.DrawLine(penLines, quadraticBezier.Point1.AsPoint(), quadraticBezier.Point2.AsPoint());
            dc.DrawEllipse(brushPoints, null, quadraticBezier.StartPoint.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.Point1.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.Point2.AsPoint(), 4, 4);
        }
    }
}
