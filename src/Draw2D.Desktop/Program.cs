// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace Draw2D.Desktop
{
    class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            try
            {
                if (App.ParseArgs(args) == true)
                {
                    return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
            return 0;
        }

        private static Win32PlatformOptions GetWin32PlatformOptions()
        {
            return new Win32PlatformOptions
            {
                EnableMultitouch = true,
                AllowEglInitialization = false
            };
        }

        private static X11PlatformOptions GetX11PlatformOptions()
        {
            return new X11PlatformOptions
            {
                EnableMultiTouch = true,
                UseGpu = true,
                UseEGL = true
            };
        }

        private static AvaloniaNativePlatformOptions GetAvaloniaNativePlatformOptions()
        {
            return new AvaloniaNativePlatformOptions
            {
                UseGpu = true
            };
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(GetWin32PlatformOptions())
                         .With(GetX11PlatformOptions())
                         .With(GetAvaloniaNativePlatformOptions())
                         .UseSkia()
                         .LogToDebug();
    }
}
