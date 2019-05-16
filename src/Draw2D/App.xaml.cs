// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Linq;
using Avalonia;
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
            BuildAvaloniaApp().Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            EditContainerView editContainerView = null;

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

            var window = new MainWindow
            {
                DataContext = editContainerView
            };

            app.Run(window);

            EditContainerView.SaveAsjson<EditContainerView>("editor.json", editContainerView);
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
