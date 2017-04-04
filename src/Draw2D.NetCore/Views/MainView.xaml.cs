// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Draw2D.Core.Containers;
using Draw2D.Core.Editor.Tools;
using Draw2D.Core.ViewModels.Containers;
using Draw2D.NetCore.Controls;

namespace Draw2D.NetCore.Views
{
    public class MainView : UserControl
    {
        private ShapesContainerInputView inputView;
        private ShapesContainerRenderView rendererView;

        public MainView()
        {
            this.InitializeComponent();
            this.KeyDown += MainView_KeyDown;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            inputView = this.FindControl<ShapesContainerInputView>("inputView");
            rendererView = this.FindControl<ShapesContainerRenderView>("rendererView");
        }

        public void SetSelectionTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Selection").FirstOrDefault();
            }
        }

        public void SetLineTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Line").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "Line").FirstOrDefault();
                }
            }
        }

        public void SetCubicBezierTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                }
            }
        }

        public void SetQuadraticBezierTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                }
            }
        }

        public void SetPathTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Path").FirstOrDefault();
            }
        }

        public void SetMoveTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Move").FirstOrDefault();
                }
            }
        }

        private void MainView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == InputModifiers.Control)
            {
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
                }
            }
            else if (e.Modifiers == InputModifiers.None)
            {
                switch (e.Key)
                {
                    case Key.S:
                        SetSelectionTool();
                        break;
                    case Key.L:
                        SetLineTool();
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
                    case Key.Delete:
                        Delete();
                        break;
                }
            }
        }

        private void New()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                New(vm);
                rendererView.InvalidateVisual();
            }
        }

        private void Open()
        {
            // TODO:
        }

        private void SaveAs()
        {
            // TODO:
        }

        private void Cut()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Cut(vm);
                rendererView.InvalidateVisual();
            }
        }

        private void Copy()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Copy(vm);
            }
        }

        private void Paste()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Paste(vm);
                rendererView.InvalidateVisual();
            }
        }

        private void Delete()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Delete(vm);
                rendererView.InvalidateVisual();
            }
        }

        private void New(ShapesContainerViewModel vm)
        {
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            var container = new ShapesContainer()
            {
                Width = 720,
                Height = 630
            };
            var workingContainer = new ShapesContainer();
            vm.CurrentContainer = container;
            vm.WorkingContainer = new ShapesContainer();
        }

        private void Open(string path, ShapesContainerViewModel vm)
        {
            // TODO:
        }

        private void Save(string path, ShapesContainerViewModel vm)
        {
            // TODO:
        }

        private void Cut(ShapesContainerViewModel vm)
        {
            // TODO:
        }

        private void Copy(ShapesContainerViewModel vm)
        {
            // TODO:
        }

        private void Paste(ShapesContainerViewModel vm)
        {
            // TODO:
        }

        private void Delete(ShapesContainerViewModel vm)
        {
            foreach (var shape in vm.Renderer.Selected)
            {
                if (vm.CurrentContainer.Shapes.Contains(shape))
                {
                    vm.CurrentContainer.Shapes.Remove(shape);
                }
            }
            vm.Renderer.Selected.Clear();
        }
    }
}
