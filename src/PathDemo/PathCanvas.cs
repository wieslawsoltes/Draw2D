using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PathDemo
{
    public class PathCanvas : Canvas, IToolContext
    {
        public ObservableCollection<ShapeBase> Shapes { get; set; }
        public HashSet<ShapeBase> Selected { get; set; }

        public ObservableCollection<ToolBase> Tools { get; set; }
        public ToolBase CurrentTool { get; set; }

        public PathCanvas()
        {
            Shapes = new ObservableCollection<ShapeBase>();
            Selected = new HashSet<ShapeBase>();

            Tools = new ObservableCollection<ToolBase>();
            Tools.Add(new LineTool() { Name = "Line" });
            Tools.Add(new CubicBezierTool() { Name = "CubicBezier" });
            Tools.Add(new QuadraticBezierTool() { Name = "QuadraticBezier" });
            Tools.Add(new PathTool() { Name = "Path" });
            CurrentTool = Tools[0];
        }

        public PointShape GetNextPoint(Point point)
        {
            return PointShape.FromPoint(point);
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
                context.BeginFigure(line.StartPoint, false, false);
                context.LineTo(line.Point, true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawLineHelpers(DrawingContext dc, LineShape line)
        {
            var brushPoints = Brushes.Yellow;
            dc.DrawEllipse(brushPoints, null, line.StartPoint, 4, 4);
            dc.DrawEllipse(brushPoints, null, line.Point, 4, 4);
        }

        private void DrawCubicBezier(DrawingContext dc, CubicBezierShape cubicBezier)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(cubicBezier.StartPoint, false, false);
                context.BezierTo(cubicBezier.Point1, cubicBezier.Point2, cubicBezier.Point3, true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawCubicBezierHelpers(DrawingContext dc, CubicBezierShape cubicBezier)
        {
            var brushLines = Brushes.Cyan;
            var penLines = new Pen(brushLines, 2.0);
            var brushPoints = Brushes.Yellow;
            dc.DrawLine(penLines, cubicBezier.StartPoint, cubicBezier.Point1);
            dc.DrawLine(penLines, cubicBezier.Point3, cubicBezier.Point2);
            dc.DrawLine(penLines, cubicBezier.Point1, cubicBezier.Point2);
            dc.DrawEllipse(brushPoints, null, cubicBezier.StartPoint, 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point1, 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point2, 4, 4);
            dc.DrawEllipse(brushPoints, null, cubicBezier.Point3, 4, 4);
        }

        private void DrawQuadraticBezier(DrawingContext dc, QuadraticBezierShape quadraticBezier)
        {
            var brushPen = Brushes.Yellow;
            var penStroke = new Pen(brushPen, 2);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(quadraticBezier.StartPoint, false, false);
                context.QuadraticBezierTo(quadraticBezier.Point1, quadraticBezier.Point2, true, false);
            }
            dc.DrawGeometry(null, penStroke, geometry);
        }

        private void DrawQuadraticBezierHelpers(DrawingContext dc, QuadraticBezierShape quadraticBezier)
        {
            var brushLines = Brushes.Cyan;
            var penLines = new Pen(brushLines, 2.0);
            var brushPoints = Brushes.Yellow;
            dc.DrawLine(penLines, quadraticBezier.StartPoint, quadraticBezier.Point1);
            dc.DrawLine(penLines, quadraticBezier.Point1, quadraticBezier.Point2);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.StartPoint, 4, 4);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.Point1, 4, 4);
            dc.DrawEllipse(brushPoints, null, quadraticBezier.Point2, 4, 4);
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
                                context.BeginFigure(line.StartPoint, figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.LineTo(line.Point, true, false);
                        }
                        else if (segment is CubicBezierShape)
                        {
                            var cubicBezier = segment as CubicBezierShape;
                            if (isFirstSegment)
                            {
                                context.BeginFigure(cubicBezier.StartPoint, figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.BezierTo(cubicBezier.Point1, cubicBezier.Point2, cubicBezier.Point3, true, false);
                        }
                        else if (segment is QuadraticBezierShape)
                        {
                            var quadraticBezier = segment as QuadraticBezierShape;
                            if (isFirstSegment)
                            {
                                context.BeginFigure(quadraticBezier.StartPoint, figure.IsFilled, figure.IsClosed);
                                isFirstSegment = false;
                            }
                            context.QuadraticBezierTo(quadraticBezier.Point1, quadraticBezier.Point2, true, false);
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
