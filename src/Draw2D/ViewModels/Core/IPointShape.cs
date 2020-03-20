
namespace Draw2D.ViewModels
{
    public interface IPointShape : IBaseShape
    {
        double X { get; set; }
        double Y { get; set; }
        HAlign HAlign { get; set; }
        VAlign VAlign { get; set; }
        IBaseShape Template { get; set; }
    }
}
