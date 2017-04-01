using System.Collections.Generic;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Renderers.Helpers
{
    public class CubicBezierHelper : CommonHelper
    {
        public void Draw(object dc, ShapeRenderer r, CubicBezierShape cubicBezier)
        {
            DrawLine(dc, r, cubicBezier.StartPoint, cubicBezier.Point1);
            DrawLine(dc, r, cubicBezier.Point3, cubicBezier.Point2);
            DrawLine(dc, r, cubicBezier.Point1, cubicBezier.Point2);
            DrawEllipse(dc, r, cubicBezier.StartPoint, 4.0);
            DrawEllipse(dc, r, cubicBezier.Point1, 4.0);
            DrawEllipse(dc, r, cubicBezier.Point2, 4.0);
            DrawEllipse(dc, r, cubicBezier.Point3, 4.0);
        }

        public override void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, r, cubicBezier);
            }
        }
    }
}
