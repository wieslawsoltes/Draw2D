using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;
using PathDemo.Tools;

namespace PathDemo.Controls
{
    public class PathCanvas : Canvas, IToolContext
    {
        public ISet<ShapeObject> Selected { get; set; }

        public IShapesContainer CurrentContainer { get; set; }

        public IShapesContainer WorkingContainer { get; set; }

        public DrawStyle CurrentStyle { get; set; }

        public ShapeObject PointShape { get; set; }

        public HitTest HitTest { get; set; }

        public PointShape GetNextPoint(double x, double y) => new PointShape() { X = x, Y = y };

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }

        public ObservableCollection<ToolBase> Tools { get; set; }

        public ToolBase CurrentTool { get; set; }

        public ShapeRenderer Renderer { get; set; }

        public PathCanvas()
        {
            Selected = new HashSet<ShapeObject>();
            CurrentContainer = new ShapesContainer();

            Capture = () => this.CaptureMouse();
            Release = () => this.ReleaseMouseCapture();
            Invalidate = () => this.InvalidateVisual();

            Tools = new ObservableCollection<ToolBase>
            {
                new LineTool(),
                new CubicBezierTool(),
                new QuadraticBezierTool(),
                new PathTool()
            };

            CurrentTool = Tools[0];

            Renderer = new ShapeRenderer();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            var point = e.GetPosition(this);
            CurrentTool.LeftDown(this, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            var point = e.GetPosition(this);
            CurrentTool.LeftUp(this, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            var point = e.GetPosition(this);
            CurrentTool.RightDown(this, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);
            var point = e.GetPosition(this);
            CurrentTool.RightUp(this, point.X, point.Y, Modifier.None);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            var point = e.GetPosition(this);
            CurrentTool.Move(this, point.X, point.Y, Modifier.None);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Renderer.Draw(dc, this);
        }
    }
}
