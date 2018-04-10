// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Core2D.ViewModels.Containers;

namespace Core2D.Avalonia.Views
{
    public class MainView : UserControl
    {
        private ZoomBorder zoomBorder;

        public MainView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
                vm.Reset = () => zoomBorder.Reset();
                vm.AutoFit = () => zoomBorder.AutoFit();
                vm.StretchNone = () => zoomBorder.Stretch = PanAndZoom.StretchMode.None;
                vm.StretchFill = () => zoomBorder.Stretch = PanAndZoom.StretchMode.Fill;
                vm.StretchUniform = () => zoomBorder.Stretch = PanAndZoom.StretchMode.Uniform;
                vm.StretchUniformToFill = () => zoomBorder.Stretch = PanAndZoom.StretchMode.UniformToFill;
            }

            this.FindControl<MenuItem>("DebugDrawDirtyRects").Click += DebugDrawDirtyRects_Click;
            this.FindControl<MenuItem>("DebugDrawFps").Click += DebugDrawFps_Click;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is LayerContainerViewModel vm)
            {
                vm.Reset = null;
                vm.AutoFit = null;
                vm.StretchNone = null;
                vm.StretchFill = null;
                vm.StretchUniform = null;
                vm.StretchUniformToFill = null;
            }

            this.FindControl<MenuItem>("DebugDrawDirtyRects").Click -= DebugDrawDirtyRects_Click;
            this.FindControl<MenuItem>("DebugDrawFps").Click -= DebugDrawFps_Click;
        }

        private void DebugDrawDirtyRects_Click(object sender, RoutedEventArgs e)
        {
            bool value = !VisualRoot.Renderer.DrawDirtyRects;
            VisualRoot.Renderer.DrawDirtyRects = value;
            this.FindControl<CheckBox>("DebugDrawDirtyRectsCheckBox").IsChecked = value;
        }

        private void DebugDrawFps_Click(object sender, RoutedEventArgs e)
        {
            bool value = !VisualRoot.Renderer.DrawFps;
            VisualRoot.Renderer.DrawFps = value;
            this.FindControl<CheckBox>("DebugDrawFpsCheckBox").IsChecked = value;
        }
    }
}
