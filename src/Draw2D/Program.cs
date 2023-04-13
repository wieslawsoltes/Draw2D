using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.OpenGL;
using Avalonia.Win32;

namespace Draw2D;

internal class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        return 0;
    }

    private static AngleOptions GetAngleOptions()
    {
        return new AngleOptions()
        {
            AllowedPlatformApis = new List<AngleOptions.PlatformApi>
            {
                AngleOptions.PlatformApi.DirectX11
            }
        };
    }

    private static Win32PlatformOptions GetWin32PlatformOptions()
    {
        return new Win32PlatformOptions
        {
            AllowEglInitialization = true
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
            UseGpu = true,
        };
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(GetWin32PlatformOptions())
            .With(GetAngleOptions())
            .With(GetX11PlatformOptions())
            .With(GetAvaloniaNativePlatformOptions())
            .UseSkia()
            .LogToTrace();
}
