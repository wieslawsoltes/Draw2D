using System.Collections.Generic;

namespace Draw2D.ViewModels.Tools
{
    public interface IPointFilter : INode, IDirty
    {
        IList<IBaseShape> Guides { get; set; }
        bool Process(IToolContext context, ref double x, ref double y);
        void Clear(IToolContext context);
    }
}
