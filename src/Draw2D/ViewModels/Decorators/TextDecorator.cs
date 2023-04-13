using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators;

[DataContract(IsReference = true)]
public class TextDecorator : CommonDecorator
{
    public void Draw(object dc, IShapeRenderer renderer, TextShape textShape, double dx, double dy, double scale)
    {
        DrawRectangle(dc, renderer, textShape.StartPoint, textShape.Point, dx, dy, scale);
    }

    public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (shape is TextShape textShape)
        {
            Draw(dc, renderer, textShape, dx, dy, scale);
        }
    }
}