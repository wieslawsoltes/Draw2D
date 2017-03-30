﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Renderers;
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

        private void DrawHelpers(object dc, ShapeRenderer r, IList<ShapeObject> shapes, ISet<ShapeObject> selected)
        {
            foreach (var shape in shapes)
            {
                if (shape is LineShape line)
                {
                    if (selected.Contains(line))
                    {
                        _lineHelper.Draw(dc, r, line);
                    }
                }
                else if (shape is CubicBezierShape cubicBezier)
                {
                    if (selected.Contains(cubicBezier))
                    {
                        _cubiceBezierHelper.Draw(dc, r, cubicBezier);
                    }
                }
                else if (shape is QuadraticBezierShape quadraticBezier)
                {
                    if (selected.Contains(quadraticBezier))
                    {
                        _quadraticBezierHelper.Draw(dc, r, quadraticBezier);
                    }
                }
                else if (shape is PathShape path)
                {
                    if (selected.Contains(path))
                    {
                        _pathHelper.Draw(dc, r, path, selected);
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            ViewModel.Draw(dc);
            DrawHelpers(dc, ViewModel.Renderer, ViewModel.CurrentContainer.Shapes, ViewModel.Selected);
        }
    }
}
