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
                        var editorToolContext = EditorToolContext.Create();
                        var containerView = editorToolContext.ContainerView;
                        EditorToolContext.SaveAsjson(containerView.Title + ".json", containerView);
                        return;
                    }

                    if (command == "--new-editor")
                    {
                        var editorToolContext = EditorToolContext.Create();
                        EditorToolContext.SaveAsjson("editor.json", editorToolContext);
                        return;
                    }
                }
                else if (args.Length == 2)
                {
                    var inputPath = args[0];
                    var outputPath = args[1];
                    var inputExtension = Path.GetExtension(inputPath);
                    var outputExtension = Path.GetExtension(outputPath);

                    if (string.Compare(inputExtension, ".json", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var containerView = EditorToolContext.LoadFromJson<ContainerView>(inputPath);

                        if (string.Compare(outputExtension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                             EditorToolContext.ExportSvg(outputPath, containerView);
                        }
                        else if (string.Compare(outputExtension, ".png", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                             EditorToolContext.ExportPng(outputPath, containerView);
                        }
                        else if (string.Compare(outputExtension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                             EditorToolContext.ExportPdf(outputPath, containerView);
                        }
                    }

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
            EditorToolContext editorToolContext = null;
            WindowSettings mainWindowSettings = null;

            if (File.Exists("editor.json"))
            {
                editorToolContext = EditorToolContext.LoadFromJson<EditorToolContext>("editor.json");

                foreach (var containerView in editorToolContext.ContainerViews)
                {
                    editorToolContext.InitContainerView(containerView);
                }
            }
            else
            {
                editorToolContext = EditorToolContext.Create();
                editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
            }

            if (File.Exists("window.json"))
            {
                mainWindowSettings = EditorToolContext.LoadFromJson<WindowSettings>("window.json");
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
                DataContext = editorToolContext
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

            EditorToolContext.SaveAsjson<EditorToolContext>("editor.json", editorToolContext);
            EditorToolContext.SaveAsjson<WindowSettings>("window.json", mainWindowSettings);
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
