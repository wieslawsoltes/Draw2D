using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.ViewModels.Containers;
using Draw2D.Wpf.Renderers;
using PathDemo.Tools;

namespace PathDemo.Controls
{
    public class PathCanvas : Canvas
    {
        public ShapesContainerViewModel ViewModel { get; set; }

        public ShapeRenderer Renderer { get; set; }

        public PathCanvas()
        {
            ViewModel = new ShapesContainerViewModel()
            {
                Selected = new HashSet<ShapeObject>(),
                CurrentContainer = new ShapesContainer(),
                Capture = () => this.CaptureMouse(),
                Release = () => this.ReleaseMouseCapture(),
                Invalidate = () => this.InvalidateVisual(),
                Tools = new ObservableCollection<ToolBase>
                {
                    new LineTool(),
                    new CubicBezierTool(),
                    new QuadraticBezierTool(),
                    new PathTool()
                },
                Renderer = new WpfShapeRenderer()
            };

            ViewModel.CurrentTool = ViewModel.Tools[0];

            Renderer = new ShapeRenderer();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            var point = e.GetPosition(this);
            ViewModel.CurrentTool.LeftDown(ViewModel, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            var point = e.GetPosition(this);
            ViewModel.CurrentTool.LeftUp(ViewModel, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            var point = e.GetPosition(this);
            ViewModel.CurrentTool.RightDown(ViewModel, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);
            var point = e.GetPosition(this);
            ViewModel.CurrentTool.RightUp(ViewModel, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            var point = e.GetPosition(this);
            ViewModel.CurrentTool.Move(ViewModel, point.X, point.Y, Modifier.None);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Renderer.Draw(dc, ViewModel);
        }
    }
}
