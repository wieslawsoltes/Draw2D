using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators;

[DataContract(IsReference = true)]
public class CubicBezierDecorator : CommonDecorator
{
    public void Draw(object dc, IShapeRenderer renderer, CubicBezierShape cubicBezier, double dx, double dy, double scale)
    {
        DrawLine(dc, renderer, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy, scale);
        DrawLine(dc, renderer, cubicBezier.Point3, cubicBezier.Point2, dx, dy, scale);
        DrawLine(dc, renderer, cubicBezier.Point1, cubicBezier.Point2, dx, dy, scale);
    }

    public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (shape is CubicBezierShape cubicBezier)
        {
            Draw(dc, renderer, cubicBezier, dx, dy, scale);
        }
    }
}