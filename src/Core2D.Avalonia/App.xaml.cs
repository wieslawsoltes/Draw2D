// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Renderers;
using Core2D.ViewModels;

namespace Core2D.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        static object BuildDataContext()
        {
            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();

            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new AvaloniaShapeRenderer();
            vm.Selected = vm.Renderer.Selected;

            return vm;
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         //.UseDirect2D1()
                         //.UseSkia()
                         .LogToDebug();

        static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp().Start<MainWindow>(() => BuildDataContext());
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }
    }
}
