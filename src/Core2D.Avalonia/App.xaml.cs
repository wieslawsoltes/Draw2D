// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Controls;
using Core2D.Avalonia.Renderers;
using Core2D.Avalonia.Views;
using Core2D.ViewModels;
using Serilog;

namespace Core2D.Avalonia
{
    public class App : Application
    {
        static void Main(string[] args)
        {
            InitializeLogging();

            var app = new App();
            AppBuilder.Configure(app)
                .UsePlatformDetect()
                .SetupWithoutStarting();
            app.Start();
        }

        static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Start()
        {
            var window = new MainWindow();
            var inputRoot = window as IInputRoot;
            var mainView = window.FindControl<MainView>("mainView");
            var rendererView = mainView.FindControl<LayerContainerRenderView>("rendererView");

            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();
            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new AvaloniaShapeRenderer();
            vm.Selected = vm.Renderer.Selected;
            vm.Capture = () =>
            {
                if (inputRoot.MouseDevice?.Captured == null)
                {
                    inputRoot.MouseDevice?.Capture(rendererView);
                }
            };
            vm.Release = () =>
            {
                if (inputRoot.MouseDevice?.Captured != null)
                {
                    inputRoot.MouseDevice?.Capture(null);
                }
            };
            vm.Invalidate = () => rendererView.InvalidateVisual();

            window.DataContext = vm;
            window.ShowDialog();
            Run(window);
        }
    }
}
