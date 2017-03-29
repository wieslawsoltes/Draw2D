using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Models;
using Draw2D.Models.Shapes;

namespace PathDemo.Controls
{
    public class PathCanvas : Canvas, IToolContext
    {
        public ISet<ShapeObject> Selected { get; set; }
        public ObservableCollection<ShapeObject> Shapes { get; set; }

        public ObservableCollection<ToolBase> Tools { get; set; }
        public ToolBase CurrentTool { get; set; }

        public PathCanvas()
        {
            Shapes = new ObservableCollection<ShapeObject>();
            Selected = new HashSet<ShapeObject>();

            Tools = new ObservableCollection<ToolBase>();
            Tools.Add(new LineTool() { Name = "Line" });
            Tools.Add(new CubicBezierTool() { Name = "CubicBezier" });
            Tools.Add(new QuadraticBezierTool() { Name = "QuadraticBezier" });
            Tools.Add(new PathTool() { Name = "Path" });
            CurrentTool = Tools[0];
        }

        public PointShape GetNextPoint(Point point)
        {
            return new PointShape() { X = point.X, Y = point.Y };
        }

        public void Capture()
        {
            this.CaptureMouse();
        }

        public void Release()
        {
            this.ReleaseMouseCapture();
        }

        public void Invalidate()
        {
            this.InvalidateVisual();
        }

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

        private void DrawLine(DrawingContext dc, LineShape line)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(line.StartPoint.AsPoint(), false, false);
                context.LineTo(line.Point.AsPoint(), true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawLineHelpers(DrawingContext dc, LineShape line)
        {
            var brushPoints = Brushes.Yellow;
            dc.DrawEllipse(brushPoints, null, line.StartPoint.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, line.Point.AsPoint(), 4, 4);
        }

        private void DrawCubicBezier(DrawingContext dc, CubicBezierShape cubicBezier)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(cubicBezier.StartPoint.AsPoint(), false, false);
                context.BezierTo(cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint(), cubicBezier.Point3.AsPoint(), true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawCubicBezierHelpers(DrawingContext dc, CubicBezierShape cubicBezier)
        {
            var brushLines = Brushes.Cyan;
            var penLines = new Pen(brushLines, 2.0);
            var brushPoints = Brushes.Yellow;
            dc.DrawLine(penLines, cubicBezier.StartPoint.AsPoint(), cubicBezier.Point1.AsPoint());
            dc.DrawLine(penLines, cubicBezier.Point3.AsPoint(), cubicBezier.Point2.AsPoint());
            dc.DrawLine(penLines, cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint());
            dc.DrawEllipse(brushPoints, null, cubicBezier.StartPoint.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point1.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point2.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point3.AsPoint(), 4, 4);
        }

        private void DrawQuadraticBezier(DrawingContext dc, QuadraticBezierShape quadraticBezier)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(quadraticBezier.StartPoint.AsPoint(), false, false);
                context.QuadraticBezierTo(quadraticBezier.Point1.AsPoint(), quadraticBezier.Point2.AsPoint(), true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawQuadraticBezierHelpers(DrawingContext dc, QuadraticBezierShape quadraticBezier)
        {
            var brushLines = Brushes.Cyan;
            var penLines = new Pen(brushLines, 2.0);
            var brushPoints = Brushes.Yellow;
            dc.DrawLine(penLines, quadraticBezier.StartPoint.AsPoint(), quadraticBezier.Point1.AsPoint());
            dc.DrawLine(penLines, quadraticBezier.Point1.AsPoint(), quadraticBezier.Point2.AsPoint());
            dc.DrawEllipse(brushPoints, null, quadraticBezier.StartPoint.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.Point1.AsPoint(), 4, 4);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.Point2.AsPoint(), 4, 4);
        }

        private Geometry ToGeometry(PathShape path)
        {
            var geometry = new StreamGeometry()
            {
                FillRule = path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero
            };

            using (var context = geometry.Open())
            {
                foreach (var figure in path.Figures)
                {
                    bool isFirstSegment = true;
                    foreach (var segment in figure.Segments)
                    {
                        if (segment is LineShape)
                        {
                            var line = segment as LineShape;
                            if (isFirstSegment)
                            {
                                context.BeginFigure(line.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.LineTo(line.Point.AsPoint(), true, false);
                        }
                        else if (segment is CubicBezierShape)
                        {
                            var cubicBezier = segment as CubicBezierShape;
                            if (isFirstSegment)
                            {
                                context.BeginFigure(cubicBezier.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.BezierTo(cubicBezier.Point1.AsPoint(), cubicBezier.Point2.AsPoint(), cubicBezier.Point3.AsPoint(), true, false);
                        }
                        else if (segment is QuadraticBezierShape)
                        {
                            var quadraticBezier = segment as QuadraticBezierShape;
                            if (isFirstSegment)
                            {
                                context.BeginFigure(quadraticBezier.StartPoint.AsPoint(), figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.QuadraticBezierTo(quadraticBezier.Point1.AsPoint(), quadraticBezier.Point2.AsPoint(), true, false);
                        }
                    }
                }
            }

            return geometry;
        }

        private void DrawPath(DrawingContext dc, PathShape path)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = ToGeometry(path);
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawPathHelpers(DrawingContext dc, PathShape path)
        {
            foreach (var figure in path.Figures)
            {
                foreach (var segment in figure.Segments)
                {
                    if (segment is LineShape)
                    {
                        var line = segment as LineShape;
                        if (Selected.Contains(line))
                        {
                            DrawLineHelpers(dc, line);
                        }
                    }
                    else if (segment is CubicBezierShape)
                    {
                        var cubicBezier = segment as CubicBezierShape;
                        if (Selected.Contains(cubicBezier))
                        {
                            DrawCubicBezierHelpers(dc, cubicBezier);
                        }
                    }
                    else if (segment is QuadraticBezierShape)
                    {
                        var quadraticBezier = segment as QuadraticBezierShape;
                        DrawQuadraticBezier(dc, quadraticBezier);
                        if (Selected.Contains(quadraticBezier))
                        {
                            DrawQuadraticBezierHelpers(dc, quadraticBezier);
                        }
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            foreach (var shape in Shapes)
            {
                if (shape is LineShape)
                {
                    var line = shape as LineShape;
                    DrawLine(dc, line);
                    if (Selected.Contains(line))
                    {
                        DrawLineHelpers(dc, line);
                    }
                }
                else if (shape is CubicBezierShape)
                {
                    var cubicBezier = shape as CubicBezierShape;
                    DrawCubicBezier(dc, cubicBezier);
                    if (Selected.Contains(cubicBezier))
                    {
                        DrawCubicBezierHelpers(dc, cubicBezier);
                    }
                }
                else if (shape is QuadraticBezierShape)
                {
                    var quadraticBezier = shape as QuadraticBezierShape;
                    DrawQuadraticBezier(dc, quadraticBezier);
                    if (Selected.Contains(quadraticBezier))
                    {
                        DrawQuadraticBezierHelpers(dc, quadraticBezier);
                    }
                }
                else if (shape is PathShape)
                {
                    var path = shape as PathShape;
                    DrawPath(dc, path);
                    if (Selected.Contains(path))
                    {
                        DrawPathHelpers(dc, path);
                    }
                }
            }
        }
    }
}
