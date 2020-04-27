using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers
{
    public interface IContainerView : IDrawTarget, IHitTestable, ICopyable
    {
        string Title { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        IPaint PrintBackground { get; set; }
        IPaint WorkBackground { get; set; }
        IPaint InputBackground { get; set; }
        ICanvasContainer CurrentContainer { get; set; }
        ICanvasContainer WorkingContainer { get; set; }
        ISelectionState SelectionState { get; set; }
        IZoomServiceState ZoomServiceState { get; set; }
        IContainerPresenter ContainerPresenter { get; set; }
        void Add(IBaseShape shape);
        void Remove(IBaseShape shape);
        void Reference(IBaseShape shape);
        void Style(string styleId);
    }
}
