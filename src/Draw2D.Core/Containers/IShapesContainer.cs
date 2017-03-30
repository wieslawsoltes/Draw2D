using System.Collections.ObjectModel;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.Models.Containers
{
    public interface IShapesContainer
    {
        double Width { get; set; }
        double Height { get; set; }
        ObservableCollection<DrawStyle> Styles { get; set; }
        ObservableCollection<LineShape> Guides { get; set; }
        ObservableCollection<ShapeObject> Shapes { get; set; }
    }
}
