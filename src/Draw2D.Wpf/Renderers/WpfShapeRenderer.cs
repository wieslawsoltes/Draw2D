﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IDictionary<MatrixObject, MatrixTransform> _matrixCache;
        private readonly IDictionary<CubicBezierShape, Geometry> _cubicGeometryCache;
        private readonly IDictionary<QuadraticBezierShape, Geometry> _quadGeometryCache;
        private readonly IDictionary<PathShape, Geometry> _pathGeometryCache;
        private ISet<ShapeObject> _selected;

        public override ISet<ShapeObject> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public WpfShapeRenderer()
        {
            _brushCache = new Dictionary<DrawStyle, WpfBrushCache>();
            _matrixCache = new Dictionary<MatrixObject, MatrixTransform>();
            _cubicGeometryCache = new Dictionary<CubicBezierShape, Geometry>();
            _quadGeometryCache = new Dictionary<QuadraticBezierShape, Geometry>();
            _pathGeometryCache = new Dictionary<PathShape, Geometry>();
            _selected = new HashSet<ShapeObject>();
        }

        private static Point FromPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static IEnumerable<Point> FromPoints(IEnumerable<PointShape> points, double dx, double dy)
        {
            return points.Select(point => new Point(point.X + dx, point.Y + dy));
        }

        private static Rect FromPoints(double x1, double y1, double x2, double y2, double dx, double dy)
        {
            double x = Math.Min(x1 + dx, x2 + dx);
            double y = Math.Min(y1 + dy, y2 + dy);
            double width = Math.Abs(Math.Max(x1 + dx, x2 + dx) - x);
            double height = Math.Abs(Math.Max(y1 + dy, y2 + dy) - y);
            return new Rect(x, y, width, height);
        }

        private static Rect FromRectnagle(RectangleShape rectangle, double dx, double dy)
        {
            return FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y,
                dx, dy);
        }

        private static Rect FromEllipse(EllipseShape ellipse, double dx, double dy)
        {
            return FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y,
                dx, dy);
        }

        private static void FromEllipse(EllipseShape ellipse, double dx, double dy, out double radiusX, out double radiusY, out Point center)
        {
            var rect = FromEllipse(ellipse, dx, dy);
            radiusX = rect.Width / 2;
            radiusY = rect.Height / 2;
            center = new Point(rect.TopLeft.X, rect.TopLeft.Y);
            center.Offset(radiusX, radiusY);
        }

        private WpfBrushCache? GetBrushCache(DrawStyle style)
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

        private static Geometry ToGeometry(IList<PointShape> points, DrawStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();
            var result = FromPoints(points, dx, dy);
            if (result.Count() >= 2)
            {
                using (var context = geometry.Open())
                {
                    context.BeginFigure(result.First(), style.IsFilled, false);
                    context.PolyLineTo(result.Skip(1).ToList(), style.IsStroked, false);
                }
            }
            return geometry;
        }

        private static Geometry ToGeometry(CubicBezierShape cubicBezier, DrawStyle style, double dx, double dy)
        {
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

            return geometry;
        }

        private static Geometry ToGeometry(QuadraticBezierShape quadraticBezier, DrawStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(FromPoint(quadraticBezier.StartPoint, dx, dy), style.IsFilled, false);
                context.QuadraticBezierTo(
                    FromPoint(quadraticBezier.Point1, dx, dy),
                    FromPoint(quadraticBezier.Point2, dx, dy),
                    style.IsStroked, false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(PathShape path, DrawStyle style, double dx, double dy)
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

        private Geometry GetGeometryCache(CubicBezierShape cubic, DrawStyle style, double dx, double dy)
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

        private Geometry GetGeometryCache(QuadraticBezierShape quad, DrawStyle style, double dx, double dy)
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

        private Geometry GetGeometryCache(PathShape path, DrawStyle style, double dx, double dy)
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

        public override void InvalidateCache(DrawStyle style)
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

        public override void InvalidateCache(ShapeObject shape, DrawStyle style, double dx, double dy)
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

        public override void DrawLine(object dc, LineShape line, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, FromPoint(line.StartPoint, dx, dy), FromPoint(line.Point, dx, dy));
        }

        public override void DrawPolyLine(object dc, IList<PointShape> points, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = ToGeometry(points, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(cubicBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(quadraticBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawPath(object dc, PathShape path, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(path, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawRectangle(object dc, RectangleShape rectangle, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = FromRectnagle(rectangle, dx, dy);
            _dc.DrawRectangle(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, rect);
        }

        public override void DrawEllipse(object dc, EllipseShape ellipse, DrawStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            FromEllipse(ellipse, dx, dy, out double radiusX, out double radiusY, out Point center);
            _dc.DrawEllipse(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, center, radiusX, radiusY);
        }
    }
}
