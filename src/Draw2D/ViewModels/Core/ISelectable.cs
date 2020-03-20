
namespace Draw2D.ViewModels
{
    public interface ISelectable
    {
        void Move(ISelectionState selectionState, double dx, double dy);
        void Select(ISelectionState selectionState);
        void Deselect(ISelectionState selectionState);
    }
}
