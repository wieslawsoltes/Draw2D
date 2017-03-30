using Draw2D.Models.Shapes;

namespace Draw2D.Models.Renderers.Helpers
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
