// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Draw2D.Views;

namespace Draw2D
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                if (App.ParseArgs(args) == true)
                {
                    BuildAvaloniaApp().Start(AppMain, args);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions
                         {
                             EnableMultitouch = true,
                             AllowEglInitialization = false
                         })
                         .With(new X11PlatformOptions
                         {
                             EnableMultiTouch = true,
                             UseGpu = true,
                             UseEGL = true
                         })
                         .With(new AvaloniaNativePlatformOptions
                         {
                             UseGpu = true
                         })
                         .UseSkia()
                         .LogToDebug();

        private static void AppMain(Application app, string[] args)
        {
            App.Load();

            var window = new MainWindow
            {
                DataContext = App.ToolContext
            };

            App.SetWindowSettings(window);

            window.Closing += (sender, e) =>
            {
                App.GetWindowSettings(window);
            };

            app.Run(window);

            App.Save();
        }
    }
}
