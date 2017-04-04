// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using Draw2D.Core;
using Draw2D.Wpf.Renderers;
using Draw2D.Wpf.Views;

namespace Draw2D.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainView = new MainView();
            var rendererView = mainView.RendererView;

            var vm = new Bootstrapper().CreateViewModel();

            vm.Renderer = new WpfShapeRenderer();
            vm.Selected = vm.Renderer.Selected;
            vm.Capture = () => rendererView.CaptureMouse();
            vm.Release = () => rendererView.ReleaseMouseCapture();
            vm.Invalidate = () => rendererView.InvalidateVisual();

            mainView.DataContext = vm;
            mainView.ShowDialog();
        }
    }
}
