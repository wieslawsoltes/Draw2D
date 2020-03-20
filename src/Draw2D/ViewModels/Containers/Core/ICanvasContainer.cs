using System.Collections.Generic;

namespace Draw2D.ViewModels.Containers
{
    public interface ICanvasContainer : IBaseShape
    {
        IList<IBaseShape> Shapes { get; set; }
    }
}
