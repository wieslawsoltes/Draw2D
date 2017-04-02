using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Draw2D.Editor;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;
using Draw2D.ViewModels.Containers;
using Draw2D.Wpf.Renderers;
using Draw2D.PathDemo.Views;
using System.Linq;
using Draw2D.Core.Presenters;
using System;
using Draw2D.Core.Renderers;
using Draw2D.Core.Renderers.Helpers;
using Draw2D.Editor.Tools;

namespace Draw2D.PathDemo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var vm = CreateViewModel();
            var mainView = new MainView();
            var rendererView = mainView.RendererView;

            vm.Capture = () => rendererView.CaptureMouse();
            vm.Release = () => rendererView.ReleaseMouseCapture();
            vm.Invalidate = () => rendererView.InvalidateVisual();

            mainView.DataContext = vm;
            mainView.ShowDialog();
        }

        private ShapesContainerViewModel CreateViewModel()
        {
            var vm = new ShapesContainerViewModel()
            {
                Selected = new HashSet<ShapeObject>(),
                CurrentContainer = new ShapesContainer()
                {
                    Width = 720,
                    Height = 630
                },
                WorkingContainer = new ShapesContainer()
                {
                    Width = 720,
                    Height = 630
                },
                Tools = new ObservableCollection<ToolBase>
                {
                    new NoneTool(),
                    new SelectionTool(),
                    new GuideTool(),
                    new PointTool(),
                    new LineTool(),
                    new PolyLineTool(),
                    new CubicBezierTool(),
                    new QuadraticBezierTool(),
                    new PathTool(),
                    new ScribbleTool(),
                    new RectangleTool(),
                    new EllipseTool()
                },
                Renderer = new WpfShapeRenderer(),
                CurrentStyle = new DrawStyle(new DrawColor(255, 0, 255, 0), new DrawColor(80, 0, 255, 0), 2.0, true, true),
                PointShape = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
                {
                    Style = new DrawStyle(new DrawColor(0, 0, 0, 0), new DrawColor(255, 255, 255, 0), 2.0, false, true)
                },
                Presenter = new DefaultShapePresenter()
                {
                    Helpers = new Dictionary<Type, ShapeHelper>
                    {
                        { typeof(LineShape), new LineHelper() },
                        { typeof(CubicBezierShape), new CubicBezierHelper() },
                        { typeof(QuadraticBezierShape), new QuadraticBezierHelper() },
                        { typeof(PathShape), new PathHelper() },
                        { typeof(RectangleShape), new RectangleHelper() },
                        { typeof(EllipseShape), new EllipseHelper() }
                    }
                }
            };

            vm.CurrentTool = vm.Tools.Where(t => t.Name == "Path").FirstOrDefault();

            return vm;
        }
    }
}
