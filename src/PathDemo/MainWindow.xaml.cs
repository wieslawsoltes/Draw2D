using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PathDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.L:
                    {
                        if (canvas.CurrentTool.Name == "Path")
                        {
                            var pathTool = canvas.CurrentTool as PathTool;
                            pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Line").FirstOrDefault();
                        }
                        else
                        {
                            canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "Line").FirstOrDefault();
                        }
                    }
                    break;
                case Key.C:
                    {
                        if (canvas.CurrentTool.Name == "Path")
                        {
                            var pathTool = canvas.CurrentTool as PathTool;
                            pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                        }
                        else
                        {
                            canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                        }
                    }
                    break;
                case Key.Q:
                    {
                        if (canvas.CurrentTool.Name == "Path")
                        {
                            var pathTool = canvas.CurrentTool as PathTool;
                            pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                        }
                        else
                        {
                            canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                        }
                    }
                    break;
                case Key.H:
                    {
                        canvas.CurrentTool = canvas.Tools.Where(t => t.Name == "Path").FirstOrDefault();
                    }
                    break;
                case Key.M:
                    {
                        if (canvas.CurrentTool.Name == "Path")
                        {
                            var pathTool = canvas.CurrentTool as PathTool;
                            pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Move").FirstOrDefault();
                        }
                    }
                    break;
            }
        }
    }
}
