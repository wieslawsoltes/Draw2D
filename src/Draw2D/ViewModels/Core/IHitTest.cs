using System.Collections.Generic;
using Core2D.UI.Zoom.Input;
using Spatial;

namespace Draw2D.ViewModels
{
    public interface IHitTest
    {
        IPointShape TryToGetPoint(IEnumerable<IBaseShape> shapes, Point2 target, double radius, double scale, Modifier modifier, IPointShape exclude);
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, double scale, Modifier modifier);
        IBaseShape TryToGetShape(IEnumerable<IBaseShape> shapes, Point2 target, double radius, double scale, Modifier modifier);
        ISet<IBaseShape> TryToGetShapes(IEnumerable<IBaseShape> shapes, Rect2 target, double radius, double scale, Modifier modifier);
    }
}
