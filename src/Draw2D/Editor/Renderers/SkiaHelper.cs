// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define USE_CLOSE_SHAPE
using System;
using System.Collections.ObjectModel;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Editor.Renderers
{
    internal class SkiaHelper
    {
        public static SKTypeface ToSKTypeface(string familyName)
        {
            return SKTypeface.FromFamilyName(
                familyName,
                SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright);
        }

        public static SKColor ToSKColor(ArgbColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static SKPaint ToSKPaintPen(ShapeStyle style, double scale)
        {
            return new SKPaint()
            {
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = (float)(style.Thickness / scale),
                Color = ToSKColor(style.Stroke),
                //Shader = SKShader.CreateColor(ToSKColor(style.Stroke)),
                StrokeCap = SKStrokeCap.Butt,
                PathEffect = null
            };
        }

        public static SKPaint ToSKPaintBrush(ArgbColor color)
        {
            return new SKPaint()
            {
                IsAntialias = true,
                IsStroke = false,
                LcdRenderText = true,
                SubpixelText = true,
                Color = ToSKColor(color),
                //Shader = SKShader.CreateColor(ToSKColor(color)),
                TextAlign = SKTextAlign.Left
            };
        }

        public static void ToSKPaintPenUpdate(SKPaint paint, ShapeStyle style, double scale)
        {
            paint.StrokeWidth = (float)(style.Thickness / scale);
            //paint.Color = ToSKColor(style.Stroke);
            //paint.Shader = SKShader.CreateColor(ToSKColor(style.Stroke));
        }

        public static void ToSKPaintBrushUpdate(SKPaint paint, ArgbColor color)
        {
            //paint.Color = ToSKColor(color);
            //paint.Shader = SKShader.CreateColor(ToSKColor(color));
        }

        public static SKPoint ToPoint(PointShape point, double dx, double dy)
        {
            return new SKPoint((float)(point.X + dx), (float)(point.Y + dy));
        }

        public static SKRect ToRect(double left, double top, double right, double bottom)
        {
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        public static SKRect ToRect(PointShape p1, PointShape p2, double dx, double dy)
        {
            double left = Math.Min(p1.X + dx, p2.X + dx);
            double top = Math.Min(p1.Y + dy, p2.Y + dy);
            double right = left + Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - left);
            double bottom = top + Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - top);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        public static SKMatrix Multiply(SKMatrix value1, SKMatrix value2)
        {
            return ToSKMatrix(
                (value1.ScaleX * value2.ScaleX) + (value1.SkewY * value2.SkewX),
                (value1.ScaleX * value2.SkewY) + (value1.SkewY * value2.ScaleY),
                (value1.SkewX * value2.ScaleX) + (value1.ScaleY * value2.SkewX),
                (value1.SkewX * value2.SkewY) + (value1.ScaleY * value2.ScaleY),
                (value1.TransX * value2.ScaleX) + (value1.TransY * value2.SkewX) + value2.TransX,
                (value1.TransX * value2.SkewY) + (value1.TransY * value2.ScaleY) + value2.TransY);
        }

        public static SKMatrix ToSKMatrix(double m11, double m12, double m21, double m22, double m31, double m32)
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

        public static SKPathFillType ToFillType(PathFillRule fillRule)
        {
            return fillRule == PathFillRule.EvenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
        }

        public static SKPath ToGeometry(LineShape line, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(line.StartPoint, dx, dy));
            geometry.LineTo(ToPoint(line.Point, dx, dy));
            return geometry;
        }

        public static SKPath ToGeometry(CubicBezierShape cubicBezier, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(cubicBezier.StartPoint, dx, dy));
            geometry.CubicTo(
                ToPoint(cubicBezier.Point1, dx, dy),
                ToPoint(cubicBezier.Point2, dx, dy),
                ToPoint(cubicBezier.Point3, dx, dy));
            return geometry;
        }

        public static SKPath ToGeometry(QuadraticBezierShape quadraticBezier, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(quadraticBezier.StartPoint, dx, dy));
            geometry.QuadTo(
                ToPoint(quadraticBezier.Point1, dx, dy),
                ToPoint(quadraticBezier.Point2, dx, dy));
            return geometry;
        }

        public static SKPath ToGeometry(ConicShape conic, double dx, double dy)
        {
            var geometry = new SKPath();
            geometry.MoveTo(ToPoint(conic.StartPoint, dx, dy));
            geometry.ConicTo(
                ToPoint(conic.Point1, dx, dy),
                ToPoint(conic.Point2, dx, dy),
                (float)conic.Weight);
            return geometry;
        }

        public static void ToGeometry(FigureShape figure, double dx, double dy, SKPath geometry)
        {
            bool isFirstShape = true;

            foreach (var shape in figure.Shapes)
            {
                if (shape is LineShape line)
                {
                    if (isFirstShape)
                    {
                        geometry.MoveTo(ToPoint(line.StartPoint, dx, dy));
                        isFirstShape = false;
                    }
                    geometry.LineTo(ToPoint(line.Point, dx, dy));
                }
                else if (shape is CubicBezierShape cubicBezier)
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
                else if (shape is QuadraticBezierShape quadraticBezier)
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
                else if (shape is ConicShape conic)
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
            }

            if (!isFirstShape && figure.IsClosed)
            {
                geometry.Close();
            }
        }

        public static SKPath ToGeometry(PathShape path, double dx, double dy)
        {
            var geometry = new SKPath
            {
                FillType = ToFillType(path.FillRule)
            };

            foreach (var figure in path.Figures)
            {
                ToGeometry(figure, dx, dy, geometry);
            }

            return geometry;
        }

        public static SKPath ToGeometry(RectangleShape rectangle, double dx, double dy)
        {
            var rect = SkiaHelper.ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            var geometry = new SKPath();
            geometry.AddRect(rect, SKPathDirection.Clockwise);
            return geometry;
        }

        public static SKPath ToGeometry(EllipseShape ellipse, double dx, double dy)
        {
            var rect = SkiaHelper.ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            var geometry = new SKPath();
            geometry.AddOval(rect, SKPathDirection.Clockwise);
            return geometry;
        }

        public static SKPath ToGeometry(Text text, PointShape topLeft, PointShape bottomRight, ShapeStyle style, double dx, double dy)
        {
            using (var typeface = SkiaHelper.ToSKTypeface(style.TextStyle.FontFamily))
            using (var paint = SkiaHelper.ToSKPaintBrush(style.TextStyle.Stroke))
            {
                paint.Typeface = typeface;
                paint.TextEncoding = SKTextEncoding.Utf16;
                paint.TextSize = (float)style.TextStyle.FontSize;
                var outlineGeometry = paint.GetTextPath(text.Value, 0.0f, 0.0f);
                var fillGeometry = paint.GetFillPath(outlineGeometry);
                return fillGeometry;
            }
        }

        public static SKPath ToGeometry(TextShape text, double dx, double dy)
        {
            return ToGeometry(text.Text, text.TopLeft, text.BottomRight, text.Style, dx, dy);
        }

        public static SKPath ToGeometry(string svgPathData)
        {
            return SKPath.ParseSvgPathData(svgPathData);
        }

        public static PathShape FromGeometry(SKPath path, ShapeStyle style, BaseShape pointTemplate)
        {
            var pathShape = new PathShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Figures = new ObservableCollection<FigureShape>(),
                FillRule = PathFillRule.EvenOdd,
                Text = new Text(),
                Style = style
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
                                    Shapes = new ObservableCollection<BaseShape>(),
                                    IsFilled = true,
                                    IsClosed = false
                                };
                                pathShape.Figures.Add(figureShape);
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
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Style = style
                                };
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
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Point3 = new PointShape(points[3].X, points[3].Y, pointTemplate),
                                    Style = style
                                };
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
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Style = style
                                };
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
                                var quadraticBezierShape = new ConicShape()
                                {
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, pointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, pointTemplate),
                                    Weight = iterator.ConicWeight(),
                                    Style = style
                                };
                                figureShape.Shapes.Add(quadraticBezierShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Close:
                            {
#if USE_CLOSE_SHAPE
                                var line = new LineShape()
                                {
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = pathShape.GetLastPoint(),
                                    Point = pathShape.GetFirstPoint(),
                                    Style = style
                                };
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
    }
}
