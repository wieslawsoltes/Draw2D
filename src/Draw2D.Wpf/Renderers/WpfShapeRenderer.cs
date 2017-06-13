// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Draw2D.Core.Renderer;
using Draw2D.Core.Shape;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Wpf.Renderers
{
    public class WpfShapeRenderer : ShapeRenderer
    {
        private readonly IDictionary<ShapeStyle, WpfBrushCache> _brushCache;
        private readonly IDictionary<MatrixObject, MatrixTransform> _matrixCache;
        private readonly IDictionary<CubicBezierShape, Geometry> _cubicGeometryCache;
        private readonly IDictionary<QuadraticBezierShape, Geometry> _quadGeometryCache;
        private readonly IDictionary<PathShape, Geometry> _pathGeometryCache;
        private ISet<BaseShape> _selected;

        public override ISet<BaseShape> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public WpfShapeRenderer()
        {
            _brushCache = new Dictionary<ShapeStyle, WpfBrushCache>();
            _matrixCache = new Dictionary<MatrixObject, MatrixTransform>();
            _cubicGeometryCache = new Dictionary<CubicBezierShape, Geometry>();
            _quadGeometryCache = new Dictionary<QuadraticBezierShape, Geometry>();
            _pathGeometryCache = new Dictionary<PathShape, Geometry>();
            _selected = new HashSet<BaseShape>();
        }

        private static Point ToPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static IEnumerable<Point> ToPoints(IEnumerable<PointShape> points, double dx, double dy)
        {
            return points.Select(point => new Point(point.X + dx, point.Y + dy));
        }

        private static Rect ToRect(PointShape p1, PointShape p2, double dx, double dy)
        {
            double x = Math.Min(p1.X + dx, p2.X + dx);
            double y = Math.Min(p1.Y + dy, p2.Y + dy);
            double width = Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - x);
            double height = Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - y);
            return new Rect(x, y, width, height);
        }

        private static void ToEllipse(EllipseShape ellipse, double dx, double dy, out double radiusX, out double radiusY, out Point center)
        {
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            radiusX = rect.Width / 2;
            radiusY = rect.Height / 2;
            center = new Point(rect.TopLeft.X, rect.TopLeft.Y);
            center.Offset(radiusX, radiusY);
        }

        private WpfBrushCache? GetBrushCache(ShapeStyle style)
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

        private static MatrixTransform ToMatrixTransform(MatrixObject matrix)
        {
            return new MatrixTransform(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        private MatrixTransform GetMatrixCache(MatrixObject matrix)
        {
            if (matrix == null)
            {
                return null;
            }
            if (!_matrixCache.TryGetValue(matrix, out var cache))
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
                return _matrixCache[matrix];
            }
            return cache;
        }

        private static Geometry ToGeometry(CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), style.IsFilled, false);
                context.BezierTo(
                    ToPoint(cubicBezier.Point1, dx, dy),
                    ToPoint(cubicBezier.Point2, dx, dy),
                    ToPoint(cubicBezier.Point3, dx, dy),
                    style.IsStroked, false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), style.IsFilled, false);
                context.QuadraticBezierTo(
                    ToPoint(quadraticBezier.Point1, dx, dy),
                    ToPoint(quadraticBezier.Point2, dx, dy),
                    style.IsStroked, false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(PathShape path, ShapeStyle style, double dx, double dy)
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
                                context.BeginFigure(ToPoint(line.StartPoint, dx, dy), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.LineTo(ToPoint(line.Point, dx, dy), true, false);
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.BezierTo(
                                ToPoint(cubicBezier.Point1, dx, dy),
                                ToPoint(cubicBezier.Point2, dx, dy),
                                ToPoint(cubicBezier.Point3, dx, dy),
                                true, false);
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), figure.IsFilled, figure.IsClosed);
                                isFirstShape = false;
                            }
                            context.QuadraticBezierTo(
                                ToPoint(quadraticBezier.Point1, dx, dy),
                                ToPoint(quadraticBezier.Point2, dx, dy),
                                true, false);
                        }
                    }
                }
            }

            return geometry;
        }

        private Geometry GetGeometryCache(CubicBezierShape cubic, ShapeStyle style, double dx, double dy)
        {
            if (cubic == null)
            {
                return null;
            }
            if (!_cubicGeometryCache.TryGetValue(cubic, out var cache))
            {
                var geometry = ToGeometry(cubic, style, dx, dy);
                if (geometry != null)
                {
                    _cubicGeometryCache[cubic] = geometry;
                    return _cubicGeometryCache[cubic];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(QuadraticBezierShape quad, ShapeStyle style, double dx, double dy)
        {
            if (quad == null)
            {
                return null;
            }
            if (!_quadGeometryCache.TryGetValue(quad, out var cache))
            {
                var geometry = ToGeometry(quad, style, dx, dy);
                if (geometry != null)
                {
                    _quadGeometryCache[quad] = geometry;
                    return _quadGeometryCache[quad];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(PathShape path, ShapeStyle style, double dx, double dy)
        {
            if (path == null)
            {
                return null;
            }
            if (!_pathGeometryCache.TryGetValue(path, out var cache))
            {
                var geometry = ToGeometry(path, style, dx, dy);
                if (geometry != null)
                {
                    _pathGeometryCache[path] = geometry;
                    return _pathGeometryCache[path];
                }
                return null;
            }
            return cache;
        }

        public override void InvalidateCache(ShapeStyle style)
        {
            if (style != null)
            {
                if (!_brushCache.TryGetValue(style, out var cache))
                {
                    cache.Dispose();
                }
                _brushCache[style] = WpfBrushCache.FromDrawStyle(style);
            }
        }

        public override void InvalidateCache(MatrixObject matrix)
        {
            if (matrix != null)
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
            }
        }

        public override void InvalidateCache(BaseShape shape, ShapeStyle style, double dx, double dy)
        {
            switch (shape)
            {
                case CubicBezierShape cubic:
                    {
                        var geometry = ToGeometry(cubic, style, dx, dy);
                        if (geometry != null)
                        {
                            _cubicGeometryCache[cubic] = geometry;
                        }
                    }
                    break;
                case QuadraticBezierShape quad:
                    {
                        var geometry = ToGeometry(quad, style, dx, dy);
                        if (geometry != null)
                        {
                            _quadGeometryCache[quad] = geometry;
                        }
                    }
                    break;
                case PathShape path:
                    {
                        var geometry = ToGeometry(path, style, dx, dy);
                        if (geometry != null)
                        {
                            _pathGeometryCache[path] = geometry;
                        }
                    }
                    break;
            }
        }

        public override object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            _dc.PushTransform(GetMatrixCache(matrix));
            return null;
        }

        public override void PopMatrix(object dc, object state)
        {
            var _dc = dc as DrawingContext;
            _dc.Pop();
        }

        public override void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, ToPoint(line.StartPoint, dx, dy), ToPoint(line.Point, dx, dy));
        }

        public override void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(cubicBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(quadraticBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(path, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            _dc.DrawRectangle(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, rect);
        }

        public override void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            ToEllipse(ellipse, dx, dy, out double radiusX, out double radiusY, out Point center);
            _dc.DrawEllipse(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, center, radiusX, radiusY);
        }

        public override void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);
            _dc.DrawRectangle(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, rect);
            if (text.Text != null)
            {
                // TODO: Draw text Value string.
            }
        }
    }
}
