using System.Linq;
using System.Windows;
using System.Windows.Input;
using Draw2D.ViewModels.Containers;
using PathDemo.Tools;

namespace PathDemo.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            KeyDown += MainView_KeyDown;
        }

        public void SetLineTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
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
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Move").FirstOrDefault();
                }
            }
        }

        private void MainView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
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
            }
        }
    }
}
