using System.Windows.Media;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class LineHelper
    {
        public static void Draw(DrawingContext dc, LineShape line)
        {
            var brushPoints = Brushes.Yellow;
            dc.DrawEllipse(brushPoints, null, line.StartPoint.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, line.Point.AsPoint(), 4, 4);
        }
    }
}
