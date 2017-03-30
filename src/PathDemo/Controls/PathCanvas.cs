using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PathDemo.Controls
{
    public class PathCanvas : Canvas
    {
        private LineHelper _lineHelper;
        private CubiceBezierHelper _cubiceBezierHelper;
        private QuadraticBezierHelper _quadraticBezierHelper;
        private PathHelper _pathHelper;

        public ShapesContainerViewModel ViewModel { get; set; }

        public PathCanvas()
        {
            _lineHelper = new LineHelper();
            _cubiceBezierHelper = new CubiceBezierHelper();
            _quadraticBezierHelper = new QuadraticBezierHelper();
            _pathHelper = new PathHelper();
            
            ViewModel = new ShapesContainerViewModel()
            {
                Selected = new HashSet<ShapeObject>(),
                CurrentContainer = new ShapesContainer(),
                WorkingContainer = new ShapesContainer(),
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
                Renderer = new WpfShapeRenderer(),
                CurrentStyle = new DrawStyle(new DrawColor(255, 0, 255, 0), new DrawColor(80, 0, 255, 0), 2.0, true, true),
                PointShape = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
                {
                    Style = new DrawStyle(new DrawColor(0, 0, 0, 0), new DrawColor(255, 255, 255, 0), 2.0, false, true)
                }
            };

            ViewModel.CurrentTool = ViewModel.Tools[0];
        }

        private void Draw(DrawingContext dc, ShapesContainerViewModel vm)
        {
            foreach (var shape in vm.CurrentContainer.Guides)
            {
                shape.Draw(dc, vm.Renderer, 0.0, 0.0);
            }

            foreach (var shape in vm.CurrentContainer.Shapes)
            {
                shape.Draw(dc, vm.Renderer, 0.0, 0.0);
            }

            foreach (var shape in vm.WorkingContainer.Shapes)
            {
                shape.Draw(dc, vm.Renderer, 0.0, 0.0);
            }
        }

        private void DrawHelpers(DrawingContext dc, ShapesContainerViewModel vm)
        {
            foreach (var shape in vm.CurrentContainer.Shapes)
            {
                if (shape is LineShape line)
                {
                    if (vm.Selected.Contains(line))
                    {
                        _lineHelper.Draw(dc, vm.Renderer, line);
                    }
                }
                else if (shape is CubicBezierShape cubicBezier)
                {
                    if (vm.Selected.Contains(cubicBezier))
                    {
                        _cubiceBezierHelper.Draw(dc, vm.Renderer, cubicBezier);
                    }
                }
                else if (shape is QuadraticBezierShape quadraticBezier)
                {
                    if (vm.Selected.Contains(quadraticBezier))
                    {
                        _quadraticBezierHelper.Draw(dc, vm.Renderer, quadraticBezier);
                    }
                }
                else if (shape is PathShape path)
                {
                    if (vm.Selected.Contains(path))
                    {
                        _pathHelper.Draw(dc, vm.Renderer, path, vm);
                    }
                }
            }
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
            Draw(dc, ViewModel);
            DrawHelpers(dc, ViewModel);
        }
    }
}
