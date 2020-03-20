using System.Collections.Generic;

namespace Draw2D.ViewModels
{
    public interface IConnectable
    {
        IList<IPointShape> Points { get; set; }
        bool Connect(IPointShape point, IPointShape target);
        bool Disconnect(IPointShape point, out IPointShape result);
        bool Disconnect();
    }
}
