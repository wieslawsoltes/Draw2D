// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Draw2D.Core;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Wpf.Renderers
{
    public class WpfShapeRenderer : ShapeRenderer
    {
        private readonly IDictionary<DrawStyle, WpfBrushCache> _brushCache;

        private ISet<ShapeObject> _selected;

        public override ISet<ShapeObject> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public WpfShapeRenderer()
        {
            _brushCache = new Dictionary<DrawStyle, WpfBrushCache>();
            _selected = new HashSet<ShapeObject>();
        }

        private Point FromPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static List<Point> FromPoints(IList<PointShape> points, double dx, double dy)
        {
            var result = new List<Point>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                result.Add(new Point(point.X + dx, point.Y + dy));
            }
            return result;
        }

        private Rect FromPoints(double x1, double y1, double x2, double y2, double dx, double dy)
        {
            double x = Math.Min(x1 + dx, x2 + dx);
            double y = Math.Min(y1 + dy, y2 + dy);
            double width = Math.Abs(Math.Max(x1 + dx, x2 + dx) - x);
            double height = Math.Abs(Math.Max(y1 + dy, y2 + dy) - y);
            return new Rect(x, y, width, height);
        }

        private Rect FromRectnagle(RectangleShape rectangle, double dx, double dy)
        {
            return FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                dx, dy);
        }

        private Rect FromEllipse(EllipseShape ellipse, double dx, double dy)
        {
            return FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                dx, dy);
        }

        private Geometry ToGeometry(PathShape path, double dx, double dy)
        {
            var geometry = new StreamGeometry()
            {
                FillRule = path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.Nonzero
            };

            using (var context = geometry.Open())
            {
                foreach (var figure in path.Figures)
                {
                    bool isFirstShape = true;
                    foreach (var shape in figure.Shapes)
                    {
                        if (shape is LineShape line)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(FromPoint(line.StartPoint, dx, dy), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.LineTo(FromPoint(line.Point, dx, dy), true, false);
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(FromPoint(cubicBezier.StartPoint, dx, dy), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.BezierTo(
                                FromPoint(cubicBezier.Point1, dx, dy),
                                FromPoint(cubicBezier.Point2, dx, dy),
                                FromPoint(cubicBezier.Point3, dx, dy),
                                true, false);
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(FromPoint(quadraticBezier.StartPoint, dx, dy), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.QuadraticBezierTo(
                                FromPoint(quadraticBezier.Point1, dx, dy),
                                FromPoint(quadraticBezier.Point2, dx, dy),
                                true, false);
                        }
                    }
                }
            }

            return geometry;
        }

        private WpfBrushCache? GetOrCreateCache(DrawStyle style)
        {
            if (style == null)
            {
                return null;
            }
            if (!_brushCache.TryGetValue(style, out var cache))
            {
                _brushCache[style] = WpfBrushCache.FromDrawStyle(style);
                return _brushCache[style];
            }
            return cache;
        }

        private MatrixTransform ToMatrixTransform(MatrixObject matrix)
        {
            return new MatrixTransform(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        public override object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            _dc.PushTransform(ToMatrixTransform(matrix));
            return null;
        }

        public override void PopMatrix(object dc, object state)
        {
            var _dc = dc as DrawingContext;
            _dc.Pop();
        }

        public override void DrawLine(object dc, LineShape line, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, FromPoint(line.StartPoint, dx, dy), FromPoint(line.Point, dx, dy));
        }

        public override void DrawPolyLine(object dc, PointShape start, IList<PointShape> points, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(FromPoint(start, dx, dy), style.IsFilled, false);
                context.PolyLineTo(FromPoints(points, dx, dy), style.IsStroked, false);
            }
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(FromPoint(cubicBezier.StartPoint, dx, dy), style.IsFilled, false);
                context.BezierTo(
                    FromPoint(cubicBezier.Point1, dx, dy),
                    FromPoint(cubicBezier.Point2, dx, dy),
                    FromPoint(cubicBezier.Point3, dx, dy),
                    style.IsStroked, false);
            }
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(FromPoint(quadraticBezier.StartPoint, dx, dy), style.IsFilled, false);
                context.QuadraticBezierTo(
                    FromPoint(quadraticBezier.Point1, dx, dy),
                    FromPoint(quadraticBezier.Point2, dx, dy),
                    style.IsStroked, false);
            }
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawPath(object dc, PathShape path, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = ToGeometry(path, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawRectangle(object dc, RectangleShape rectangle, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromRectnagle(rectangle, dx, dy);
            _dc.DrawRectangle(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, rect);
        }

        public override void DrawEllipse(object dc, EllipseShape ellipse, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromEllipse(ellipse, dx, dy);
            var radiusX = rect.Width / 2;
            var radiusY = rect.Height / 2;
            var center = new Point(rect.TopLeft.X, rect.TopLeft.Y);
            center.Offset(radiusX, radiusY);
            _dc.DrawEllipse(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, center, radiusX, radiusY);
        }
    }
}
