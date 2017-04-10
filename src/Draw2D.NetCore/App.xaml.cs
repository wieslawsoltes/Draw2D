// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Draw2D.Core;
using Draw2D.NetCore.Controls;
using Draw2D.NetCore.Renderers;
using Draw2D.NetCore.Views;

namespace Draw2D.NetCore
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Start()
        {
            var window = new MainWindow();
            var mainView = window.FindControl<MainView>("mainView");
            var rendererView = mainView.FindControl<ShapesContainerRenderView>("rendererView");

            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();
            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new AvaloniaShapeRenderer();
            vm.Selected = vm.Renderer.Selected;
            vm.Capture = () => MouseDevice.Instance.Capture(rendererView);
            vm.Release = () => MouseDevice.Instance.Capture(null);
            vm.Invalidate = () => rendererView.InvalidateVisual();

            window.DataContext = vm;
            window.ShowDialog();
            Run(window);
        }
    }
}
