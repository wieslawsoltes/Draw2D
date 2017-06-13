// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Draw2D.Core.Containers;
using Draw2D.Editor.Tools;
using Draw2D.Json;
using Draw2D.ViewModels.Containers;
using Microsoft.Win32;

namespace Draw2D.Wpf.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            KeyDown += MainView_KeyDown;
        }

        public void SetNoneTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "None").FirstOrDefault();
            }
        }

        public void SetSelectionTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "Selection").FirstOrDefault();
            }
        }

        public void SetLineTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "Line").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Title == "Line").FirstOrDefault();
                }
            }
        }

        public void SetPointTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "Point").FirstOrDefault();
            }
        }

        public void SetCubicBezierTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "CubicBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Title == "CubicBezier").FirstOrDefault();
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
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "QuadraticBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Title == "QuadraticBezier").FirstOrDefault();
                }
            }
        }

        public void SetPathTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "Path").FirstOrDefault();
            }
        }

        public void SetMoveTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanCurrentTool(vm);
                    pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "Move").FirstOrDefault();
                }
            }
        }

        public void SetRectangleTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "Rectangle").FirstOrDefault();
            }
        }

        public void SetEllipseTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "Ellipse").FirstOrDefault();
            }
        }

        public void SetTextTool()
        {
            if (this.DataContext is ShapeContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Title == "Text").FirstOrDefault();
            }
        }

        private void MainView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
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
            else if (Keyboard.Modifiers == ModifierKeys.None)
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
                    case Key.T:
                        SetTextTool();
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
            App.Current.Windows[0].Close();
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

        private void Open()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 0
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                if (this.DataContext is ShapeContainerViewModel vm)
                {
                    Open(path, vm);
                    rendererView.InvalidateVisual();
                }
            }
        }

        private void SaveAs()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "container"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
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
