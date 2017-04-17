// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Draw2D.Avalonia.Controls;
using Draw2D.Avalonia.Renderers;
using Draw2D.Avalonia.Views;
using Draw2D.ViewModels;
using Serilog;

namespace Draw2D.Avalonia
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
            var mainView = window.FindControl<MainView>("mainView");
            var rendererView = mainView.FindControl<ShapeContainerRenderView>("rendererView");

            var bootstrapper = new Bootstrapper();
            var vm = bootstrapper.CreateDemoViewModel();
            bootstrapper.CreateDemoContainer(vm);

            vm.Renderer = new AvaloniaShapeRenderer();
            vm.Selected = vm.Renderer.Selected;
            vm.Capture = () => MouseDevice.Instance.Capture(rendererView);
            vm.Release = () => MouseDevice.Instance.Capture(null);
            vm.Invalidate = () => rendererView.InvalidateVisual();

            window.DataContext = vm;
            window.ShowDialog();
            Run(window);
        }
    }
}
