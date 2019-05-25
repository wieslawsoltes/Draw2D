// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace Draw2D
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (AppData.ParseArgs(args) == true)
            {
                BuildAvaloniaApp().Start(AppMain, args);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions { AllowEglInitialization = false })
                         .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                         .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                         .UseSkia()
                         .LogToDebug();

        private static void AppMain(Application app, string[] args)
        {
            var window = AppData.Load();
            if (window != null)
            {
                app.Run(window);
                AppData.Save();
            }
            else
            {
                app.Shutdown();
            }
        }
    }
}
