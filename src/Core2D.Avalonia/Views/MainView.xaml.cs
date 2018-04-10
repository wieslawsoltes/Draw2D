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
        public static readonly StyledProperty<bool> DrawDirtyRectsProperty =
            AvaloniaProperty.Register<MainView, bool>(nameof(DrawDirtyRects));

        public bool DrawDirtyRects
        {
            get { return GetValue(DrawDirtyRectsProperty); }
            set { SetValue(DrawDirtyRectsProperty, value); }
        }

        public static readonly StyledProperty<bool> DrawFpsProperty =
            AvaloniaProperty.Register<MainView, bool>(nameof(DrawFps));

        public bool DrawFps
        {
            get { return GetValue(DrawFpsProperty); }
            set { SetValue(DrawFpsProperty, value); }
        }

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
                var zoomBorder = this.FindControl<ZoomBorder>("zoomBorder");
                vm.Reset = () => zoomBorder.Reset();
                vm.AutoFit = () => zoomBorder.AutoFit();
                vm.StretchNone = () => zoomBorder.Stretch = PanAndZoom.StretchMode.None;
                vm.StretchFill = () => zoomBorder.Stretch = PanAndZoom.StretchMode.Fill;
                vm.StretchUniform = () => zoomBorder.Stretch = PanAndZoom.StretchMode.Uniform;
                vm.StretchUniformToFill = () => zoomBorder.Stretch = PanAndZoom.StretchMode.UniformToFill;
            }

            DrawDirtyRects = VisualRoot.Renderer.DrawDirtyRects;
            DrawFps = VisualRoot.Renderer.DrawFps;
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
        }

        private void DebugDrawDirtyRects_Click(object sender, RoutedEventArgs e)
        {
            VisualRoot.Renderer.DrawDirtyRects = !VisualRoot.Renderer.DrawDirtyRects;
            DrawDirtyRects = VisualRoot.Renderer.DrawDirtyRects;
        }

        private void DebugDrawFps_Click(object sender, RoutedEventArgs e)
        {
            VisualRoot.Renderer.DrawFps = !VisualRoot.Renderer.DrawFps;
            DrawFps = VisualRoot.Renderer.DrawFps;
        }
    }
}
