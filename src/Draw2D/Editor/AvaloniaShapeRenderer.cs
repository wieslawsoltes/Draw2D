// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;

namespace Draw2D.Editor
{
    public class AvaloniaShapeRenderer : ViewModelBase, IShapeRenderer
    {
        private readonly IDictionary<ShapeStyle, AvaloniaBrushCache> _brushCache;
        private readonly IDictionary<MatrixObject, Matrix> _matrixCache;
        private readonly IDictionary<CubicBezierShape, Geometry> _cubicGeometryCache;
        private readonly IDictionary<QuadraticBezierShape, Geometry> _quadGeometryCache;
        private readonly IDictionary<PathShape, Geometry> _pathGeometryCache;
        private readonly IDictionary<EllipseShape, Geometry> _ellipseGeometryCache;
        private readonly IDictionary<TextShape, AvaloniaFormattedTextCache> _formattedTextCache;

        private ISelection _selection;

        public ISelection Selection
        {
            get => _selection;
            set => Update(ref _selection, value);
        }

        public AvaloniaShapeRenderer()
        {
            _brushCache = new Dictionary<ShapeStyle, AvaloniaBrushCache>();
            _formattedTextCache = new Dictionary<TextShape, AvaloniaFormattedTextCache>();
            _matrixCache = new Dictionary<MatrixObject, Matrix>();
            _cubicGeometryCache = new Dictionary<CubicBezierShape, Geometry>();
            _quadGeometryCache = new Dictionary<QuadraticBezierShape, Geometry>();
            _pathGeometryCache = new Dictionary<PathShape, Geometry>();
            _ellipseGeometryCache = new Dictionary<EllipseShape, Geometry>();
        }

        private static Point ToPoint(PointShape point, double dx, double dy, double zx, double zy)
        {
            return new Point((point.X + dx) * zx, (point.Y + dy) * zy);
        }

        public static IEnumerable<Point> ToPoints(IEnumerable<PointShape> points, double dx, double dy, double zx, double zy)
        {
            return points.Select(point => new Point((point.X + dx) * zx, (point.Y + dy) * zy));
        }

        private static Rect ToRect(PointShape p1, PointShape p2, double dx, double dy, double zx, double zy)
        {
            double x = Math.Min(p1.X + dx, p2.X + dx);
            double y = Math.Min(p1.Y + dy, p2.Y + dy);
            double width = Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - x);
            double height = Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - y);
            return new Rect(x * zx, y * zy, width * zx, height * zy);
        }

        private AvaloniaBrushCache? GetBrushCache(ShapeStyle style)
        {
            if (style == null)
            {
                return null;
            }
            if (!_brushCache.TryGetValue(style, out var cache))
            {
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
                return _brushCache[style];
            }
            return cache;
        }

        private static Matrix ToMatrixTransform(MatrixObject m)
        {
            return new Matrix(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        private Matrix? GetMatrixCache(MatrixObject matrix)
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

        private static Geometry ToGeometry(CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy, zx, zy), false);
                context.CubicBezierTo(
                    ToPoint(cubicBezier.Point1, dx, dy, zx, zy),
                    ToPoint(cubicBezier.Point2, dx, dy, zx, zy),
                    ToPoint(cubicBezier.Point3, dx, dy, zx, zy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy, zx, zy), false);
                context.QuadraticBezierTo(
                    ToPoint(quadraticBezier.Point1, dx, dy, zx, zy),
                    ToPoint(quadraticBezier.Point2, dx, , zx, zy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(PathShape path, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.SetFillRule(path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero);

                foreach (var figure in path.Figures)
                {
                    bool isFirstShape = true;
                    foreach (var shape in figure.Shapes)
                    {
                        if (shape is LineShape line)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(line.StartPoint, dx, dy, zx, zy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.LineTo(ToPoint(line.Point, dx, dy, zx, zy));
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy, zx, zy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.CubicBezierTo(
                                ToPoint(cubicBezier.Point1, dx, dy, zx, zy),
                                ToPoint(cubicBezier.Point2, dx, dy, zx, zy),
                                ToPoint(cubicBezier.Point3, dx, dy, zx, zy));
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy, zx, zy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.QuadraticBezierTo(
                                ToPoint(quadraticBezier.Point1, dx, dy, zx, zy),
                                ToPoint(quadraticBezier.Point2, dx, dy, zx, zy));
                        }
                    }

                    if (!isFirstShape)
                    {
                        context.EndFigure(figure.IsClosed);
                    }
                }
            }

            return geometry;
        }

        private static Geometry ToGeometry(EllipseShape ellipse, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy, zx, zy);
            return new EllipseGeometry(rect);
        }

        private Geometry GetGeometryCache(CubicBezierShape cubic, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            if (cubic == null)
            {
                return null;
            }
            if (!_cubicGeometryCache.TryGetValue(cubic, out var cache))
            {
                var geometry = ToGeometry(cubic, style, dx, dy, zx, zy);
                if (geometry != null)
                {
                    _cubicGeometryCache[cubic] = geometry;
                    return _cubicGeometryCache[cubic];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(QuadraticBezierShape quad, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            if (quad == null)
            {
                return null;
            }
            if (!_quadGeometryCache.TryGetValue(quad, out var cache))
            {
                var geometry = ToGeometry(quad, style, dx, dy, zx, zy);
                if (geometry != null)
                {
                    _quadGeometryCache[quad] = geometry;
                    return _quadGeometryCache[quad];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(PathShape path, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            if (path == null)
            {
                return null;
            }
            if (!_pathGeometryCache.TryGetValue(path, out var cache))
            {
                var geometry = ToGeometry(path, style, dx, dy, zx, zy);
                if (geometry != null)
                {
                    _pathGeometryCache[path] = geometry;
                    return _pathGeometryCache[path];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(EllipseShape ellipse, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            if (ellipse == null)
            {
                return null;
            }
            if (!_ellipseGeometryCache.TryGetValue(ellipse, out var cache))
            {
                var geometry = ToGeometry(ellipse, style, dx, dy, zx, zy);
                if (geometry != null)
                {
                    _ellipseGeometryCache[ellipse] = geometry;
                    return _ellipseGeometryCache[ellipse];
                }
                return null;
            }
            return cache;
        }

        private AvaloniaFormattedTextCache GetTextCache(TextShape text, Rect rect, double zx, double zy)
        {
            if (!_formattedTextCache.TryGetValue(text, out var cache))
            {
                _formattedTextCache[text] = AvaloniaFormattedTextCache.FromTextShape(text, rect, zx, zy);
                return _formattedTextCache[text];
            }
            return cache;
        }

        public void InvalidateCache(ShapeStyle style)
        {
            if (style != null)
            {
                if (!_brushCache.TryGetValue(style, out var cache))
                {
                    cache.Dispose();
                }
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
            }
        }

        public void InvalidateCache(MatrixObject matrix)
        {
            if (matrix != null)
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
            }
        }

        public void InvalidateCache(BaseShape shape, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            switch (shape)
            {
                case CubicBezierShape cubic:
                    {
                        var geometry = ToGeometry(cubic, style, dx, dy, zx, zy);
                        if (geometry != null)
                        {
                            _cubicGeometryCache[cubic] = geometry;
                        }
                    }
                    break;
                case QuadraticBezierShape quad:
                    {
                        var geometry = ToGeometry(quad, style, dx, dy, zx, zy);
                        if (geometry != null)
                        {
                            _quadGeometryCache[quad] = geometry;
                        }
                    }
                    break;
                case PathShape path:
                    {
                        var geometry = ToGeometry(path, style, dx, dy, zx, zy);
                        if (geometry != null)
                        {
                            _pathGeometryCache[path] = geometry;
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        var geometry = ToGeometry(ellipse, style, dx, dy, zx, zy);
                        if (geometry != null)
                        {
                            _ellipseGeometryCache[ellipse] = geometry;
                        }
                    }
                    break;
                case TextShape text:
                    {
                        if (!_formattedTextCache.TryGetValue(text, out var cache))
                        {
                            cache.Dispose();
                        }
                        var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy, zx, zy);
                        _formattedTextCache[text] = AvaloniaFormattedTextCache.FromTextShape(text, rect);
                    }
                    break;
            }
        }

        public object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            return _dc.PushPreTransform(GetMatrixCache(matrix).Value);
        }

        public void PopMatrix(object dc, object state)
        {
            var _state = (DrawingContext.PushedState)state;
            _state.Dispose();
        }

        public void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, ToPoint(line.StartPoint, dx, dy), ToPoint(line.Point, dx, dy));
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(cubicBezier, style, dx, dy, zx, zy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(quadraticBezier, style, dx, dy, zx, zy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(path, style, dx, dy, zx, zy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy, zx, zy);
            if (style.IsFilled)
            {
                _dc.FillRectangle(cache?.Fill, rect);
            }
            if (style.IsStroked)
            {
                _dc.DrawRectangle(cache?.StrokePen, rect);
            }
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(ellipse, style, dx, dy, zx, zy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy, double zx, double zy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy, zx, zy);
            if (text.Text != null)
            {
                var ftc = GetTextCache(text, rect, zx, zy);
                _dc.DrawText(cache?.Stroke, ftc.Origin, ftc.FormattedText);
            }
        }
    }
}
