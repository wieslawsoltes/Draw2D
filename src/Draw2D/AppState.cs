// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Draw2D.Editor;
using Draw2D.ViewModels.Containers;
using Draw2D.Views;

namespace Draw2D
{
    public static class AppState
    {
        public static string StylesPath { get; set; }
        public static string GroupsPath { get; set; }
        public static string EditorPath { get; set; }
        public static string WindowPath { get; set; }
        public static IFactory Factory { get; set; }
        public static IStyleLibrary StyleLibrary { get; set; }
        public static IGroupLibrary GroupLibrary { get; set; }
        public static IToolContext ToolContext { get; set; }
        public static WindowSettings WindowSettings { get; set; }

        static AppState()
        {
            StylesPath = "styles.json";
            GroupsPath = "groups.json";
            EditorPath = "editor.json";
            WindowPath = "window.json";

            Factory = new EditorFactory();

            if (Design.IsDesignMode)
            {
                StyleLibrary = Factory.CreateStyleLibrary();
                GroupLibrary = Factory.CreateGroupLibrary();

                ToolContext = Factory.CreateToolContext();
                ToolContext.StyleLibrary = StyleLibrary;
                ToolContext.GroupLibrary = GroupLibrary;

                if (ToolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.Factory = Factory;
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
            if (File.Exists(StylesPath))
            {
                StyleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(StylesPath);
            }
            else
            {
                StyleLibrary = Factory.CreateStyleLibrary();
            }

            if (File.Exists(GroupsPath))
            {
                GroupLibrary = JsonSerializer.FromJsonFile<IGroupLibrary>(GroupsPath);
            }
            else
            {
                GroupLibrary = Factory.CreateGroupLibrary();
            }

            if (File.Exists(EditorPath))
            {
                ToolContext = JsonSerializer.FromJsonFile<IToolContext>(EditorPath);
                ToolContext.StyleLibrary = StyleLibrary;
                ToolContext.GroupLibrary = GroupLibrary;

                if (ToolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.Factory = Factory;
                    foreach (var containerView in editorToolContext.ContainerViews)
                    {
                        editorToolContext.InitContainerView(containerView);
                    }
                }
            }
            else
            {
                ToolContext = Factory.CreateToolContext();
                ToolContext.StyleLibrary = StyleLibrary;
                ToolContext.GroupLibrary = GroupLibrary;

                if (ToolContext is EditorToolContext editorToolContext)
                {
                    editorToolContext.Factory = Factory;
                    editorToolContext.NewContainerView("View");

                    editorToolContext.CurrentDirectory = Directory.GetCurrentDirectory();
                    editorToolContext.AddFiles(editorToolContext.CurrentDirectory);
                }
            }

            if (File.Exists(WindowPath))
            {
                WindowSettings = JsonSerializer.FromJsonFile<WindowSettings>(WindowPath);
            }
            else
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
            JsonSerializer.ToJsonFile(StylesPath, ToolContext.StyleLibrary);
            JsonSerializer.ToJsonFile(GroupsPath, ToolContext.GroupLibrary);
            JsonSerializer.ToJsonFile(EditorPath, ToolContext);
            JsonSerializer.ToJsonFile(WindowPath, WindowSettings);

            WindowSettings = null;
            ToolContext.Dispose();
            ToolContext = null;
            StyleLibrary = null;
            GroupLibrary = null;
            Factory = null;
        }

        public static bool ParseArgs(string[] args)
        {
            if (args.Length == 1)
            {
                var command = args[0];

                if (command == "--new-styles")
                {
                    var styleLibrary = Factory.CreateStyleLibrary();
                    JsonSerializer.ToJsonFile("styles.json", styleLibrary);
                    return false;
                }

                if (command == "--new-groups")
                {
                    var groupLibrary = Factory.CreateGroupLibrary();
                    JsonSerializer.ToJsonFile("groups.json", groupLibrary);
                    return false;
                }

                if (command == "--new-view")
                {
                    var containerView = Factory.CreateContainerView("View");
                    JsonSerializer.ToJsonFile("View.json", containerView);
                    return false;
                }

                if (command == "--new-editor")
                {
                    var toolContext = Factory.CreateToolContext();
                    JsonSerializer.ToJsonFile("editor.json", toolContext);
                    return false;
                }

                if (command == "--demo")
                {
                    var toolContext = Factory.CreateToolContext();
                    var styleLibrary = Factory.CreateStyleLibrary();
                    var groupLibrary = Factory.CreateGroupLibrary();
                    toolContext.StyleLibrary = styleLibrary;
                    toolContext.GroupLibrary = groupLibrary;
                    if (toolContext is EditorToolContext editorToolContext)
                    {
                        editorToolContext.NewContainerView("Demo");
                        editorToolContext.CreateDemoGroup(editorToolContext);
                        JsonSerializer.ToJsonFile("Demo.json", editorToolContext.ContainerView);
                    }
                    return false;
                }
            }
            else if (args.Length == 5)
            {
                var command = args[0];

                if (command == "--export")
                {
                    var toolContext = Factory.CreateToolContext();
                    var styleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(args[1]);
                    toolContext.StyleLibrary = styleLibrary;
                    var groupLibrary = JsonSerializer.FromJsonFile<IGroupLibrary>(args[2]);
                    toolContext.GroupLibrary = groupLibrary;
                    var containerView = JsonSerializer.FromJsonFile<ContainerView>(args[3]);
                    EditorToolContext.Export(args[4], containerView, toolContext);
                    return false;
                }
            }

            return true;
        }
    }
}
