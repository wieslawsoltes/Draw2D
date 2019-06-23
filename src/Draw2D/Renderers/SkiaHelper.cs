// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Renderers
{
    internal class SkiaHelper
    {
        internal static ShapeStyle GetShapeStyle(IToolContext context, string styleId)
        {
            if (context?.StyleLibrary?.Styles != null)
            {
                foreach (var style in context?.StyleLibrary.Styles)
                {
                    if (style.Title == styleId)
                    {
                        return style;
                    }
                }
            }
            return null;
        }

        internal static IList<IBaseShape> GetShapes(ICollection<IBaseShape> selected)
        {
            if (selected == null || selected.Count <= 0)
            {
                return null;
            }

            var shapes = new List<IBaseShape>();

            foreach (var shape in selected)
            {
                if (!(shape is IPointShape))
                {
                    shapes.Add(shape);
                }
            }

            return shapes;
        }

        internal static SKTypeface ToSKTypeface(string familyName)
        {
            return SKTypeface.FromFamilyName(
                familyName,
                SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright);
        }

        internal static SKColor ToSKColor(ArgbColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        internal static SKPaint ToSKPaintPen(ShapeStyle style, double scale)
        {
            return new SKPaint()
            {
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = (float)(style.Thickness / scale),
                Color = ToSKColor(style.Stroke),
                //Shader = SKShader.CreateColor(ToSKColor(style.Stroke)),
                StrokeCap = SKStrokeCap.Butt,
                PathEffect = null,
                Style = SKPaintStyle.Stroke
            };
        }

        internal static SKPaint ToSKPaintBrush(ArgbColor color)
        {
            return new SKPaint()
            {
                IsAntialias = true,
                IsStroke = false,
                LcdRenderText = true,
                SubpixelText = true,
                Color = ToSKColor(color),
                //Shader = SKShader.CreateColor(ToSKColor(color)),
                TextAlign = SKTextAlign.Left,
                Style = SKPaintStyle.Fill
            };
        }

        internal static void ToSKPaintPenUpdate(SKPaint paint, ShapeStyle style, double scale)
        {
            paint.StrokeWidth = (float)(style.Thickness / scale);
            //paint.Color = ToSKColor(style.Stroke);
            //paint.Shader = SKShader.CreateColor(ToSKColor(style.Stroke));
        }

        internal static void ToSKPaintBrushUpdate(SKPaint paint, ArgbColor color)
        {
            //paint.Color = ToSKColor(color);
            //paint.Shader = SKShader.CreateColor(ToSKColor(color));
        }

        internal static SKPoint ToPoint(IPointShape point, double dx, double dy)
        {
            return new SKPoint((float)(point.X + dx), (float)(point.Y + dy));
        }

        internal static SKRect ToRect(double left, double top, double right, double bottom)
        {
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        internal static SKRect ToRect(IPointShape p1, IPointShape p2, double dx, double dy)
        {
            double left = Math.Min(p1.X + dx, p2.X + dx);
            double top = Math.Min(p1.Y + dy, p2.Y + dy);
            double right = left + Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - left);
            double bottom = top + Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - top);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        internal static SKMatrix Multiply(SKMatrix value1, SKMatrix value2)
        {
            return ToSKMatrix(
                (value1.ScaleX * value2.ScaleX) + (value1.SkewY * value2.SkewX),
                (value1.ScaleX * value2.SkewY) + (value1.SkewY * value2.ScaleY),
                (value1.SkewX * value2.ScaleX) + (value1.ScaleY * value2.SkewX),
                (value1.SkewX * value2.SkewY) + (value1.ScaleY * value2.ScaleY),
                (value1.TransX * value2.ScaleX) + (value1.TransY * value2.SkewX) + value2.TransX,
                (value1.TransX * value2.SkewY) + (value1.TransY * value2.ScaleY) + value2.TransY);
        }

        internal static SKMatrix ToSKMatrix(double m11, double m12, double m21, double m22, double m31, double m32)
        {
            return new SKMatrix
            {
                ScaleX = (float)m11,
                SkewX = (float)m21,
                TransX = (float)m31,
                SkewY = (float)m12,
                ScaleY = (float)m22,
                TransY = (float)m32,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        internal static SKPathFillType ToFillType(PathFillRule fillRule)
        {
            return fillRule == PathFillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
        }

        internal static SKPathOp ToPathOp(PathOp op)
        {
            switch (op)
            {
                default:
                case PathOp.Difference:
                    return SKPathOp.Difference;
                case PathOp.Intersect:
                    return SKPathOp.Intersect;
                case PathOp.Union:
                    return SKPathOp.Union;
                case PathOp.Xor:
                    return SKPathOp.Xor;
                case PathOp.ReverseDifference:
                    return SKPathOp.ReverseDifference;
            }
        }

        internal static void ToGeometry(LineShape line, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToPoint(line.StartPoint, dx, dy));
            geometry.LineTo(ToPoint(line.Point, dx, dy));
        }

        internal static void ToGeometry(CubicBezierShape cubicBezier, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToPoint(cubicBezier.StartPoint, dx, dy));
            geometry.CubicTo(
                ToPoint(cubicBezier.Point1, dx, dy),
                ToPoint(cubicBezier.Point2, dx, dy),
                ToPoint(cubicBezier.Point3, dx, dy));
        }

        internal static void ToGeometry(QuadraticBezierShape quadraticBezier, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToPoint(quadraticBezier.StartPoint, dx, dy));
            geometry.QuadTo(
                ToPoint(quadraticBezier.Point1, dx, dy),
                ToPoint(quadraticBezier.Point2, dx, dy));
        }

        internal static void ToGeometry(ConicShape conic, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToPoint(conic.StartPoint, dx, dy));
            geometry.ConicTo(
                ToPoint(conic.Point1, dx, dy),
                ToPoint(conic.Point2, dx, dy),
                (float)conic.Weight);
        }

        internal static void ToGeometry(RectangleShape rectangle, double dx, double dy, SKPath geometry)
        {
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            geometry.AddRect(rect, SKPathDirection.Clockwise);
        }

        internal static void ToGeometry(EllipseShape ellipse, double dx, double dy, SKPath geometry)
        {
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            geometry.AddOval(rect, SKPathDirection.Clockwise);
        }

        internal static void ToGeometry(Text text, IPointShape topLeft, IPointShape bottomRight, TextStyle style, double dx, double dy, SKPath geometry)
        {
            using (var typeface = ToSKTypeface(style.FontFamily))
            using (var paint = ToSKPaintBrush(style.Stroke))
            {
                paint.Typeface = typeface;
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.TextSize = (float)style.FontSize;

                switch (style.HAlign)
                {
                    default:
                    case HAlign.Left:
                        paint.TextAlign = SKTextAlign.Left;
                        break;
                    case HAlign.Center:
                        paint.TextAlign = SKTextAlign.Center;
                        break;
                    case HAlign.Right:
                        paint.TextAlign = SKTextAlign.Right;
                        break;
                }

                var metrics = paint.FontMetrics;
                var mAscent = metrics.Ascent;
                var mDescent = metrics.Descent;
                var rect = ToRect(topLeft, bottomRight, dx, dy);
                float x = rect.Left;
                float y = rect.Top;
                float width = rect.Width;
                float height = rect.Height;

                switch (style.VAlign)
                {
                    default:
                    case VAlign.Top:
                        y -= mAscent;
                        break;
                    case VAlign.Center:
                        y += (height / 2.0f) - (mAscent / 2.0f) - mDescent / 2.0f;
                        break;
                    case VAlign.Bottom:
                        y += height - mDescent;
                        break;
                }

                switch (style.HAlign)
                {
                    default:
                    case HAlign.Left:
                        // x = x;
                        break;
                    case HAlign.Center:
                        x += width / 2.0f;
                        break;
                    case HAlign.Right:
                        x += width;
                        break;
                }

                using (var outlineGeometry = paint.GetTextPath(text.Value, x, y))
                using (var fillGeometry = paint.GetFillPath(outlineGeometry))
                {
                    geometry.AddPath(fillGeometry, SKPathAddMode.Append);
                }
            }
        }

        internal static void ToGeometry(TextShape text, double dx, double dy, SKPath geometry, IToolContext context)
        {
            var style = GetShapeStyle(context, text.StyleId);
            if (style != null)
            {
                ToGeometry(text.Text, text.TopLeft, text.BottomRight, style.TextStyle, dx, dy, geometry);
            }
        }

        internal static void ToFigureGeometry(IList<IBaseShape> shapes, bool isClosed, double dx, double dy, SKPath geometry)
        {
            bool isFirstShape = true;

            foreach (var shape in shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToPoint(line.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.LineTo(ToPoint(line.Point, dx, dy));
                        }
                        break;
                    case CubicBezierShape cubicBezier:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToPoint(cubicBezier.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.CubicTo(
                                ToPoint(cubicBezier.Point1, dx, dy),
                                ToPoint(cubicBezier.Point2, dx, dy),
                                ToPoint(cubicBezier.Point3, dx, dy));
                        }
                        break;
                    case QuadraticBezierShape quadraticBezier:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToPoint(quadraticBezier.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.QuadTo(
                                ToPoint(quadraticBezier.Point1, dx, dy),
                                ToPoint(quadraticBezier.Point2, dx, dy));
                        }
                        break;
                    case ConicShape conic:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToPoint(conic.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.ConicTo(
                                ToPoint(conic.Point1, dx, dy),
                                ToPoint(conic.Point2, dx, dy),
                                (float)conic.Weight);
                        }
                        break;
                }
            }

            if (!isFirstShape && isClosed)
            {
                geometry.Close();
            }
        }

        internal static void ToGeometry(GroupShape group, double dx, double dy, SKPath geometry)
        {
            ToFigureGeometry(group.Shapes, false, dx, dy, geometry);
        }

        internal static void ToGeometry(ReferenceShape reference, double dx, double dy, SKPath geometry)
        {
            ToGeometry(reference.Template, dx + reference.X, dy + reference.Y, geometry);
        }

        internal static void ToGeometry(FigureShape figure, double dx, double dy, SKPath geometry)
        {
            ToFigureGeometry(figure.Shapes, figure.IsClosed, dx, dy, geometry);
        }

        internal static void ToGeometry(PathShape path, double dx, double dy, SKPath geometry)
        {
            geometry.FillType = ToFillType(path.FillRule);

            foreach (var shape in path.Shapes)
            {
                if (shape is FigureShape figure)
                {
                    ToGeometry(figure, dx, dy, geometry);
                }
            }
        }

        internal static bool ToGeometry(IBaseShape shape, double dx, double dy, SKPath geometry)
        {
            switch (shape)
            {
                case LineShape line:
                    ToGeometry(line, dx, dy, geometry);
                    return true;
                case CubicBezierShape cubicBezier:
                    ToGeometry(cubicBezier, dx, dy, geometry);
                    return true;
                case QuadraticBezierShape quadraticBezier:
                    ToGeometry(quadraticBezier, dx, dy, geometry);
                    return true;
                case ConicShape conic:
                    ToGeometry(conic, dx, dy, geometry);
                    return true;
                case FigureShape figure:
                    ToGeometry(figure, dx, dy, geometry);
                    return true;
                case PathShape path:
                    ToGeometry(path, dx, dy, geometry);
                    return true;
                case RectangleShape rectangle:
                    ToGeometry(rectangle, dx, dy, geometry);
                    return true;
                case EllipseShape ellipse:
                    ToGeometry(ellipse, dx, dy, geometry);
                    return true;
                case IPointShape point:
                    ToGeometry(point, dx, dy, geometry);
                    return true;
                case TextShape text:
                    ToGeometry(text, dx, dy, geometry);
                    return true;
                case GroupShape group:
                    ToGeometry(group, dx, dy, geometry);
                    return true;
                case ReferenceShape reference:
                    ToGeometry(reference, dx, dy, geometry);
                    return true;
            };
            return false;
        }

        internal static SKPath ToGeometry(string svgPathData)
        {
            return SKPath.ParseSvgPathData(svgPathData);
        }

        internal static bool ToGeometry(IToolContext context, IBaseShape shape, SKPath geometry)
        {
            switch (shape)
            {
                case LineShape line:
                    ToGeometry(line, 0.0, 0.0, geometry);
                    return true;
                case CubicBezierShape cubicBezier:
                    ToGeometry(cubicBezier, 0.0, 0.0, geometry);
                    return true;
                case QuadraticBezierShape quadraticBezier:
                    ToGeometry(quadraticBezier, 0.0, 0.0, geometry);
                    return true;
                case ConicShape conic:
                    ToGeometry(conic, 0.0, 0.0, geometry);
                    return true;
                case PathShape pathShape:
                    ToGeometry(pathShape, 0.0, 0.0, geometry);
                    return true;
                case RectangleShape rectangle:
                    ToGeometry(rectangle, 0.0, 0.0, geometry);
                    return true;
                case EllipseShape ellipse:
                    ToGeometry(ellipse, 0.0, 0.0, geometry);
                    return true;
                case TextShape text:
                    ToGeometry(text, 0.0, 0.0, geometry);
                    break;
            };
            return false;
        }

        internal static IList<SKPath> ToGeometries(IToolContext context, IList<IBaseShape> shapes)
        {
            if (shapes == null || shapes.Count <= 0)
            {
                return null;
            }

            var paths = new List<SKPath>();

            for (int i = 0; i < shapes.Count; i++)
            {
                var path = new SKPath();
                var result = ToGeometry(context, shapes[i], path);
                if (result == true && path.IsEmpty == false)
                {
                    paths.Add(path);
                }
                else
                {
                    path.Dispose();
                }
            }

            return paths;
        }

        internal static SKPath Op(SKPathOp op, IList<SKPath> paths)
        {
            if (paths == null || paths.Count <= 0)
            {
                return null;
            }

            if (paths.Count == 1)
            {
                using (var empty = new SKPath())
                {
                    return empty.Op(paths[0], op);
                }
            }
            else
            {
                var haveResult = false;
                var result = new SKPath(paths[0]);

                for (int i = 1; i < paths.Count; i++)
                {
                    var next = result.Op(paths[i], op);
                    if (next != null)
                    {
                        result.Dispose();
                        result = next;
                        haveResult = true;
                    }
                }

                return haveResult ? result : null;
            }
        }

        internal static PathShape ToPathShape(SKPath path, ShapeStyle style, IBaseShape pointTemplate)
        {
            var pathShape = new PathShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                FillRule = PathFillRule.EvenOdd,
                Text = new Text(),
                StyleId = style.Title
            };

            var figureShape = default(FigureShape);

            using (var iterator = path.CreateRawIterator())
            {
                var points = new SKPoint[4];
                var pathVerb = SKPathVerb.Move;
                var firstPoint = new SKPoint();
                var lastPoint = new SKPoint();

                while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (pathVerb)
                    {
                        case SKPathVerb.Move:
                            {
                                figureShape = new FigureShape()
                                {
                                    Shapes = new ObservableCollection<IBaseShape>(),
                                    IsFilled = true,
                                    IsClosed = false
                                };
                                pathShape.Shapes.Add(figureShape);
                                firstPoint = lastPoint = points[0];
                            }
                            break;
                        case SKPathVerb.Line:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var lineShape = new LineShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                lineShape.StartPoint.Owner = lineShape;
                                lineShape.Point.Owner = lineShape;
                                figureShape.Shapes.Add(lineShape);
                                lastPoint = points[1];
                            }
                            break;
                        case SKPathVerb.Cubic:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var cubicBezierShape = new CubicBezierShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Point3 = new PointShape(points[3].X, points[3].Y, pointTemplate),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                cubicBezierShape.StartPoint.Owner = cubicBezierShape;
                                cubicBezierShape.Point1.Owner = cubicBezierShape;
                                cubicBezierShape.Point2.Owner = cubicBezierShape;
                                cubicBezierShape.Point3.Owner = cubicBezierShape;
                                figureShape.Shapes.Add(cubicBezierShape);
                                lastPoint = points[3];
                            }
                            break;
                        case SKPathVerb.Quad:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var quadraticBezierShape = new QuadraticBezierShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                quadraticBezierShape.StartPoint.Owner = quadraticBezierShape;
                                quadraticBezierShape.Point1.Owner = quadraticBezierShape;
                                quadraticBezierShape.Point2.Owner = quadraticBezierShape;
                                figureShape.Shapes.Add(quadraticBezierShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Conic:
                            {
                                var lastPointShape = pathShape.GetLastPoint();
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, pointTemplate);
                                }
                                var conicShape = new ConicShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Weight = iterator.ConicWeight(),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                conicShape.StartPoint.Owner = conicShape;
                                conicShape.Point1.Owner = conicShape;
                                conicShape.Point2.Owner = conicShape;
                                figureShape.Shapes.Add(conicShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Close:
                            {
#if USE_CLOSE_SHAPE
                                var line = new LineShape()
                                {
                                    Points = new ObservableCollection<IPointShape>(),
                                    StartPoint = pathShape.GetLastPoint(),
                                    Point = pathShape.GetFirstPoint(),
                                    Text = new Text(),
                                    StyleId = style.Title
                                };
                                line.StartPoint.Owner = line;
                                line.Point.Owner = line;
                                figureShape.Shapes.Add(line);
#else
                                figureShape.IsClosed = true;
                                firstPoint = lastPoint = new SKPoint(0, 0);
#endif
                            }
                            break;
                    }
                }
            }

            return pathShape;
        }

        internal static void ToSvgPathData(Text text, IPointShape topLeft, IPointShape bottomRight, TextStyle style, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(text?.Value))
            {
                using (var geometry = new SKPath())
                {
                    ToGeometry(text, topLeft, bottomRight, style, 0.0, 0.0, geometry);
                    sb.AppendLine(geometry.ToSvgPathData());
                }
            }
        }

        internal static void ToSvgPathData(IToolContext context, IBaseShape shape, StringBuilder sb)
        {
            switch (shape)
            {
                case LineShape line:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(line, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case CubicBezierShape cubicBezier:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(cubicBezier, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case QuadraticBezierShape quadraticBezier:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(quadraticBezier, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case ConicShape conic:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(conic, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case PathShape pathShape:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(pathShape, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case RectangleShape rectangle:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(rectangle, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = GetShapeStyle(context, rectangle.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(rectangle.Text, rectangle.TopLeft, rectangle.BottomRight, style.TextStyle, sb);
                            }
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        using (var geometry = new SKPath())
                        {
                            ToGeometry(ellipse, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = GetShapeStyle(context, ellipse.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(ellipse.Text, ellipse.TopLeft, ellipse.BottomRight, style.TextStyle, sb);
                            }
                        }
                    }
                    break;
#if USE_SVG_POINT
                case IPointShape point:
                    {
                        if (point.Template != null)
                        {
                            ToSvgPathData(context, point.Template, sb);
                        }
                    }
                    break;
#endif
                case GroupShape group:
                    {
                        foreach (var groupShape in group.Shapes)
                        {
                            ToSvgPathData(context, groupShape, sb);
                        }
                    }
                    break;
                case TextShape text:
                    {
                        var style = GetShapeStyle(context, text.StyleId);
                        if (style != null)
                        {
                            ToSvgPathData(text.Text, text.TopLeft, text.BottomRight, style.TextStyle, sb);
                        }
                    }
                    break;
            };
        }

        public static PathShape ToPathShape(IToolContext context, IBaseShape shape)
        {
            using (var geometry = new SKPath())
            {
                if (ToGeometry(shape, 0.0, 0.0, geometry) == true)
                {
                    return ToPathShape(geometry, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                }
            }
            return null;
        }

        public static PathShape Op(IToolContext context, PathOp op, ICollection<IBaseShape> selected)
        {
            var path = default(PathShape);
            var shapes = GetShapes(selected);
            if (shapes != null && shapes.Count > 0)
            {
                var paths = ToGeometries(context, shapes);
                if (paths != null && paths.Count > 0)
                {
                    var result = Op(ToPathOp(op), paths);
                    if (result != null)
                    {
                        if (!result.IsEmpty)
                        {
                            path = ToPathShape(result, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                        }
                        result.Dispose();
                    }

                    for (int i = 0; i < paths.Count; i++)
                    {
                        paths[i].Dispose();
                    }
                }
            }
            return path;
        }

        public static PathShape ToPathShape(IToolContext context, string svgPathData)
        {
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                var path = ToGeometry(svgPathData);
                return ToPathShape(path, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
            }
            return null;
        }

        public static string ToSvgPathData(IToolContext context, ICollection<IBaseShape> selected)
        {
            var sb = new StringBuilder();

            foreach (var shape in selected)
            {
                ToSvgPathData(context, shape, sb);
            }

            return sb.ToString();
        }
    }
}
