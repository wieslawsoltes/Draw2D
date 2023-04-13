using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators;

[DataContract(IsReference = true)]
public class ConicDecorator : CommonDecorator
{
    public void Draw(object dc, IShapeRenderer renderer, ConicShape conic, double dx, double dy, double scale)
    {
        DrawLine(dc, renderer, conic.StartPoint, conic.Point1, dx, dy, scale);
        DrawLine(dc, renderer, conic.Point1, conic.Point2, dx, dy, scale);
    }

    public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (shape is ConicShape conic)
        {
            Draw(dc, renderer, conic, dx, dy, scale);
        }
    }
}