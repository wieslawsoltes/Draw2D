using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using SkiaSharp;
using Svg;
using Svg.Transforms;

namespace SvgDemo
{
    public static class SkiaSvgRenderer
    {
        public static SKColor GetColor(SvgColourServer svgColourServer, float opacity, bool forStroke = false)
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

        public static SKPath ToSKPath(SvgPath svgPath)
        {
            var sb = new StringBuilder();

            foreach (var svgSegment in svgPath.PathData)
            {
                sb.AppendLine(svgSegment.ToString());
            }

            var pathData = sb.ToString();

            return SKPath.ParseSvgPathData(pathData);
        }

        public static SKMatrix Multiply(ref SKMatrix value1, ref SKMatrix value2)
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

        public static SKMatrix ToSKmatrix(Matrix matrix)
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

        public static void Draw(SKCanvas canvas, Element element)
        {
            int count = Transform(canvas, element.Original.Transforms);

            switch (element.Original)
            {
                case SvgFragment svgFragment:
                    {
                        Draw(canvas, element.Children);
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
                                var paint = new SKPaint();

                                paint.Color = GetColor(svgColourServer, svgLine.StrokeOpacity, true);
                                paint.StrokeWidth = svgLine.StrokeWidth;
                                paint.Style = SKPaintStyle.Stroke;

                                canvas.DrawLine(p0, p1, paint);
                            }
                        }
                    }
                    break;
                case SvgPath svgPath:
                    {
                        var path = ToSKPath(svgPath);
                        if (path == null || path.IsEmpty)
                        {
                            break;
                        }

                        if (svgPath.Fill != null && svgPath.Fill != SvgColourServer.NotSet)
                        {
                            var paint = new SKPaint();

                            if (svgPath.Fill is SvgColourServer svgColourServer)
                            {
                                paint.Color = paint.Color = GetColor(svgColourServer, svgPath.StrokeOpacity, false);
                                paint.Style = SKPaintStyle.Fill;
                            }

                            canvas.DrawPath(path, paint);
                        }

                        if (svgPath.Stroke != null && svgPath.Stroke != SvgColourServer.NotSet)
                        {
                            var paint = new SKPaint();

                            if (svgPath.Stroke is SvgColourServer svgColourServer)
                            {
                                paint.Color = paint.Color = GetColor(svgColourServer, svgPath.StrokeOpacity, true);
                                paint.StrokeWidth = svgPath.StrokeWidth;
                                paint.Style = SKPaintStyle.Stroke;
                            }

                            canvas.DrawPath(path, paint);
                        }
                    }
                    break;
            }

            canvas.RestoreToCount(count);
        }

        public static void Draw(SKCanvas canvas, List<Element> elements)
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
