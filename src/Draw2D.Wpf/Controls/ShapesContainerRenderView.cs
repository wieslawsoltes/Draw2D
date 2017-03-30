using System;
using System.Windows.Controls;
using System.Windows.Media;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Wpf.Controls
{
    public class ShapesContainerRenderView : Canvas
    {
        private void Draw(DrawingContext dc, ShapesContainerViewModel vm)
        {
            foreach (var shape in vm.CurrentContainer.Guides)
            {
                shape.Draw(dc, vm.Renderer, 0.0, 0.0);
            }

            foreach (var shape in vm.CurrentContainer.Shapes)
            {
                shape.Draw(dc, vm.Renderer, 0.0, 0.0);
            }

            foreach (var shape in vm.WorkingContainer.Shapes)
            {
                shape.Draw(dc, vm.Renderer, 0.0, 0.0);
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Draw(dc, vm);
            }
        }
    }
}
