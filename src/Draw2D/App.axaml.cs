using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Draw2D.Editor;
using Draw2D.Export;
using Draw2D.Serializer;
using Draw2D.Settings;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using Draw2D.Views;

namespace Draw2D
{
    public static class EditorToolContextBuilder
    {
        public static void UseSkia(this IEditorToolContext editorToolContext)
        {
            editorToolContext.PathConverter = new SkiaPathConverter();
            editorToolContext.AvaloniaXamlConverter = new AvaloniaXamlConverter();
            editorToolContext.ContainerImporter = new SkiaContainerImporter();
            editorToolContext.ContainerExporter = new SkiaContainerExporter();
            editorToolContext.SvgConverter = new SkiaSvgConverter();
        }
    }

    public class App : Application
    {
        public static string DocumentPath { get; set; }
        public static string EditorPath { get; set; }
        public static string WindowPath { get; set; }
        public static IContainerFactory ContainerFactory { get; set; }
        public static IStyleLibrary StyleLibrary { get; set; }
        public static IGroupLibrary GroupLibrary { get; set; }
        public static IToolContext ToolContext { get; set; }
        public static WindowSettings WindowSettings { get; set; }

        static App()
        {
            DocumentPath = "document.json";
            EditorPath = "editor.json";
            WindowPath = "window.json";

            ContainerFactory = new DefaultContainerFactory();

            if (Design.IsDesignMode)
            {
                StyleLibrary = ContainerFactory.CreateStyleLibrary();
                GroupLibrary = ContainerFactory.CreateGroupLibrary();
                ToolContext = ContainerFactory.CreateToolContext();
                ToolContext.DocumentContainer.StyleLibrary = StyleLibrary;
                ToolContext.DocumentContainer.GroupLibrary = GroupLibrary;

                if (ToolContext is IEditorToolContext editorToolContext)
                {
                    editorToolContext.ContainerFactory = ContainerFactory;
                    editorToolContext.UseSkia();
                    editorToolContext.NewContainerView("View");

                    editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                    editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
                }
            }
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

        public static void Load()
        {
            if (File.Exists(EditorPath))
            {
                ToolContext = JsonSerializer.FromJsonFile<IToolContext>(EditorPath);
            }

            if (ToolContext == null)
            {
                ToolContext = ContainerFactory.CreateToolContext();
            }

            bool isNewDocument;

            if (File.Exists(DocumentPath))
            {
                ToolContext.DocumentContainer = JsonSerializer.FromJsonFile<IDocumentContainer>(DocumentPath);
                isNewDocument = false;
            }
            else
            {
                ToolContext.DocumentContainer = ContainerFactory.CreateDocumentContainer("document");
                ToolContext.DocumentContainer.StyleLibrary = ContainerFactory.CreateStyleLibrary();
                ToolContext.DocumentContainer.GroupLibrary = ContainerFactory.CreateGroupLibrary();
                isNewDocument = true;
            }

            if (ToolContext is IEditorToolContext editorToolContext)
            {
                editorToolContext.ContainerFactory = ContainerFactory;
                editorToolContext.UseSkia();

                if (editorToolContext.CurrentDirectory == null)
                {
                    editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                    editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
                }

                if (isNewDocument)
                {
                    editorToolContext.NewContainerView("View");
                }
                else
                {
                    foreach (var containerView in editorToolContext.DocumentContainer.ContainerViews)
                    {
                        editorToolContext.InitContainerView(containerView);
                    }
                }
            }

            if (File.Exists(WindowPath))
            {
                WindowSettings = JsonSerializer.FromJsonFile<WindowSettings>(WindowPath);
            }

            if (WindowSettings == null)
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
        }

        public static void Save()
        {
            JsonSerializer.ToJsonFile(DocumentPath, ToolContext.DocumentContainer);
            JsonSerializer.ToJsonFile(EditorPath, ToolContext);
            JsonSerializer.ToJsonFile(WindowPath, WindowSettings);
            WindowSettings = null;
            ToolContext.Dispose();
            ToolContext = null;
            StyleLibrary = null;
            GroupLibrary = null;
            ContainerFactory = null;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                App.Load();

                var window = new MainWindow
                {
                    DataContext = App.ToolContext
                };

                App.SetWindowSettings(window);

                window.Closing += (sender, e) =>
                {
                    App.GetWindowSettings(window);
                };

                desktopLifetime.MainWindow = window;

                desktopLifetime.Exit += (sennder, e) =>
                {
                    App.Save();
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                App.Load();

                var view = new MainView
                {
                    DataContext = App.ToolContext
                };

                view.Initialized += (sennder, e) =>
                {
                };

                singleViewLifetime.MainView = view;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
