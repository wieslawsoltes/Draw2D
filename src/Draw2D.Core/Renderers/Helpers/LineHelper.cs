using Draw2D.Models.Shapes;

namespace Draw2D.Models.Renderers.Helpers
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
