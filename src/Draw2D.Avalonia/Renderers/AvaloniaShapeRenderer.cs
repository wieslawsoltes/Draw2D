// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Draw2D.Core;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Avalonia.Renderers
{
    public class AvaloniaShapeRenderer : ShapeRenderer
    {
        private readonly IDictionary<DrawStyle, AvaloniaBrushCache> _brushCache;

        private ISet<ShapeObject> _selected;

        public override ISet<ShapeObject> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public AvaloniaShapeRenderer()
        {
            _brushCache = new Dictionary<DrawStyle, AvaloniaBrushCache>();
            _selected = new HashSet<ShapeObject>();
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

        private static Geometry ToGeometry(CubicBezierShape cubicBezier, DrawStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), false);
                context.CubicBezierTo(
                    ToPoint(cubicBezier.Point1, dx, dy),
                    ToPoint(cubicBezier.Point2, dx, dy),
                    ToPoint(cubicBezier.Point3, dx, dy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(QuadraticBezierShape quadraticBezier, DrawStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), false);
                context.QuadraticBezierTo(
                    ToPoint(quadraticBezier.Point1, dx, dy),
                    ToPoint(quadraticBezier.Point2, dx, dy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(PathShape path, DrawStyle style, double dx, double dy)
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
                                context.BeginFigure(ToPoint(line.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.LineTo(ToPoint(line.Point, dx, dy));
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.CubicBezierTo(
                                ToPoint(cubicBezier.Point1, dx, dy),
                                ToPoint(cubicBezier.Point2, dx, dy),
                                ToPoint(cubicBezier.Point3, dx, dy));
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.QuadraticBezierTo(
                                ToPoint(quadraticBezier.Point1, dx, dy),
                                ToPoint(quadraticBezier.Point2, dx, dy));
                        }
                    }

                    context.EndFigure(figure.IsClosed);
                }
            }

            return geometry;
        }

        private static Geometry ToGeometry(EllipseShape ellipse, DrawStyle style, double dx, double dy)
        {
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            return new EllipseGeometry(rect);
        }

        private AvaloniaBrushCache? GetOrCreateCache(DrawStyle style)
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

        private static Matrix ToMatrixTransform(MatrixObject matrix)
        {
            return new Matrix(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        public override void InvalidateCache(DrawStyle style)
        {
            // TODO: Implement InvalidateCache method.
        }

        public override void InvalidateCache(MatrixObject matrix)
        {
            // TODO: Implement InvalidateCache method.
        }

        public override void InvalidateCache(ShapeObject shape, DrawStyle style, double dx, double dy)
        {
            // TODO: Implement InvalidateCache method.
        }

        public override object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            return _dc.PushPreTransform(ToMatrixTransform(matrix));
        }

        public override void PopMatrix(object dc, object state)
        {
            var _state = (DrawingContext.PushedState)state;
            _state.Dispose();
        }

        public override void DrawLine(object dc, LineShape line, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, ToPoint(line.StartPoint, dx, dy), ToPoint(line.Point, dx, dy));
        }

        public override void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = ToGeometry(cubicBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = ToGeometry(quadraticBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawPath(object dc, PathShape path, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = ToGeometry(path, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawRectangle(object dc, RectangleShape rectangle, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            if (style.IsFilled)
            {
                _dc.FillRectangle(cache?.Fill, rect);
            }
            if (style.IsStroked)
            {
                _dc.DrawRectangle(cache?.StrokePen, rect);
            }
        }

        public override void DrawEllipse(object dc, EllipseShape ellipse, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var geometry = ToGeometry(ellipse, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public override void DrawText(object dc, TextShape text, DrawStyle style, double dx, double dy)
        {
            var cache = GetOrCreateCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);
            if (style.IsFilled)
            {
                _dc.FillRectangle(cache?.Fill, rect);
            }
            if (style.IsStroked)
            {
                _dc.DrawRectangle(cache?.StrokePen, rect);
            }
            if (text.Text != null)
            {
                // TODO: Draw text Value string.
            }
        }
    }
}
