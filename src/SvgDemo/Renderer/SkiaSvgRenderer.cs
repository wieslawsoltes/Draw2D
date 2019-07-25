using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using SkiaSharp;
using Svg;
using Svg.Document_Structure;
using Svg.Pathing;
using Svg.Transforms;

namespace SvgDemo
{
    public static class SkiaSvgRenderer
    {
        private static SKColor GetColor(SvgColourServer svgColourServer, float opacity, bool forStroke = false)
        {
            if (svgColourServer == SvgPaintServer.None)
            {
                return SKColors.Transparent;
            }

            if (svgColourServer == SvgColourServer.NotSet && forStroke)
            {
                return SKColors.Transparent;
            }

            var colour = svgColourServer.Colour;
            byte alpha = (byte)Math.Round((opacity * (svgColourServer.Colour.A / 255.0)) * 255);

            return new SKColor(colour.R, colour.G, colour.B, alpha);
        }

        private static string ToSvgPathData(SvgPathSegmentList svgPathSegmentList)
        {
            var sb = new StringBuilder();
            foreach (var svgSegment in svgPathSegmentList)
            {
                sb.AppendLine(svgSegment.ToString());
            }
            return sb.ToString();
        }

        private static SKPath ToSKPath(SvgPathSegmentList svgPathSegmentList, SvgFillRule svgFillRule)
        {
            var path = new SKPath()
            {
                FillType = (svgFillRule == SvgFillRule.EvenOdd) ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            foreach (var svgSegment in svgPathSegmentList)
            {
                switch (svgSegment)
                {
                    case SvgMoveToSegment svgMoveToSegment:
                        {
                            float x = (float)svgMoveToSegment.Start.X;
                            float y = (float)svgMoveToSegment.Start.Y;
                            path.MoveTo(x, y);
                        }
                        break;
                    case SvgLineSegment svgLineSegment:
                        {
                            float x = (float)svgLineSegment.End.X;
                            float y = (float)svgLineSegment.End.Y;
                            path.LineTo(x, y);
                        }
                        break;
                    case SvgCubicCurveSegment svgCubicCurveSegment:
                        {
                            float x0 = (float)svgCubicCurveSegment.FirstControlPoint.X;
                            float y0 = (float)svgCubicCurveSegment.FirstControlPoint.Y;
                            float x1 = (float)svgCubicCurveSegment.SecondControlPoint.X;
                            float y1 = (float)svgCubicCurveSegment.SecondControlPoint.Y;
                            float x2 = (float)svgCubicCurveSegment.End.X;
                            float y2 = (float)svgCubicCurveSegment.End.Y;
                            path.CubicTo(x0, y0, x1, y1, x2, y2);
                        }
                        break;
                    case SvgQuadraticCurveSegment svgQuadraticCurveSegment:
                        {
                            float x0 = (float)svgQuadraticCurveSegment.ControlPoint.X;
                            float y0 = (float)svgQuadraticCurveSegment.ControlPoint.Y;
                            float x1 = (float)svgQuadraticCurveSegment.End.X;
                            float y1 = (float)svgQuadraticCurveSegment.End.Y;
                            path.QuadTo(x0, y0, x1, y1);
                        }
                        break;
                    case SvgArcSegment svgArcSegment:
                        {
                            float rx = svgArcSegment.RadiusX;
                            float ry = svgArcSegment.RadiusY;
                            float xAxisRotate = (float)(svgArcSegment.Angle * Math.PI / 180.0);
                            var largeArc = svgArcSegment.Size == SvgArcSize.Small ? SKPathArcSize.Small : SKPathArcSize.Large;
                            var sweep = svgArcSegment.Sweep == SvgArcSweep.Negative ? SKPathDirection.CounterClockwise : SKPathDirection.Clockwise;
                            float x = (float)svgArcSegment.End.X;
                            float y = (float)svgArcSegment.End.Y;
                            path.ArcTo(rx, ry, xAxisRotate, largeArc, sweep, x, y);
                        }
                        break;
                    case SvgClosePathSegment svgClosePathSegment:
                        {
                            path.Close();
                        }
                        break;
                }
            }

            return path;
        }

        private static SKPath ToSKPath(SvgPointCollection svgPointCollection, SvgFillRule svgFillRule, bool isClosed)
        {
            var path = new SKPath()
            {
                FillType = (svgFillRule == SvgFillRule.EvenOdd) ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            var points = new SKPoint[svgPointCollection.Count / 2];

            for (int i = 0; (i + 1) < svgPointCollection.Count; i += 2)
            {
                float x = (float)svgPointCollection[i];
                float y = (float)svgPointCollection[i + 1];
                points[i / 2] = new SKPoint(x, y);
            }

            path.AddPoly(points, false);

            if (isClosed)
            {
                path.Close();
            }

            return path;
        }

        private static SKMatrix Multiply(ref SKMatrix value1, ref SKMatrix value2)
        {
            return new SKMatrix()
            {
                ScaleX = (value1.ScaleX * value2.ScaleX) + (value1.SkewY * value2.SkewX),
                SkewY = (value1.ScaleX * value2.SkewY) + (value1.SkewY * value2.ScaleY),
                SkewX = (value1.SkewX * value2.ScaleX) + (value1.ScaleY * value2.SkewX),
                ScaleY = (value1.SkewX * value2.SkewY) + (value1.ScaleY * value2.ScaleY),
                TransX = (value1.TransX * value2.ScaleX) + (value1.TransY * value2.SkewX) + value2.TransX,
                TransY = (value1.TransX * value2.SkewY) + (value1.TransY * value2.ScaleY) + value2.TransY,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        private static SKMatrix ToSKmatrix(Matrix matrix)
        {
            return new SKMatrix()
            {
                ScaleX = matrix.Elements[0],
                SkewY = matrix.Elements[1],
                SkewX = matrix.Elements[2],
                ScaleY = matrix.Elements[3],
                TransX = matrix.Elements[4],
                TransY = matrix.Elements[5],
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        private static SKPaint GetFillSKPaint(SvgElement svgElement)
        {
            var paint = new SKPaint()
            {
                IsAntialias = true
            };

            if (svgElement.Fill is SvgColourServer svgColourServer)
            {
                paint.Color = paint.Color = GetColor(svgColourServer, svgElement.StrokeOpacity, false);
                paint.Style = SKPaintStyle.Fill;
            }

            return paint;
        }

        private static SKPaint GetStrokeSKPaint(SvgElement svgElement)
        {
            var paint = new SKPaint()
            {
                IsAntialias = true
            };

            if (svgElement.Stroke is SvgColourServer svgColourServer)
            {
                paint.Color = paint.Color = GetColor(svgColourServer, svgElement.StrokeOpacity, true);
                paint.StrokeWidth = svgElement.StrokeWidth.ToDeviceValue(null, UnitRenderingType.Other, svgElement);
                paint.Style = SKPaintStyle.Stroke;
            }

            return paint;
        }

        private static void Transform(SKCanvas canvas, SvgViewBox viewBox, SvgAspectRatio aspectRatio, float x, float y, float width, float height)
        {
            if (viewBox.Equals(SvgViewBox.Empty))
            {
                canvas.Translate(x, y);
                return;
            }

            float fScaleX = width / viewBox.Width;
            float fScaleY = height / viewBox.Height;
            float fMinX = -viewBox.MinX * fScaleX;
            float fMinY = -viewBox.MinY * fScaleY;

            if (aspectRatio == null)
            {
                aspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid, false);
            }

            if (aspectRatio.Align != SvgPreserveAspectRatio.none)
            {
                if (aspectRatio.Slice)
                {
                    fScaleX = Math.Max(fScaleX, fScaleY);
                    fScaleY = Math.Max(fScaleX, fScaleY);
                }
                else
                {
                    fScaleX = Math.Min(fScaleX, fScaleY);
                    fScaleY = Math.Min(fScaleX, fScaleY);
                }
                float fViewMidX = (viewBox.Width / 2) * fScaleX;
                float fViewMidY = (viewBox.Height / 2) * fScaleY;
                float fMidX = width / 2;
                float fMidY = height / 2;
                fMinX = -viewBox.MinX * fScaleX;
                fMinY = -viewBox.MinY * fScaleY;

                switch (aspectRatio.Align)
                {
                    case SvgPreserveAspectRatio.xMinYMin:
                        break;
                    case SvgPreserveAspectRatio.xMidYMin:
                        fMinX += fMidX - fViewMidX;
                        break;
                    case SvgPreserveAspectRatio.xMaxYMin:
                        fMinX += width - viewBox.Width * fScaleX;
                        break;
                    case SvgPreserveAspectRatio.xMinYMid:
                        fMinY += fMidY - fViewMidY;
                        break;
                    case SvgPreserveAspectRatio.xMidYMid:
                        fMinX += fMidX - fViewMidX;
                        fMinY += fMidY - fViewMidY;
                        break;
                    case SvgPreserveAspectRatio.xMaxYMid:
                        fMinX += width - viewBox.Width * fScaleX;
                        fMinY += fMidY - fViewMidY;
                        break;
                    case SvgPreserveAspectRatio.xMinYMax:
                        fMinY += height - viewBox.Height * fScaleY;
                        break;
                    case SvgPreserveAspectRatio.xMidYMax:
                        fMinX += fMidX - fViewMidX;
                        fMinY += height - viewBox.Height * fScaleY;
                        break;
                    case SvgPreserveAspectRatio.xMaxYMax:
                        fMinX += width - viewBox.Width * fScaleX;
                        fMinY += height - viewBox.Height * fScaleY;
                        break;
                    default:
                        break;
                }
            }

            canvas.Translate(x, y);
            canvas.Translate(fMinX, fMinY);
            canvas.Scale(fScaleX, fScaleY);
        }

        private static void Transform(SKCanvas canvas, SvgTransformCollection transforms)
        {
            var totalMatrix = canvas.TotalMatrix;

            foreach (var svgTransform in transforms)
            {
                var matrix = ToSKmatrix(svgTransform.Matrix);
                totalMatrix = Multiply(ref totalMatrix, ref matrix);
            }

            canvas.SetMatrix(totalMatrix);
        }

        private static void DrawSymbol(SKCanvas canvas, SvgSymbol svgSymbol)
        {
            canvas.Save();

            Transform(canvas, svgSymbol.Transforms);

            float x = 0f;
            float y = 0f;
            float width = svgSymbol.ViewBox.Width;
            float height = svgSymbol.ViewBox.Height;

Console.WriteLine($"svgSymbol: {svgSymbol.ViewBox.Width} {svgSymbol.ViewBox.Height}");

            Transform(canvas, svgSymbol.ViewBox, svgSymbol.AspectRatio, x, y, width, height);

            Draw(canvas, svgSymbol.Children);

            canvas.Restore();
        }

        private static void Draw(SKCanvas canvas, SvgElement svgElement)
        {
            canvas.Save();

            Transform(canvas, svgElement.Transforms);

            switch (svgElement)
            {
                case SvgFragment svgFragment:
                    {
                        canvas.Save();

                        float x = svgFragment.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
                        float y = svgFragment.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
                        float width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
                        float height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);

                        Transform(canvas, svgFragment.ViewBox, svgFragment.AspectRatio, x, y, width, height);

                        Draw(canvas, svgFragment.Children);

                        canvas.Restore();
                    }
                    break;
                case SvgSymbol svgSymbol:
                    {
                    }
                    break;
                case SvgUse svgUse:
                    {
                        var svgVisualElement = svgUse.OwnerDocument.GetElementById(svgUse.ReferencedElement.ToString()) as SvgVisualElement;
                        if (svgVisualElement != null)
                        {
                            //var parent = svgUse.Parent;
                            //svgVisualElement.Parent = svgUse;
                            //svgVisualElement.InvalidateChildPaths();

                            canvas.Save();

                            float x = svgUse.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgUse);
                            float y = svgUse.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgUse);
                            canvas.Translate(x, y);

                            if (svgVisualElement is SvgSymbol)
                            {
                                DrawSymbol(canvas, svgVisualElement as SvgSymbol);
                            }
                            else
                            {
                                Draw(canvas, svgVisualElement);
                            }

                            //svgVisualElement.Parent = parent;

                            canvas.Restore();
                        }
                    }
                    break;
                case SvgCircle svgCircle:
                    {
                        float cx = svgCircle.CenterX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgCircle);
                        float cy = svgCircle.CenterY.ToDeviceValue(null, UnitRenderingType.Vertical, svgCircle);
                        float radius = svgCircle.Radius.ToDeviceValue(null, UnitRenderingType.Other, svgCircle);

                        if (svgCircle.Fill != null)
                        {
                            using (var paint = GetFillSKPaint(svgCircle))
                            {
                                canvas.DrawCircle(cx, cy, radius, paint);
                            }
                        }

                        if (svgCircle.Stroke != null)
                        {
                            using (var paint = GetStrokeSKPaint(svgCircle))
                            {
                                canvas.DrawCircle(cx, cy, radius, paint);
                            }
                        }
                    }
                    break;
                case SvgEllipse svgEllipse:
                    {
                        float cx = svgEllipse.CenterX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgEllipse);
                        float cy = svgEllipse.CenterY.ToDeviceValue(null, UnitRenderingType.Vertical, svgEllipse);
                        float rx = svgEllipse.RadiusX.ToDeviceValue(null, UnitRenderingType.Other, svgEllipse);
                        float ry = svgEllipse.RadiusY.ToDeviceValue(null, UnitRenderingType.Other, svgEllipse);

                        if (svgEllipse.Fill != null)
                        {
                            using (var paint = GetFillSKPaint(svgEllipse))
                            {
                                canvas.DrawOval(cx, cy, rx, ry, paint);
                            }
                        }

                        if (svgEllipse.Stroke != null)
                        {
                            using (var paint = GetStrokeSKPaint(svgEllipse))
                            {
                                canvas.DrawOval(cx, cy, rx, ry, paint);
                            }
                        }
                    }
                    break;
                case SvgRectangle svgRectangle:
                    {
                        float x = svgRectangle.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
                        float y = svgRectangle.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
                        float width = svgRectangle.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
                        float height = svgRectangle.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
                        float rx = svgRectangle.CornerRadiusX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
                        float ry = svgRectangle.CornerRadiusY.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
                        var rect = new SKRect(x, y, x + width, y + height);
                        bool isRound = rx > 0f && ry > 0f;

                        if (svgRectangle.Fill != null)
                        {
                            using (var paint = GetFillSKPaint(svgRectangle))
                            {
                                if (isRound)
                                {
                                    canvas.DrawRoundRect(rect, rx, ry, paint);
                                }
                                else
                                {
                                    canvas.DrawRect(rect, paint);
                                }
                            }
                        }

                        if (svgRectangle.Stroke != null)
                        {
                            using (var paint = GetStrokeSKPaint(svgRectangle))
                            {
                                if (isRound)
                                {
                                    canvas.DrawRoundRect(rect, rx, ry, paint);
                                }
                                else
                                {
                                    canvas.DrawRect(rect, paint);
                                }
                            }
                        }
                    }
                    break;
                case SvgGroup svgGroup:
                    {
                        Draw(canvas, svgGroup.Children);
                    }
                    break;
                case SvgLine svgLine:
                    {
                        float x0 = svgLine.StartX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLine);
                        float y0 = svgLine.StartY.ToDeviceValue(null, UnitRenderingType.Vertical, svgLine);
                        float x1 = svgLine.EndX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLine);
                        float y1 = svgLine.EndY.ToDeviceValue(null, UnitRenderingType.Vertical, svgLine);

                        if (svgLine.Stroke != null)
                        {
                            if (svgLine.Stroke is SvgColourServer svgColourServer)
                            {
                                using (var paint = GetStrokeSKPaint(svgLine))
                                {
                                    canvas.DrawLine(x0, y0, x1, y1, paint);
                                }
                            }
                        }
                    }
                    break;
                case SvgPath svgPath:
                    {
                        using (var path = ToSKPath(svgPath.PathData, svgPath.FillRule))
                        {
                            if (path == null || path.IsEmpty)
                            {
                                break;
                            }

                            if (svgPath.Fill != null)
                            {
                                using (var paint = GetFillSKPaint(svgPath))
                                {
                                    canvas.DrawPath(path, paint);
                                }
                            }

                            if (svgPath.Stroke != null)
                            {
                                using (var paint = GetStrokeSKPaint(svgPath))
                                {
                                    canvas.DrawPath(path, paint);
                                }
                            }
                        }
                    }
                    break;
                case SvgPolyline svgPolyline:
                    {
                        using (var path = ToSKPath(svgPolyline.Points, svgPolyline.FillRule, false))
                        {
                            if (path == null || path.IsEmpty)
                            {
                                break;
                            }

                            if (svgPolyline.Fill != null)
                            {
                                using (var paint = GetFillSKPaint(svgPolyline))
                                {
                                    canvas.DrawPath(path, paint);
                                }
                            }

                            if (svgPolyline.Stroke != null)
                            {
                                using (var paint = GetStrokeSKPaint(svgPolyline))
                                {
                                    canvas.DrawPath(path, paint);
                                }
                            }
                        }
                    }
                    break;
                case SvgPolygon svgPolygon:
                    {
                        using (var path = ToSKPath(svgPolygon.Points, svgPolygon.FillRule, true))
                        {
                            if (path == null || path.IsEmpty)
                            {
                                break;
                            }

                            if (svgPolygon.Fill != null)
                            {
                                using (var paint = GetFillSKPaint(svgPolygon))
                                {
                                    canvas.DrawPath(path, paint);
                                }
                            }

                            if (svgPolygon.Stroke != null)
                            {
                                using (var paint = GetStrokeSKPaint(svgPolygon))
                                {
                                    canvas.DrawPath(path, paint);
                                }
                            }
                        }
                    }
                    break;
            }

            canvas.Restore();
        }

        private static void Draw(SKCanvas canvas, SvgElementCollection svgElementCollection)
        {
            foreach (var svgElement in svgElementCollection)
            {
                Draw(canvas, svgElement);
            }
        }

        public static void SaveImage(string path, SvgElement svgElement, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            if (svgElement is SvgFragment svgFragment)
            {
                float width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
                float height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);

                var info = new SKImageInfo((int)width, (int)height);
                using (var bitmap = new SKBitmap(info))
                {
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear(SKColors.Transparent);

                        Draw(canvas, svgFragment);

                        using (var image = SKImage.FromBitmap(bitmap))
                        using (var data = image.Encode(format, quality))
                        {
                            if (data != null)
                            {
                                using (var stream = File.OpenWrite(path))
                                {
                                    data.SaveTo(stream);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
