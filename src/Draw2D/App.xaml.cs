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
    public class App : Application
    {
        private static string s_stylesPath = "styles.json";
        private static string s_editorPath = "editor.json";
        private static string s_windowPath = "window.json";
        private static IFactory s_factory = null;
        private static IStyleLibrary s_styleLibrary = null;
        private static IToolContext s_toolContext = null;
        private static WindowSettings s_windowSettings = null;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                s_factory = new Factory();

                if (args.Length == 1)
                {
                    var command = args[0];

                    if (command == "--new-styles")
                    {
                        var styleLibrary = s_factory.CreateStyleLibrary();
                        EditorToolContext.SaveAsjson("styles.json", styleLibrary);
                        return;
                    }

                    if (command == "--new-view")
                    {
                        var editorToolContext = s_factory.CreateToolContext();
                        var containerView = s_factory.CreateContainerView("View");
                        EditorToolContext.SaveAsjson(containerView.Title + ".json", containerView);
                        return;
                    }

                    if (command == "--new-editor")
                    {
                        var editorToolContext = s_factory.CreateToolContext();
                        EditorToolContext.SaveAsjson("editor.json", editorToolContext);
                        return;
                    }
                }
                else if (args.Length == 4)
                {
                    var command = args[0];

                    if (command == "--export")
                    {
                        var styleLibrary = EditorToolContext.LoadFromJson<IStyleLibrary>(args[1]);
                        var containerView = EditorToolContext.LoadFromJson<ContainerView>(args[2]);
                        EditorToolContext.Export(args[3], containerView, styleLibrary);
                        return;
                    }
                }

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
            if (File.Exists(s_stylesPath))
            {
                s_styleLibrary = EditorToolContext.LoadFromJson<IStyleLibrary>(s_stylesPath);
            }
            else
            {
                s_styleLibrary = s_factory.CreateStyleLibrary();
            }

            if (File.Exists(s_editorPath))
            {
                s_toolContext = EditorToolContext.LoadFromJson<IToolContext>(s_editorPath);
                s_toolContext.StyleLibrary = s_styleLibrary;

                if (s_toolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.Factory = s_factory;
                    foreach (var containerView in editorToolContext.ContainerViews)
                    {
                        editorToolContext.InitContainerView(containerView);
                    }
                }
            }
            else
            {
                s_toolContext = s_factory.CreateToolContext();
                s_toolContext.StyleLibrary = s_styleLibrary;

                if (s_toolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.Factory = s_factory;
                    editorToolContext.NewContainerView("View");
                    editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                    editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
                }
            }

            if (File.Exists(s_windowPath))
            {
                s_windowSettings = EditorToolContext.LoadFromJson<WindowSettings>(s_windowPath);
            }
            else
            {
                s_windowSettings = new WindowSettings()
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
                DataContext = s_toolContext
            };

            if (!double.IsNaN(s_windowSettings.Width))
            {
                window.Width = s_windowSettings.Width;
            }

            if (!double.IsNaN(s_windowSettings.Height))
            {
                window.Height = s_windowSettings.Height;
            }

            if (!double.IsNaN(s_windowSettings.X) && !double.IsNaN(s_windowSettings.Y))
            {
                window.Position = new PixelPoint((int)s_windowSettings.X, (int)s_windowSettings.Y);
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.WindowState = s_windowSettings.WindowState;

            window.Closing += (sender, e) =>
            {
                s_windowSettings.Width = window.Width;
                s_windowSettings.Height = window.Height;
                s_windowSettings.X = window.Position.X;
                s_windowSettings.Y = window.Position.Y;
                s_windowSettings.WindowState = window.WindowState;
            };

            app.Run(window);

            EditorToolContext.SaveAsjson(s_stylesPath, s_toolContext.StyleLibrary);
            EditorToolContext.SaveAsjson(s_editorPath, s_toolContext);
            EditorToolContext.SaveAsjson(s_windowPath, s_windowSettings);

            s_windowSettings = null;
            s_toolContext.Dispose();
            s_toolContext = null;
            s_styleLibrary = null;
            s_factory = null;
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions { AllowEglInitialization = true })
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
