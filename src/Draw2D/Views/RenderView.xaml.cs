// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views
{
    public partial class RenderView : UserControl
    {
        private ZoomBorder _zoomBorder;

        public RenderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is MainViewModel vm)
            {
                vm.Reset = () => _zoomBorder.Reset();
                vm.AutoFit = () => _zoomBorder.AutoFit();
                vm.StretchNone = () => _zoomBorder.Stretch = PanAndZoom.StretchMode.None;
                vm.StretchFill = () => _zoomBorder.Stretch = PanAndZoom.StretchMode.Fill;
                vm.StretchUniform = () => _zoomBorder.Stretch = PanAndZoom.StretchMode.Uniform;
                vm.StretchUniformToFill = () => _zoomBorder.Stretch = PanAndZoom.StretchMode.UniformToFill;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is MainViewModel vm)
            {
                vm.Reset = null;
                vm.AutoFit = null;
                vm.StretchNone = null;
                vm.StretchFill = null;
                vm.StretchUniform = null;
                vm.StretchUniformToFill = null;
            }
        }
    }
}
