using System;
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

            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                foreach (var shape in vm.Container.Guides)
                {
                    shape.Draw(dc, vm.Renderer, 0.0, 0.0);
                }

                foreach (var shape in vm.Container.Shapes)
                {
                    shape.Draw(dc, vm.Renderer, 0.0, 0.0);
                }

                foreach (var shape in vm.WorkingContainer.Shapes)
                {
                    shape.Draw(dc, vm.Renderer, 0.0, 0.0);
                }
            }
        }
    }
}
