using Core2D.UI.Zoom.Input;
using Spatial;

namespace Draw2D.ViewModels
{
    public interface IBounds
    {
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier);
        IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest, Modifier modifier);
        IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest, Modifier modifier);
    }
}
