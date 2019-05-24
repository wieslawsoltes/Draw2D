// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Draw2D.Editor;
using Draw2D.ViewModels.Containers;
using Draw2D.Views;

namespace Draw2D
{
    public class AppData
    {
        public static string StylesPath { get; set; }
        public static string EditorPath { get; set; }
        public static string WindowPath { get; set; }
        public static IFactory Factory { get; set; }
        public static IStyleLibrary StyleLibrary { get; set; }
        public static IToolContext ToolContext { get; set; }
        public static WindowSettings WindowSettings { get; set; }

        public static void CreateFactory()
        {
            Factory = new EditorFactory();
        }

        public static void CreateStyleLibrary()
        {
            StyleLibrary = Factory.CreateStyleLibrary();
        }

        public static void CreateToolContext()
        {
            ToolContext = Factory.CreateToolContext();
            ToolContext.StyleLibrary = StyleLibrary;

            if (ToolContext is EditorToolContext editorToolContext)
            {
                editorToolContext.Factory = Factory;
                editorToolContext.NewContainerView("View");
                editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
            }
        }

        public static void InitContainerViews()
        {
            if (ToolContext is EditorToolContext editorToolContext)
            {
                editorToolContext.Factory = Factory;
                foreach (var containerView in editorToolContext.ContainerViews)
                {
                    editorToolContext.InitContainerView(containerView);
                }
            }
        }

        public static void CreateWindowSettings()
        {
            WindowSettings = new WindowSettings()
            {
                Width = 1320,
                Height = 690,
                X = double.NaN,
                Y = double.NaN,
                WindowState = WindowState.Normal
            };
        }

        public static void SetWindowSettings(Window window)
        {
            if (!double.IsNaN(WindowSettings.Width))
            {
                window.Width = WindowSettings.Width;
            }

            if (!double.IsNaN(WindowSettings.Height))
            {
                window.Height = WindowSettings.Height;
            }

            if (!double.IsNaN(WindowSettings.X) && !double.IsNaN(WindowSettings.Y))
            {
                window.Position = new PixelPoint((int)WindowSettings.X, (int)WindowSettings.Y);
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.WindowState = WindowSettings.WindowState;
        }

        public static void GetWindowSettings(Window window)
        {
            WindowSettings.Width = window.Width;
            WindowSettings.Height = window.Height;
            WindowSettings.X = window.Position.X;
            WindowSettings.Y = window.Position.Y;
            WindowSettings.WindowState = window.WindowState;
        }
    }

    public class App : Application
    {
        static App()
        {
            AppData.StylesPath = "styles.json";
            AppData.EditorPath = "editor.json";
            AppData.WindowPath = "window.json";

            AppData.CreateFactory();

            if (Design.IsDesignMode)
            {
                AppData.CreateStyleLibrary();
                AppData.CreateToolContext();
                AppData.InitContainerViews();
                AppData.CreateWindowSettings();
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                var command = args[0];

                if (command == "--new-styles")
                {
                    var styleLibrary = AppData.Factory.CreateStyleLibrary();
                    JsonSerializer.ToJsonFile("styles.json", styleLibrary);
                    return;
                }

                if (command == "--new-view")
                {
                    var containerView = AppData.Factory.CreateContainerView("View");
                    JsonSerializer.ToJsonFile(containerView.Title + ".json", containerView);
                    return;
                }

                if (command == "--new-editor")
                {
                    var toolContext = AppData.Factory.CreateToolContext();
                    JsonSerializer.ToJsonFile("editor.json", toolContext);
                    return;
                }

                if (command == "--demo")
                {
                    var toolContext = AppData.Factory.CreateToolContext();
                    var styleLibrary = AppData.Factory.CreateStyleLibrary();
                    toolContext.StyleLibrary = styleLibrary;
                    if (toolContext is EditorToolContext editorToolContext)
                    {
                        editorToolContext.NewContainerView("Demo");
                        editorToolContext.CreateDemoGroup(editorToolContext);
                        JsonSerializer.ToJsonFile("Demo.json", editorToolContext.ContainerView);
                    }
                    return;
                }
            }
            else if (args.Length == 4)
            {
                var command = args[0];

                if (command == "--export")
                {
                    var styleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(args[1]);
                    var containerView = JsonSerializer.FromJsonFile<ContainerView>(args[2]);
                    EditorToolContext.Export(args[3], containerView, styleLibrary);
                    return;
                }
            }

            BuildAvaloniaApp().Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            if (File.Exists(AppData.StylesPath))
            {
                AppData.StyleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(AppData.StylesPath);
            }
            else
            {
                AppData.CreateStyleLibrary();
            }

            if (File.Exists(AppData.EditorPath))
            {
                AppData.ToolContext = JsonSerializer.FromJsonFile<IToolContext>(AppData.EditorPath);
                AppData.ToolContext.StyleLibrary = AppData.StyleLibrary;
                AppData.InitContainerViews();
            }
            else
            {
                AppData.CreateToolContext();
            }

            if (File.Exists(AppData.WindowPath))
            {
                AppData.WindowSettings = JsonSerializer.FromJsonFile<WindowSettings>(AppData.WindowPath);
            }
            else
            {
                AppData.CreateWindowSettings();
            }

            var window = new MainWindow
            {
                DataContext = AppData.ToolContext
            };

            AppData.SetWindowSettings(window);

            window.Closing += (sender, e) =>
            {
                AppData.GetWindowSettings(window);
            };

            app.Run(window);

            JsonSerializer.ToJsonFile(AppData.StylesPath, AppData.ToolContext.StyleLibrary);
            JsonSerializer.ToJsonFile(AppData.EditorPath, AppData.ToolContext);
            JsonSerializer.ToJsonFile(AppData.WindowPath, AppData.WindowSettings);

            AppData.WindowSettings = null;
            AppData.ToolContext.Dispose();
            AppData.ToolContext = null;
            AppData.StyleLibrary = null;
            AppData.Factory = null;
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions { AllowEglInitialization = false })
                         .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                         .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                         .UseSkia()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
