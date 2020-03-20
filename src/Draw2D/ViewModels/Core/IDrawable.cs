using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels
{
    public interface IDrawable
    {
        IBounds Bounds { get; }
        IShapeDecorator Decorator { get; }
        string StyleId { get; set; }
        IPaintEffects Effects { get; set; }
        void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r);
    }
}
