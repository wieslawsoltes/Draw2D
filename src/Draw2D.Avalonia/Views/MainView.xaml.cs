// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Draw2D.Avalonia.Controls;
using Draw2D.Core.Containers;
using Draw2D.Editor.Tools;
using Draw2D.Json;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Avalonia.Views
{
    public class MainView : UserControl
    {
        private ShapeContainerInputView inputView;
        private ShapeContainerRenderView rendererView;

        public MainView()
        {
            InitializeComponent();
            KeyDown += MainView_KeyDown;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            inputView = this.FindControl<ShapeContainerInputView>("inputView");
            rendererView = this.FindControl<ShapeContainerRenderView>("rendererView");

            this.FindControl<MenuItem>("FileNew").Click += FileNew_Click;
            this.FindControl<MenuItem>("FileOpen").Click += FileOpen_Click;
            this.FindControl<MenuItem>("FileSaveAs").Click += FileSaveAs_Click;
            this.FindControl<MenuItem>("FileExit").Click += FileExit_Click;
            this.FindControl<MenuItem>("EditCut").Click += EditCut_Click;
            this.FindControl<MenuItem>("EditCopy").Click += EditCopy_Click;
            this.FindControl<MenuItem>("EditPaste").Click += EditPaste_Click;
            this.FindControl<MenuItem>("EditDelete").Click += EditDelete_Click;
            this.FindControl<MenuItem>("EditGroup").Click += EditGroup_Click;
            this.FindControl<MenuItem>("EditSelectAll").Click += EditSelectAll_Click;
        }

        public void SetNoneTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "None").FirstOrDefault();
            }
        }

        public void SetSelectionTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Selection").FirstOrDefault();
            }
        }

        public void SetLineTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Name == "Line").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "Line").FirstOrDefault();
                }
            }
        }

        public void SetPointTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Point").FirstOrDefault();
            }
        }

        public void SetCubicBezierTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                }
            }
        }

        public void SetQuadraticBezierTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                }
            }
        }

        public void SetPathTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Path").FirstOrDefault();
            }
        }

        public void SetMoveTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Name == "Move").FirstOrDefault();
                }
            }
        }

        public void SetRectangleTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Rectangle").FirstOrDefault();
            }
        }

        public void SetEllipseTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Ellipse").FirstOrDefault();
            }
        }

        private void MainView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == InputModifiers.Control)
            {
                Debug.WriteLine($"Shortcut: Ctrl+{e.Key}");
                switch (e.Key)
                {
                    case Key.N:
                        New();
                        break;
                    case Key.O:
                        Open();
                        break;
                    case Key.S:
                        SaveAs();
                        break;
                    case Key.X:
                        Cut();
                        break;
                    case Key.C:
                        Copy();
                        break;
                    case Key.V:
                        Paste();
                        break;
                    case Key.G:
                        Group();
                        break;
                    case Key.A:
                        SelectAll();
                        break;
                }
            }
            else if (e.Modifiers == InputModifiers.None)
            {
                Debug.WriteLine($"Shortcut: {e.Key}");
                switch (e.Key)
                {
                    case Key.N:
                        SetNoneTool();
                        break;
                    case Key.S:
                        SetSelectionTool();
                        break;
                    case Key.L:
                        SetLineTool();
                        break;
                    case Key.P:
                        SetPointTool();
                        break;
                    case Key.C:
                        SetCubicBezierTool();
                        break;
                    case Key.Q:
                        SetQuadraticBezierTool();
                        break;
                    case Key.H:
                        SetPathTool();
                        break;
                    case Key.M:
                        SetMoveTool();
                        break;
                    case Key.R:
                        SetRectangleTool();
                        break;
                    case Key.E:
                        SetEllipseTool();
                        break;
                    case Key.Delete:
                        Delete();
                        break;
                }
            }
        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            New();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Window.OpenWindows.FirstOrDefault()?.Close();
        }

        private void EditCut_Click(object sender, RoutedEventArgs e)
        {
            Cut();
        }

        private void EditCopy_Click(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        private void EditPaste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void EditDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void EditGroup_Click(object sender, RoutedEventArgs e)
        {
            Group();
        }

        private void EditSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        private void New()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                New(vm);
                rendererView.InvalidateVisual();
            }
        }

        private async void Open()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            var result = await dlg.ShowAsync();
            if (result != null)
            {
                var path = result.FirstOrDefault();
                if (this.DataContext is ShapeContainerViewModel vm)
                {
                    Open(path, vm);
                    rendererView.InvalidateVisual();
                }
            }
        }

        private async void SaveAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = "container";
            dlg.DefaultExtension = "project";
            var result = await dlg.ShowAsync();
            if (result != null)
            {
                var path = result;
                if (this.DataContext is ShapeContainerViewModel vm)
                {
                    Save(path, vm);
                }
            }
        }

        private void Cut()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is SelectionTool selectionTool)
                {
                    selectionTool.Cut(vm);
                }
            }
        }

        private void Copy()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is SelectionTool selectionTool)
                {
                    selectionTool.Copy(vm);
                }
            }
        }

        private void Paste()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is SelectionTool selectionTool)
                {
                    selectionTool.Paste(vm);
                }
            }
        }

        private void Delete()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is SelectionTool selectionTool)
                {
                    selectionTool.Delete(vm);
                }
            }
        }

        private void Group()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is SelectionTool selectionTool)
                {
                    selectionTool.Group(vm);
                }
            }
        }

        private void SelectAll()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is SelectionTool selectionTool)
                {
                    selectionTool.SelectAll(vm);
                }
            }
        }

        private void New(ShapeContainerViewModel vm)
        {
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            var container = new ShapeContainer()
            {
                Width = 720,
                Height = 630
            };
            var workingContainer = new ShapeContainer();
            vm.CurrentContainer = container;
            vm.WorkingContainer = new ShapeContainer();
        }

        private void Open(string path, ShapeContainerViewModel vm)
        {
            var json = File.ReadAllText(path);
            var container = NewtonsoftJsonSerializer.FromJson<ShapeContainer>(json);
            var workingContainer = new ShapeContainer();
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            vm.CurrentContainer = container;
            vm.WorkingContainer = workingContainer;
        }

        private void Save(string path, ShapeContainerViewModel vm)
        {
            var json = NewtonsoftJsonSerializer.ToJson(vm.CurrentContainer);
            File.WriteAllText(path, json);
        }
    }
}
