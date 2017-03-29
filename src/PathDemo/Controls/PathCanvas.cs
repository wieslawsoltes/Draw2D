using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Models;
using Draw2D.Models.Shapes;
using PathDemo.Tools;

namespace PathDemo.Controls
{
    public class PathCanvas : Canvas, IToolContext
    {
        public ISet<ShapeObject> Selected { get; set; }
        public ObservableCollection<ShapeObject> Shapes { get; set; }
        public ObservableCollection<ToolBase> Tools { get; set; }
        public ToolBase CurrentTool { get; set; }
        public ShapeRenderer Renderer { get; set; }

        public PathCanvas()
        {
            Shapes = new ObservableCollection<ShapeObject>();
            Selected = new HashSet<ShapeObject>();
            Tools = new ObservableCollection<ToolBase>
            {
                new LineTool() { Name = "Line" },
                new CubicBezierTool() { Name = "CubicBezier" },
                new QuadraticBezierTool() { Name = "QuadraticBezier" },
                new PathTool() { Name = "Path" }
            };
            CurrentTool = Tools[0];
            Renderer = new ShapeRenderer();
        }

        public PointShape GetNextPoint(Point point) => new PointShape() { X = point.X, Y = point.Y };

        public void Capture() => this.CaptureMouse();

        public void Release() => this.ReleaseMouseCapture();

        public void Invalidate() => this.InvalidateVisual();

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            CurrentTool.LeftDown(this, e.GetPosition(this));
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            CurrentTool.LeftUp(this, e.GetPosition(this));
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            CurrentTool.RightDown(this, e.GetPosition(this));
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);
            CurrentTool.RightUp(this, e.GetPosition(this));
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            CurrentTool.Move(this, e.GetPosition(this));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Renderer.Draw(dc, this);
        }
    }
}
