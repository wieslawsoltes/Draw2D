﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Avalonia.Controls
{
    public class ShapesContainerRenderView : Canvas
    {
        private bool _drawWorking = false;

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            _drawWorking = true;
            this.InvalidateVisual();
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            _drawWorking = false;
            this.InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.Presenter.DrawContainer(context, vm.CurrentContainer, vm.Renderer, 0.0, 0.0);

                if (_drawWorking)
                {
                    vm.Presenter.DrawContainer(context, vm.WorkingContainer, vm.Renderer, 0.0, 0.0);
                }

                vm.Presenter.DrawHelpers(context, vm.CurrentContainer, vm.Renderer, 0.0, 0.0);

                if (_drawWorking)
                {
                    vm.Presenter.DrawHelpers(context, vm.WorkingContainer, vm.Renderer, 0.0, 0.0);
                }
            }
        }
    }
}