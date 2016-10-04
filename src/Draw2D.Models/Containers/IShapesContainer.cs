using System;
using System.Collections.ObjectModel;
using Draw2D.Models.Shapes;

namespace Draw2D.Models.Containers
{
    public interface IShapesContainer
    {
        double Width { get; set; }
        double Height { get; set; }
        ObservableCollection<LineShape> Guides { get; set; }
        ObservableCollection<BaseShape> Shapes { get; set; }
    }
}
