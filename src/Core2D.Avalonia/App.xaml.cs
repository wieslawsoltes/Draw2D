// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Renderers;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;

namespace Core2D.Avalonia
{
    public class App : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            var bootstrapper = new Bootstrapper();
            LayerContainerViewModel vm = bootstrapper.CreateDemoViewModel();
            bootstrapper.CreateDemoContainer(vm);
            vm.Renderer = new AvaloniaShapeRenderer();

            var window = new MainWindow
            {
                DataContext = vm,
            };

            app.Run(window);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
