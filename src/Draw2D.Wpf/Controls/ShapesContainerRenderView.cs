// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Wpf.Controls
{
    public class ShapesContainerRenderView : Canvas
    {
        private bool _drawWorking = false;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _drawWorking = true;
            this.InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _drawWorking = false;
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.Presenter.DrawContainer(dc, vm.CurrentContainer, vm.Renderer, 0.0, 0.0);

                if (_drawWorking)
                {
                    vm.Presenter.DrawContainer(dc, vm.WorkingContainer, vm.Renderer, 0.0, 0.0);
                }

                vm.Presenter.DrawHelpers(dc, vm.CurrentContainer, vm.Renderer, 0.0, 0.0);

                if (_drawWorking)
                {
                    vm.Presenter.DrawHelpers(dc, vm.WorkingContainer, vm.Renderer, 0.0, 0.0);
                }
            }
        }
    }
}
