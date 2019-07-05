// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Draw2D.Editor;
using Draw2D.Export;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Draw2D.Views;

namespace Draw2D
{
    public class App : Application
    {
        public static string StylesPath { get; set; }
        public static string GroupsPath { get; set; }
        public static string EditorPath { get; set; }
        public static string WindowPath { get; set; }
        public static IContainerFactory ContainerFactory { get; set; }
        public static IStyleLibrary StyleLibrary { get; set; }
        public static IGroupLibrary GroupLibrary { get; set; }
        public static IToolContext ToolContext { get; set; }
        public static WindowSettings WindowSettings { get; set; }

        static App()
        {
            StylesPath = "styles.json";
            GroupsPath = "groups.json";
            EditorPath = "editor.json";
            WindowPath = "window.json";

            ContainerFactory = new EditorContainerFactory();

            if (Design.IsDesignMode)
            {
                StyleLibrary = ContainerFactory.CreateStyleLibrary();
                GroupLibrary = ContainerFactory.CreateGroupLibrary();
                ToolContext = ContainerFactory.CreateToolContext();
                ToolContext.StyleLibrary = StyleLibrary;
                ToolContext.GroupLibrary = GroupLibrary;

                if (ToolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.ContainerFactory = ContainerFactory;
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
#if USE_LOAD_STYLES
            if (File.Exists(StylesPath))
            {
                StyleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(StylesPath);
            }
#endif
#if USE_LOAD_GROUPS
            if (File.Exists(GroupsPath))
            {
                GroupLibrary = JsonSerializer.FromJsonFile<IGroupLibrary>(GroupsPath);
            }
#endif

#if USE_LOAD_EDITOR
            if (File.Exists(EditorPath))
            {
                ToolContext = JsonSerializer.FromJsonFile<IToolContext>(EditorPath);
#if USE_LOAD_STYLES
                if (StyleLibrary != null)
                {
                    ToolContext.StyleLibrary = StyleLibrary;
                }
#endif
#if USE_LOAD_GROUPS
                if (GroupLibrary != null)
                {
                    ToolContext.GroupLibrary = GroupLibrary;
                }
#endif
                if (ToolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.ContainerFactory = ContainerFactory;
                    foreach (var containerView in editorToolContext.ContainerViews)
                    {
                        editorToolContext.InitContainerView(containerView);
                    }
                }
            }
#endif
            if (ToolContext == null)
            {
                ToolContext = ContainerFactory.CreateToolContext();
#if USE_LOAD_STYLES
                if (StyleLibrary != null)
                {
                    ToolContext.StyleLibrary = StyleLibrary;
                }
#endif
#if USE_LOAD_GROUPS
                if (GroupLibrary != null)
                {
                    ToolContext.GroupLibrary = GroupLibrary;
                }
#endif
                if (ToolContext.StyleLibrary == null)
                {
                    ToolContext.StyleLibrary = ContainerFactory.CreateStyleLibrary();
                }

                if (ToolContext.GroupLibrary == null)
                {
                    ToolContext.GroupLibrary = ContainerFactory.CreateGroupLibrary();
                }

                if (ToolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.ContainerFactory = ContainerFactory;
                    editorToolContext.NewContainerView("View");

                    editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                    editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
                }
            }

#if USE_LOAD_WINDOW
            if (File.Exists(WindowPath))
            {
                WindowSettings = JsonSerializer.FromJsonFile<WindowSettings>(WindowPath);
            }
#endif
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
#if USE_SAVE_STYLES
            JsonSerializer.ToJsonFile(StylesPath, ToolContext.StyleLibrary);
#endif
#if USE_SAVE_GROUPS
            JsonSerializer.ToJsonFile(GroupsPath, ToolContext.GroupLibrary);
#endif
#if USE_SAVE_EDITOR
            JsonSerializer.ToJsonFile(EditorPath, ToolContext);
#endif
#if USE_SAVE_WINDOW
            JsonSerializer.ToJsonFile(WindowPath, WindowSettings);
#endif
            WindowSettings = null;
            ToolContext.Dispose();
            ToolContext = null;
            StyleLibrary = null;
            GroupLibrary = null;
            ContainerFactory = null;
        }

        public static bool ParseArgs(string[] args)
        {
            if (args.Length == 1)
            {
                var command = args[0];

                if (command == "--new-styles")
                {
                    var styleLibrary = ContainerFactory.CreateStyleLibrary();
                    JsonSerializer.ToJsonFile("styles.json", styleLibrary);
                    return false;
                }

                if (command == "--new-groups")
                {
                    var groupLibrary = ContainerFactory.CreateGroupLibrary();
                    JsonSerializer.ToJsonFile("groups.json", groupLibrary);
                    return false;
                }

                if (command == "--new-view")
                {
                    var containerView = ContainerFactory.CreateContainerView("View");
                    JsonSerializer.ToJsonFile("View.json", containerView);
                    return false;
                }

                if (command == "--new-editor")
                {
                    var toolContext = ContainerFactory.CreateToolContext();
                    JsonSerializer.ToJsonFile("editor.json", toolContext);
                    return false;
                }
            }
            else if (args.Length == 3)
            {
                var command = args[0];

                if (command == "--export")
                {
                    var toolContext = JsonSerializer.FromJsonFile<IToolContext>(args[1]);
                    SkiaCanvasConverter.Export(toolContext, args[2], toolContext.ContainerView);
                    return false;
                }
            }

            return true;
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
                var view = new MainView
                {
                    DataContext = App.ToolContext
                };

                view.Initialized += (sennder, e) =>
                {
                    App.Load();
                };

                singleViewLifetime.MainView = view;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
