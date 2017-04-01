using System.Windows.Controls;
using System.Windows.Media;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Wpf.Controls
{
    public class ShapesContainerRenderView : Canvas
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.Presenter.Draw(dc, vm);
                vm.Presenter.DrawHelpers(dc, vm);
            }
        }
    }
}
