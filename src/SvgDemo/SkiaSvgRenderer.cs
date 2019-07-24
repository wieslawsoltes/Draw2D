using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using SkiaSharp;
using Svg;
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

        private static SKPath ToSKPath(SvgPathSegmentList svgPathSegmentList)
        {
            var sb = new StringBuilder();

            foreach (var svgSegment in svgPathSegmentList)
            {
                sb.AppendLine(svgSegment.ToString());
            }

            var pathData = sb.ToString();

            return SKPath.ParseSvgPathData(pathData);
        }

        private static SKPath ToSKPath(SvgPointCollection svgPointCollection)
        {
            var path = new SKPath();
            var points = new SKPoint[svgPointCollection.Count / 2];

            for (int i = 0; (i + 1) < svgPointCollection.Count; i += 2)
            {
                float x = (float)svgPointCollection[i];
                float y = (float)svgPointCollection[i + 1];
                points[i / 2] = new SKPoint(x, y);
            }

            path.AddPoly(points, false);

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
        
        private static int Transform(SKCanvas canvas, SvgTransformCollection transforms)
        {
            int count = canvas.Save();

            var totalMatrix = canvas.TotalMatrix;

            foreach (var svgTransform in transforms)
            {
                var matrix = ToSKmatrix(svgTransform.Matrix);
                totalMatrix = Multiply(ref totalMatrix, ref matrix);
            }

            canvas.SetMatrix(totalMatrix);

            return count;
        }

        private static SKPaint GetFillSKPaint(SvgElement svgElement)
        {
            var paint = new SKPaint();

            if (svgElement.Fill is SvgColourServer svgColourServer)
            {
                paint.Color = paint.Color = GetColor(svgColourServer, svgElement.StrokeOpacity, false);
                paint.Style = SKPaintStyle.Fill;
            }

            return paint;
        }

        private static SKPaint GetStrokeSKPaint(SvgElement svgElement)
        {
            var paint = new SKPaint();

            if (svgElement.Stroke is SvgColourServer svgColourServer)
            {
                paint.Color = paint.Color = GetColor(svgColourServer, svgElement.StrokeOpacity, true);
                paint.StrokeWidth = svgElement.StrokeWidth;
                paint.Style = SKPaintStyle.Stroke;
            }

            return paint;
        }

        private static void Draw(SKCanvas canvas, Element element)
        {
            int count = Transform(canvas, element.Original.Transforms);

            switch (element.Original)
            {
                case SvgFragment svgFragment:
                    {
                        Draw(canvas, element.Children);
                    }
                    break;
                case SvgCircle svgCircle:
                    {
                        float cx = svgCircle.CenterX;
                        float cy = svgCircle.CenterY;
                        float radius = svgCircle.Radius;

                        if (svgCircle.Fill != null && svgCircle.Fill != SvgColourServer.NotSet)
                        {
                            var paint = GetFillSKPaint(svgCircle);

                            canvas.DrawCircle(cx, cy, radius, paint);
                        }

                        if (svgCircle.Stroke != null && svgCircle.Stroke != SvgColourServer.NotSet)
                        {
                            var paint = GetStrokeSKPaint(svgCircle);

                            canvas.DrawCircle(cx, cy, radius, paint);
                        }
                    }
                    break;
                case SvgEllipse svgEllipse:
                    {
                        float cx = svgEllipse.CenterX;
                        float cy = svgEllipse.CenterY;
                        float rx = svgEllipse.RadiusX;
                        float ry = svgEllipse.RadiusY;

                        if (svgEllipse.Fill != null && svgEllipse.Fill != SvgColourServer.NotSet)
                        {
                            var paint = GetFillSKPaint(svgEllipse);

                            canvas.DrawOval(cx, cy, rx, ry, paint);
                        }

                        if (svgEllipse.Stroke != null && svgEllipse.Stroke != SvgColourServer.NotSet)
                        {
                            var paint = GetStrokeSKPaint(svgEllipse);

                            canvas.DrawOval(cx, cy, rx, ry, paint);
                        }
                    }
                    break;
                case SvgRectangle svgRectangle:
                    {
                        float x = svgRectangle.X;
                        float y = svgRectangle.Y;
                        float width = svgRectangle.Width;
                        float height = svgRectangle.Height;
                        float rx = svgRectangle.CornerRadiusX;
                        float ry = svgRectangle.CornerRadiusY;

                        var rect = new SKRect(x, y, x + width, y + height);

                        if (svgRectangle.Fill != null && svgRectangle.Fill != SvgColourServer.NotSet)
                        {
                            var paint = GetFillSKPaint(svgRectangle);

                            if (rx > 0f && ry > 0f)
                            {
                                canvas.DrawRoundRect(rect, rx, ry, paint);
                            }
                            else
                            {
                                canvas.DrawRect(rect, paint);
                            }
                        }

                        if (svgRectangle.Stroke != null && svgRectangle.Stroke != SvgColourServer.NotSet)
                        {
                            var paint = GetStrokeSKPaint(svgRectangle);

                            if (rx > 0f && ry > 0f)
                            {
                                canvas.DrawRoundRect(rect, rx, ry, paint);
                            }
                            else
                            {
                                canvas.DrawRect(rect, paint);
                            }
                        }
                    }
                    break;
                case SvgGroup svgGroup:
                    {
                        Draw(canvas, element.Children);
                    }
                    break;
                case SvgLine svgLine:
                    {
                        var p0 = new SKPoint((float)svgLine.StartX, (float)svgLine.StartY);
                        var p1 = new SKPoint((float)svgLine.EndX, (float)svgLine.EndY);

                        if (svgLine.Stroke != null && svgLine.Stroke != SvgColourServer.NotSet)
                        {
                            if (svgLine.Stroke is SvgColourServer svgColourServer)
                            {
                                var paint = GetStrokeSKPaint(svgLine);

                                canvas.DrawLine(p0, p1, paint);
                            }
                        }
                    }
                    break;
                case SvgPath svgPath:
                    {
                        var path = ToSKPath(svgPath.PathData);
                        if (path == null || path.IsEmpty)
                        {
                            break;
                        }

                        if (svgPath.Fill != null && svgPath.Fill != SvgColourServer.NotSet)
                        {
                            var paint = GetFillSKPaint(svgPath);

                            canvas.DrawPath(path, paint);
                        }

                        if (svgPath.Stroke != null && svgPath.Stroke != SvgColourServer.NotSet)
                        {
                            var paint = GetStrokeSKPaint(svgPath);

                            canvas.DrawPath(path, paint);
                        }
                    }
                    break;
                case SvgPolygon svgPolygon:
                    {
                        var path = ToSKPath(svgPolygon.Points);

                        if (svgPolygon.Fill != null && svgPolygon.Fill != SvgColourServer.NotSet)
                        {
                            var paint = GetFillSKPaint(svgPolygon);

                            canvas.DrawPath(path, paint);
                        }

                        if (svgPolygon.Stroke != null && svgPolygon.Stroke != SvgColourServer.NotSet)
                        {
                            var paint = GetStrokeSKPaint(svgPolygon);

                            canvas.DrawPath(path, paint);
                        }
                    }
                    break;
            }

            canvas.RestoreToCount(count);
        }

        private static void Draw(SKCanvas canvas, List<Element> elements)
        {
            foreach (var element in elements)
            {
                Draw(canvas, element);
            }
        }

        public static void SaveImage(string path, Element element, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100)
        {
            if (element.Original is SvgFragment svgFragment)
            {
                float width = svgFragment.Width;
                float Height = svgFragment.Height;

                var info = new SKImageInfo((int)width, (int)Height);
                using (var bitmap = new SKBitmap(info))
                {
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear(SKColors.Transparent);

                        Draw(canvas, element);

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
