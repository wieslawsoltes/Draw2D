// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Draw2D.Editor;
using Draw2D.Views;

namespace Draw2D
{
    public class App : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp().Start(AppMain, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void AppMain(Application app, string[] args)
        {
            EditContainerView editContainerView = null;
            WindowSettings mainWindowSettings = null;

            if (File.Exists("editor.json"))
            {
                editContainerView = EditContainerView.LoadFromJson<EditContainerView>("editor.json");

                foreach (var containerView in editContainerView.ContainerViews)
                {
                    editContainerView.InitContainerView(containerView);
                }
            }
            else
            {
                editContainerView = new EditContainerView();
                editContainerView.Initialize();
            }

            editContainerView.Files = Directory
                .EnumerateFiles(Directory.GetCurrentDirectory(), "*.json")
                .Select(x => Path.GetFileName(x))
                .ToList();

            if (File.Exists("window.json"))
            {
                mainWindowSettings = EditContainerView.LoadFromJson<WindowSettings>("window.json");
            }
            else
            {
                mainWindowSettings = new WindowSettings()
                {
                    Width = 1320,
                    Height = 690,
                    X = double.NaN,
                    Y = double.NaN,
                    WindowState = WindowState.Normal
                };
            }

            var window = new MainWindow
            {
                DataContext = editContainerView
            };

            if (!double.IsNaN(mainWindowSettings.Width))
            {
                window.Width = mainWindowSettings.Width;
            }

            if (!double.IsNaN(mainWindowSettings.Height))
            {
                window.Height = mainWindowSettings.Height;
            }

            if (!double.IsNaN(mainWindowSettings.X) && !double.IsNaN(mainWindowSettings.Y))
            {
                window.Position = new PixelPoint((int)mainWindowSettings.X, (int)mainWindowSettings.Y);
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.WindowState = mainWindowSettings.WindowState;

            window.Closing += (sender, e) =>
            {
                mainWindowSettings.Width = window.Width;
                mainWindowSettings.Height = window.Height;
                mainWindowSettings.X = window.Position.X;
                mainWindowSettings.Y = window.Position.Y;
                mainWindowSettings.WindowState = window.WindowState;
            };

            app.Run(window);

            EditContainerView.SaveAsjson<EditContainerView>("editor.json", editContainerView);
            EditContainerView.SaveAsjson<WindowSettings>("window.json", mainWindowSettings);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions { AllowEglInitialization = true })
                         .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                         .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                         .UseSkia()
                         //.UseDirect2D1()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
