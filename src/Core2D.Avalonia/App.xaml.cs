// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Controls;
using Core2D.Avalonia.Renderers;
using Core2D.Avalonia.Views;
using Core2D.ViewModels;

namespace Core2D.Avalonia
{
    public class App : Application
    {
        static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToDebug();

        static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp().Start<MainWindow>();
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
