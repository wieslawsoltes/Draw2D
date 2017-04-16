// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using Draw2D.ViewModels;
using Draw2D.Wpf.Renderers;

namespace Draw2D.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow();
            var mainView = window.mainView;
            var rendererView = mainView.rendererView;

            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();
            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new WpfShapeRenderer();
            vm.Selected = vm.Renderer.Selected;
            vm.Capture = () => rendererView.CaptureMouse();
            vm.Release = () => rendererView.ReleaseMouseCapture();
            vm.Invalidate = () => rendererView.InvalidateVisual();

            window.DataContext = vm;
            window.Loaded += (sender, args) => mainView.Focus();
            window.ShowDialog();
        }
    }
}
