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

namespace Draw2D.Renderers
{
    internal class AvaloniaHelper
    {
        public static Color ToColor(ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Point ToPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static IEnumerable<Point> ToPoints(IEnumerable<PointShape> points, double dx, double dy)
        {
            return points.Select(point => new Point(point.X + dx, point.Y + dy));
        }

        public static Rect ToRect(PointShape p1, PointShape p2, double dx, double dy)
        {
            double x = Math.Min(p1.X + dx, p2.X + dx);
            double y = Math.Min(p1.Y + dy, p2.Y + dy);
            double width = Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - x);
            double height = Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - y);
            return new Rect(x, y, width, height);
        }

        public static Matrix ToMatrixTransform(Matrix2 m)
        {
            return new Matrix(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        public static Geometry ToGeometry<T>(T shape, ShapeStyle style, double dx, double dy) where T : BaseShape
        {
            switch (shape)
            {
                case CubicBezierShape cubicBezier:
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
                case QuadraticBezierShape quadraticBezier:
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
                case ConicShape conic:
                    {
                        var geometry = new StreamGeometry();

                        using (var context = geometry.Open())
                        {
                            context.BeginFigure(ToPoint(conic.StartPoint, dx, dy), false);
                            // TODO: Add support for ConicTo
                            context.EndFigure(false);
                        }
                        return geometry;
                    }
                case PathShape path:
                    {
                        var geometry = new StreamGeometry();

                        using (var context = geometry.Open())
                        {
                            context.SetFillRule(path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero);

                            foreach (var figure in path.Figures)
                            {
                                bool isFirstShape = true;
                                foreach (var figureShape in figure.Shapes)
                                {
                                    if (figureShape is LineShape line)
                                    {
                                        if (isFirstShape)
                                        {
                                            context.BeginFigure(ToPoint(line.StartPoint, dx, dy), figure.IsFilled);
                                            isFirstShape = false;
                                        }
                                        context.LineTo(ToPoint(line.Point, dx, dy));
                                    }
                                    else if (figureShape is CubicBezierShape cubicBezier)
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
                                    else if (figureShape is QuadraticBezierShape quadraticBezier)
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

                                if (!isFirstShape)
                                {
                                    context.EndFigure(figure.IsClosed);
                                }
                            }
                        }
                        return geometry;
                    }
                case EllipseShape ellipse:
                    {
                        var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
                        return new EllipseGeometry(rect);
                    }
            }
            return null;
        }
    }

    internal struct AvaloniaBrushCache : IDisposable
    {
        public readonly Brush Stroke;

        public readonly Pen StrokePen;

        public readonly Brush Fill;

        public AvaloniaBrushCache(Brush stroke, Pen strokePen, Brush fill)
        {
            this.Stroke = stroke;
            this.StrokePen = strokePen;
            this.Fill = fill;
        }

        public static AvaloniaBrushCache FromDrawStyle(ShapeStyle style)
        {
            Brush stroke = null;
            Pen strokePen = null;
            Brush fill = null;

            if (style.Stroke != null)
            {
                stroke = new SolidColorBrush(AvaloniaHelper.ToColor(style.Stroke));
                strokePen = new Pen(stroke, style.Thickness);
            }

            if (style.Fill != null)
            {
                fill = new SolidColorBrush(AvaloniaHelper.ToColor(style.Fill));
            }

            return new AvaloniaBrushCache(stroke, strokePen, fill);
        }

        public void Dispose()
        {
        }
    }

    internal struct AvaloniaFormattedTextCache : IDisposable
    {
        public readonly FormattedText FormattedText;

        public readonly Point Origin;

        public AvaloniaFormattedTextCache(FormattedText formattedText, Point origin)
        {
            FormattedText = formattedText;
            Origin = origin;
        }

        public static AvaloniaFormattedTextCache FromTextShape(TextShape text, Rect rect, TextStyle style)
        {
            var constraint = new Size(rect.Width, rect.Height);

            TextAlignment textAlignment;
            switch (style.HAlign)
            {
                case HAlign.Left:
                    textAlignment = TextAlignment.Left;
                    break;
                case HAlign.Center:
                default:
                    textAlignment = TextAlignment.Center;
                    break;
                case HAlign.Right:
                    textAlignment = TextAlignment.Right;
                    break;
            }

            var formattedText = new FormattedText()
            {
                Text = text.Text.Value,
                Constraint = constraint,
                TextAlignment = textAlignment,
                Wrapping = TextWrapping.NoWrap,
                Typeface = new Typeface(style.FontFamily, style.FontSize)
            };

            var size = formattedText.Bounds.Size;

            Point origin;
            switch (style.VAlign)
            {
                case VAlign.Top:
                    origin = new Point(rect.X, rect.Y);
                    break;
                case VAlign.Center:
                default:
                    origin = new Point(rect.X, rect.Y + rect.Height / 2 - size.Height / 2);
                    break;
                case VAlign.Bottom:
                    origin = new Point(rect.X, rect.Y + rect.Height - size.Height);
                    break;
            }

            return new AvaloniaFormattedTextCache(formattedText, origin);
        }

        public void Dispose()
        {
        }
    }

    public class AvaloniaShapeRenderer : IShapeRenderer
    {
        private readonly IDictionary<ShapeStyle, AvaloniaBrushCache> _brushCache;
        private readonly IDictionary<Matrix2, Matrix> _matrixCache;
        private readonly IDictionary<CubicBezierShape, Geometry> _cubicGeometryCache;
        private readonly IDictionary<QuadraticBezierShape, Geometry> _quadGeometryCache;
        private readonly IDictionary<ConicShape, Geometry> _conicGeometryCache;
        private readonly IDictionary<PathShape, Geometry> _pathGeometryCache;
        private readonly IDictionary<EllipseShape, Geometry> _ellipseGeometryCache;
        private readonly IDictionary<TextShape, AvaloniaFormattedTextCache> _formattedTextCache;

        public double Scale { get; set; } = 1.0;

        public ISelection Selection { get; set; } = null;

        public AvaloniaShapeRenderer()
        {
            _brushCache = new Dictionary<ShapeStyle, AvaloniaBrushCache>();
            _formattedTextCache = new Dictionary<TextShape, AvaloniaFormattedTextCache>();
            _matrixCache = new Dictionary<Matrix2, Matrix>();
            _cubicGeometryCache = new Dictionary<CubicBezierShape, Geometry>();
            _quadGeometryCache = new Dictionary<QuadraticBezierShape, Geometry>();
            _conicGeometryCache = new Dictionary<ConicShape, Geometry>();
            _pathGeometryCache = new Dictionary<PathShape, Geometry>();
            _ellipseGeometryCache = new Dictionary<EllipseShape, Geometry>();
        }

        private Matrix? GetMatrixCache(Matrix2 matrix)
        {
            if (matrix == null)
            {
                return null;
            }
            Matrix cache = default;
            if (matrix.IsDirty = true || !_matrixCache.TryGetValue(matrix, out cache))
            {
                _matrixCache[matrix] = AvaloniaHelper.ToMatrixTransform(matrix);
                return _matrixCache[matrix];
            }
            return cache;
        }

        private AvaloniaBrushCache? GetBrushCache(ShapeStyle style)
        {
            if (style == null)
            {
                return null;
            }
            AvaloniaBrushCache cache = default;
            if (style.IsDirty = true || !_brushCache.TryGetValue(style, out cache))
            {
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
                return _brushCache[style];
            }
            return cache;
        }

        private Geometry GetGeometryCache<T>(T shape, IDictionary<T, Geometry> cacheDictionary, ShapeStyle style, double dx, double dy) where T : BaseShape
        {
            if (shape == null)
            {
                return null;
            }
            Geometry cache = null;
            if (shape.IsDirty = true || !cacheDictionary.TryGetValue(shape, out cache))
            {
                shape.Invalidate();
                var geometry = AvaloniaHelper.ToGeometry(shape, style, dx, dy);
                if (geometry != null)
                {
                    cacheDictionary[shape] = geometry;
                    return cacheDictionary[shape];
                }
                return null;
            }
            return cache;
        }

        private AvaloniaFormattedTextCache GetTextCache(TextShape text, Rect rect, ShapeStyle style)
        {
            if (!_formattedTextCache.TryGetValue(text, out var cache))
            {
                _formattedTextCache[text] = AvaloniaFormattedTextCache.FromTextShape(text, rect, style.TextStyle);
                return _formattedTextCache[text];
            }
            return cache;
        }

        public object PushMatrix(object dc, Matrix2 matrix)
        {
            var _dc = dc as DrawingContext;
            return _dc.PushPreTransform(GetMatrixCache(matrix).Value);
        }

        public void PopMatrix(object dc, object state)
        {
            var _state = (DrawingContext.PushedState)state;
            _state.Dispose();
        }

        public void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, AvaloniaHelper.ToPoint(line.StartPoint, dx, dy), AvaloniaHelper.ToPoint(line.Point, dx, dy));
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(cubicBezier, _cubicGeometryCache, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(quadraticBezier, _quadGeometryCache, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawConic(object dc, ConicShape conic, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(conic, _conicGeometryCache, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(path, _pathGeometryCache, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = AvaloniaHelper.ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            if (style.IsFilled)
            {
                _dc.FillRectangle(cache?.Fill, rect);
            }
            if (style.IsStroked)
            {
                _dc.DrawRectangle(cache?.StrokePen, rect);
            }
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(ellipse, _ellipseGeometryCache, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = AvaloniaHelper.ToRect(text.TopLeft, text.BottomRight, dx, dy);
            if (text.Text != null)
            {
                var ftc = GetTextCache(text, rect, style);
                _dc.DrawText(cache?.Stroke, ftc.Origin, ftc.FormattedText);
            }
        }
    }
}
