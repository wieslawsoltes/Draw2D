using System;
using System.Linq;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Intersections.Line
{
    public class RectangleLineIntersection : PointIntersection
    {
        public override string Name { get { return "Rectangle-Line"; } }
        public RectangleLineSettings Settings { get; set; }

        public override void Find(IToolContext context, ShapeObject shape)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            if (!Settings.IsEnabled)
                return;

            var rectangles = context.CurrentContainer.Shapes.OfType<RectangleShape>();
            if (rectangles.Any())
            {
                foreach (var rectangle in rectangles)
                {
                    var rect = Rect2.FromPoints(rectangle.TopLeft.ToPoint2(), rectangle.BottomRight.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    double x0clip;
                    double y0clip;
                    double x1clip;
                    double y1clip;
                    var intersections = Line2.LineIntersectsWithRect(p1, p2, rect, out x0clip, out y0clip, out x1clip, out y1clip);
                    if (intersections)
                    {
                        var point1 = new PointShape(x0clip, y0clip, context.PointShape);
                        Intersections.Add(point1);
                        context.WorkingContainer.Shapes.Add(point1);
                        context.Selected.Add(point1);

                        var point2 = new PointShape(x1clip, y1clip, context.PointShape);
                        Intersections.Add(point2);
                        context.WorkingContainer.Shapes.Add(point2);
                        context.Selected.Add(point2);
                    }
                }
            }
        }
    }
}
