// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.Avalonia.Renderers;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;

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

        static void Main(string[] args)
        {
            try
            {
                var file = "vm.json";
                var bootstrapper = new Bootstrapper();

                LayerContainerViewModel vm;

                if (File.Exists(file))
                    vm = LayerContainerViewModel.Load(file);
                else
                    vm = bootstrapper.CreateDemoViewModel();

                bootstrapper.CreateDemoContainer(vm);

                vm.Renderer = new AvaloniaShapeRenderer();
                vm.Selected = vm.Renderer.Selected;

                BuildAvaloniaApp().Start<MainWindow>(() => vm);

                LayerContainerViewModel.Save(file, vm);
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                //.UseDirect2D1()
                //.UseSkia()
                .LogToDebug();
    }
}
