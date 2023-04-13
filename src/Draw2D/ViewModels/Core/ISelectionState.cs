using System.Collections.Generic;

namespace Draw2D.ViewModels;

public interface ISelectionState : INode, IDirty
{
    IBaseShape Hovered { get; set; }
    IBaseShape Selected { get; set; }
    ISet<IBaseShape> Shapes { get; set; }
    void Hover(IBaseShape shape);
    void Dehover();
    bool IsSelected(IBaseShape shape);
    void Select(IBaseShape shape);
    void Deselect(IBaseShape shape);
    void Clear();
}