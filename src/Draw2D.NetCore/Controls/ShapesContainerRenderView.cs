// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Media;
using Draw2D.Core.ViewModels.Containers;

namespace Draw2D.NetCore.Controls
{
    public class ShapesContainerRenderView : Canvas
    {
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.Presenter.Draw(context, vm);
                vm.Presenter.DrawHelpers(context, vm);
            }
        }
    }
}
