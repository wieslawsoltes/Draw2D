using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;
using Draw2D.ViewModels.Containers;
using Draw2D.Wpf.Renderers;
using PathDemo.Tools;
using PathDemo.Views;

namespace PathDemo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var vm = new ShapesContainerViewModel()
            {
                Selected = new HashSet<ShapeObject>(),
                CurrentContainer = new ShapesContainer(),
                WorkingContainer = new ShapesContainer(),
                Tools = new ObservableCollection<ToolBase>
                {
                    new LineTool(),
                    new CubicBezierTool(),
                    new QuadraticBezierTool(),
                    new PathTool()
                },
                Renderer = new WpfShapeRenderer(),
                CurrentStyle = new DrawStyle(new DrawColor(255, 0, 255, 0), new DrawColor(80, 0, 255, 0), 2.0, true, true),
                PointShape = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
                {
                    Style = new DrawStyle(new DrawColor(0, 0, 0, 0), new DrawColor(255, 255, 255, 0), 2.0, false, true)
                }
            };

            vm.CurrentTool = vm.Tools[0];

            var mainView = new MainView();
            var rendererView = mainView.RendererView;

            vm.Capture = () => rendererView.CaptureMouse();
            vm.Release = () => rendererView.ReleaseMouseCapture();
            vm.Invalidate = () => rendererView.InvalidateVisual();

            mainView.DataContext = vm;
            mainView.ShowDialog();
        }
    }
}
