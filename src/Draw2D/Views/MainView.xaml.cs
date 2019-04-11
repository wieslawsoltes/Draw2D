// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Draw2D.ViewModels;

namespace Draw2D.Views
{
    public class IsCheckedToolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ToolBase tool && parameter is string name)
            {
                if (tool.Title == name)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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

        private ZoomBorder _zoomBorder;

        public MainView()
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

            DrawDirtyRects = VisualRoot.Renderer.DrawDirtyRects;
            DrawFps = VisualRoot.Renderer.DrawFps;

            if (this.DataContext is IToolContext ctx)
            {
                ctx.Zoom = _zoomBorder;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is IToolContext ctx)
            {
                ctx.Zoom = null;
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
