// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Renderers;
using Core2D.ViewModels;

namespace Core2D.Avalonia
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();

            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();
            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new AvaloniaShapeRenderer();
            vm.Selected = vm.Renderer.Selected;

            DataContext = vm;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
