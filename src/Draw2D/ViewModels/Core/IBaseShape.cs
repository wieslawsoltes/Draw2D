using System.Collections.Generic;

namespace Draw2D.ViewModels;

public interface IBaseShape : INode, ICopyable, IDirty, ISelectable, IDrawable
{
    void GetPoints(IList<IPointShape> points);
    bool IsPointsTreeDirty();
}