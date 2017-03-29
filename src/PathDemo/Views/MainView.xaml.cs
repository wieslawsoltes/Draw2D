using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            if (canvas.CurrentTool is PathTool pathTool)
            {
                pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Line").FirstOrDefault();
            }
            else
            {
                canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "Line").FirstOrDefault();
            }
        }

        public void SetCubicBezierTool()
        {
            if (canvas.CurrentTool is PathTool pathTool)
            {
                pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
            }
            else
            {
                canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
            }
        }

        public void SetQuadraticBezierTool()
        {
            if (canvas.CurrentTool is PathTool pathTool)
            {
                pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
            }
            else
            {
                canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
            }
        }

        public void SetPathTool()
        {
            canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "Path").FirstOrDefault();
        }

        public void SetMoveTool()
        {
            if (canvas.CurrentTool is PathTool pathTool)
            {
                pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Move").FirstOrDefault();
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
