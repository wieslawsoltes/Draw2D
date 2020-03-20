using System.Collections.Generic;

namespace Draw2D.ViewModels.Tools
{
    public interface IPointIntersection : INode, IDirty
    {
        IList<IPointShape> Intersections { get; set; }
        void Find(IToolContext context, IBaseShape shape);
        void Clear(IToolContext context);
    }
}
