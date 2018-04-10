// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.Avalonia.Renderers;
using Core2D.ViewModels;

namespace Core2D.Avalonia
{
    class Program
    {
        static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }

        static object DataContextProvider()
        {
            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();

            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new AvaloniaShapeRenderer();
            vm.Selected = vm.Renderer.Selected;

            return vm;
        }

        static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp().Start<MainWindow>(() => DataContextProvider());
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
