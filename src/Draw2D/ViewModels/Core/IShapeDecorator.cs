
namespace Draw2D.ViewModels
{
    public interface IShapeDecorator
    {
        void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale);
    }
}
