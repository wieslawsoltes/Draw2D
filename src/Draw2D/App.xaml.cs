// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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
            var window = new MainWindow
            {
                DataContext = new CanvasEditor(),
            };
            app.Run(window);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .UseSkia()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
