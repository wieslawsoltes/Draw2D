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
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1)
                {
                    var command = args[0];

                    if (command == "--new-view")
                    {
                        var factory = new Factory();
                        var editorToolContext = factory.CreateToolContext();
                        var containerView = factory.CreateContainerView("View");
                        EditorToolContext.SaveAsjson(containerView.Title + ".json", containerView);
                        return;
                    }

                    if (command == "--new-editor")
                    {
                        var factory = new Factory();
                        var editorToolContext = factory.CreateToolContext();
                        EditorToolContext.SaveAsjson("editor.json", editorToolContext);
                        return;
                    }
                }
                else if (args.Length == 2)
                {
                    var containerView = EditorToolContext.LoadFromJson<ContainerView>(args[1]);
                    EditorToolContext.Export(args[1], containerView);
                    return;
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
            IToolContext toolContext = null;
            WindowSettings windowSettings = null;

            if (File.Exists("editor.json"))
            {
                toolContext = EditorToolContext.LoadFromJson<IToolContext>("editor.json");

                // TODO: Refactor to not use concrete tool context type.
                if (toolContext is EditorToolContext editorToolContext)
                {
                    foreach (var containerView in editorToolContext.ContainerViews)
                    {
                        editorToolContext.InitContainerView(containerView);
                    }
                }
            }
            else
            {
                var factory = new Factory();
                toolContext = factory.CreateToolContext();

                // TODO: Refactor to not use concrete tool context type.
                if (toolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.NewContainerView("View");
                    editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                    editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
                }
            }

            if (File.Exists("window.json"))
            {
                windowSettings = EditorToolContext.LoadFromJson<WindowSettings>("window.json");
            }
            else
            {
                windowSettings = new WindowSettings()
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
                DataContext = toolContext
            };

            if (!double.IsNaN(windowSettings.Width))
            {
                window.Width = windowSettings.Width;
            }

            if (!double.IsNaN(windowSettings.Height))
            {
                window.Height = windowSettings.Height;
            }

            if (!double.IsNaN(windowSettings.X) && !double.IsNaN(windowSettings.Y))
            {
                window.Position = new PixelPoint((int)windowSettings.X, (int)windowSettings.Y);
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.WindowState = windowSettings.WindowState;

            window.Closing += (sender, e) =>
            {
                windowSettings.Width = window.Width;
                windowSettings.Height = window.Height;
                windowSettings.X = window.Position.X;
                windowSettings.Y = window.Position.Y;
                windowSettings.WindowState = window.WindowState;
            };

            app.Run(window);

            EditorToolContext.SaveAsjson("editor.json", toolContext);
            EditorToolContext.SaveAsjson("window.json", windowSettings);
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
