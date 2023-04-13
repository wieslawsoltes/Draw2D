using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators;

[DataContract(IsReference = true)]
public class ReferenceDecorator : CommonDecorator
{
    public void Draw(object dc, IShapeRenderer renderer, ReferenceShape reference, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (selectionState.IsSelected(reference) && reference.Template != null)
        {
            DrawBoxFromPoints(dc, renderer, reference.Template, dx + reference.X, dy + reference.Y, scale);
        }
    }

    public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
    {
        if (shape is ReferenceShape reference)
        {
            Draw(dc, renderer, reference, selectionState, dx, dy, scale);
        }
    }
}