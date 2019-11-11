// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Style.ColorFilters;
using Draw2D.ViewModels.Style.ImageFilters;
using Draw2D.ViewModels.Style.MaskFilters;
using Draw2D.ViewModels.Style.PathEffects;
using Draw2D.ViewModels.Style.Shaders;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D
{
    internal class SkiaUtil
    {
        internal static SKPaintStyle ToSKPaintStyle(PaintStyle paintStyle)
        {
            switch (paintStyle)
            {
                default:
                case PaintStyle.Fill:
                    return SKPaintStyle.Fill;
                case PaintStyle.Stroke:
                    return SKPaintStyle.Stroke;
                case PaintStyle.StrokeAndFill:
                    return SKPaintStyle.StrokeAndFill;
            }
        }

        internal static SKPathFillType ToSKPathFillType(PathFillType fillType)
        {
            switch (fillType)
            {
                default:
                case PathFillType.Winding:
                    return SKPathFillType.Winding;
                case PathFillType.EvenOdd:
                    return SKPathFillType.EvenOdd;
                case PathFillType.InverseWinding:
                    return SKPathFillType.InverseWinding;
                case PathFillType.InverseEvenOdd:
                    return SKPathFillType.InverseEvenOdd;
            }
        }

        internal static PathFillType ToPathFillType(SKPathFillType fillType)
        {
            switch (fillType)
            {
                default:
                case SKPathFillType.Winding:
                    return PathFillType.Winding;
                case SKPathFillType.EvenOdd:
                    return PathFillType.EvenOdd;
                case SKPathFillType.InverseWinding:
                    return PathFillType.InverseWinding;
                case SKPathFillType.InverseEvenOdd:
                    return PathFillType.InverseEvenOdd;
            }
        }

        internal static SKPathOp ToSKPathOp(PathOp op)
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

        internal static SKFontStyleSlant ToSKFontStyleSlant(FontStyleSlant slant)
        {
            switch (slant)
            {
                default:
                case FontStyleSlant.Upright:
                    return SKFontStyleSlant.Upright;
                case FontStyleSlant.Italic:
                    return SKFontStyleSlant.Italic;
                case FontStyleSlant.Oblique:
                    return SKFontStyleSlant.Oblique;
            }
        }

        internal static SKTypeface ToSKTypeface(Typeface typeface)
        {
            return SKTypeface.FromFamilyName(
                typeface.FontFamily,
                typeface.FontWeight,
                typeface.FontWidth,
                ToSKFontStyleSlant(typeface.FontSlant));
        }

        internal static SKStrokeCap ToSKStrokeCap(StrokeCap cap)
        {
            switch (cap)
            {
                default:
                case StrokeCap.Butt:
                    return SKStrokeCap.Butt;
                case StrokeCap.Round:
                    return SKStrokeCap.Round;
                case StrokeCap.Square:
                    return SKStrokeCap.Square;
            }
        }

        internal static SKStrokeJoin ToSKStrokeJoin(StrokeJoin join)
        {
            switch (join)
            {
                default:
                case StrokeJoin.Miter:
                    return SKStrokeJoin.Miter;
                case StrokeJoin.Round:
                    return SKStrokeJoin.Round;
                case StrokeJoin.Bevel:
                    return SKStrokeJoin.Bevel;
            }
        }

        internal static SKBlendMode ToSKBlendMode(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.Clear:
                    return SKBlendMode.Clear;
                case BlendMode.Src:
                    return SKBlendMode.Src;
                case BlendMode.Dst:
                    return SKBlendMode.Dst;
                case BlendMode.SrcOver:
                default:
                    return SKBlendMode.SrcOver;
                case BlendMode.DstOver:
                    return SKBlendMode.DstOver;
                case BlendMode.SrcIn:
                    return SKBlendMode.SrcIn;
                case BlendMode.DstIn:
                    return SKBlendMode.DstIn;
                case BlendMode.SrcOut:
                    return SKBlendMode.SrcOut;
                case BlendMode.DstOut:
                    return SKBlendMode.DstOut;
                case BlendMode.SrcATop:
                    return SKBlendMode.SrcATop;
                case BlendMode.DstATop:
                    return SKBlendMode.DstATop;
                case BlendMode.Xor:
                    return SKBlendMode.Xor;
                case BlendMode.Plus:
                    return SKBlendMode.Plus;
                case BlendMode.Modulate:
                    return SKBlendMode.Modulate;
                case BlendMode.Screen:
                    return SKBlendMode.Screen;
                case BlendMode.Overlay:
                    return SKBlendMode.Overlay;
                case BlendMode.Darken:
                    return SKBlendMode.Darken;
                case BlendMode.Lighten:
                    return SKBlendMode.Lighten;
                case BlendMode.ColorDodge:
                    return SKBlendMode.ColorDodge;
                case BlendMode.ColorBurn:
                    return SKBlendMode.ColorBurn;
                case BlendMode.HardLight:
                    return SKBlendMode.HardLight;
                case BlendMode.SoftLight:
                    return SKBlendMode.SoftLight;
                case BlendMode.Difference:
                    return SKBlendMode.Difference;
                case BlendMode.Exclusion:
                    return SKBlendMode.Exclusion;
                case BlendMode.Multiply:
                    return SKBlendMode.Multiply;
                case BlendMode.Hue:
                    return SKBlendMode.Hue;
                case BlendMode.Saturation:
                    return SKBlendMode.Saturation;
                case BlendMode.Color:
                    return SKBlendMode.Color;
                case BlendMode.Luminosity:
                    return SKBlendMode.Luminosity;
            }
        }

        internal static SKPath1DPathEffectStyle ToSKPath1DPathEffectStyle(Path1DPathEffectStyle style)
        {
            switch (style)
            {
                default:
                case Path1DPathEffectStyle.Translate:
                    return SKPath1DPathEffectStyle.Translate;
                case Path1DPathEffectStyle.Rotate:
                    return SKPath1DPathEffectStyle.Rotate;
                case Path1DPathEffectStyle.Morph:
                    return SKPath1DPathEffectStyle.Morph;
            }
        }

        internal static SKTrimPathEffectMode ToSKTrimPathEffectMode(TrimPathEffectMode mode)
        {
            switch (mode)
            {
                default:
                case TrimPathEffectMode.Normal:
                    return SKTrimPathEffectMode.Normal;
                case TrimPathEffectMode.Inverted:
                    return SKTrimPathEffectMode.Inverted;
            }
        }

        internal static float[] ToIntervals(string intervals, double strokeWidth)
        {
            string[] values = intervals.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            float[] array = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                array[i] = Convert.ToSingle(values[i]) * (float)strokeWidth;
            }
            if (array.Length >= 2 && array.Length % 2 == 0)
            {
                return array;
            }
            return null;
        }

        internal static SKShaderTileMode ToSKShaderTileMode(ShaderTileMode tileMode)
        {
            switch (tileMode)
            {
                default:
                case ShaderTileMode.Clamp:
                    return SKShaderTileMode.Clamp;
                case ShaderTileMode.Repeat:
                    return SKShaderTileMode.Repeat;
                case ShaderTileMode.Mirror:
                    return SKShaderTileMode.Mirror;
            }
        }

        internal static SKMatrix ToSKMatrix(Matrix matrix)
        {
            return new SKMatrix()
            {
                ScaleX = (float)matrix.ScaleX,
                SkewX = (float)matrix.SkewX,
                TransX = (float)matrix.TransX,
                SkewY = (float)matrix.SkewY,
                ScaleY = (float)matrix.ScaleY,
                TransY = (float)matrix.TransY,
                Persp0 = (float)matrix.Persp0,
                Persp1 = (float)matrix.Persp1,
                Persp2 = (float)matrix.Persp2
            };
        }

        internal static SKMatrix Multiply(ref SKMatrix value1, ref SKMatrix value2)
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

        internal static SKPoint ToSKPoint(IPointShape point, double dx, double dy)
        {
            return new SKPoint((float)(point.X + dx), (float)(point.Y + dy));
        }

        internal static SKRect ToSKRect(double left, double top, double right, double bottom)
        {
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        internal static SKRect ToSKRect(IPointShape p1, IPointShape p2, double dx, double dy)
        {
            double x1 = p1.X + dx;
            double y1 = p1.Y + dy;
            double x2 = p2.X + dx;
            double y2 = p2.Y + dy;
            double left = Math.Min(x1, x2);
            double top = Math.Min(y1, y2);
            double right = left + Math.Abs(Math.Max(x1, x2) - left);
            double bottom = top + Math.Abs(Math.Max(y1, y2) - top);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        internal static SKRect ToSKRect(IPointShape center, double radius, double dx, double dy)
        {
            return new SKRect(
                (float)(center.X - radius + dx),
                (float)(center.Y - radius + dy),
                (float)(center.X + radius + dx),
                (float)(center.Y + radius + dy));
        }

        internal static SKPathEffect ToSKPathEffect(IPathEffect pathEffect, double strokeWidth, IList<IDisposable> disposables)
        {
            switch (pathEffect)
            {
                case Path1DPathEffect path1DPathEffect:
                    {
                        if (path1DPathEffect.Path != null)
                        {
                            var geometry = SKPath.ParseSvgPathData(path1DPathEffect.Path);
                            if (geometry != null)
                            {
                                if (geometry.IsEmpty == false)
                                {
                                    var skPathEffect = SKPathEffect.Create1DPath(
                                        geometry,
                                        (float)path1DPathEffect.Advance,
                                        (float)path1DPathEffect.Phase,
                                        ToSKPath1DPathEffectStyle(path1DPathEffect.Style));
                                    disposables.Add(geometry);
                                    disposables.Add(skPathEffect);
                                    return skPathEffect;
                                }
                                geometry.Dispose();
                            }
                        }
                    }
                    break;
                case Path2DLineEffect path2DLineEffect:
                    {
                        if (path2DLineEffect.Matrix != null)
                        {
                            var matrix = ToSKMatrix(path2DLineEffect.Matrix);
                            var skPathEffect = SKPathEffect.Create2DLine((float)path2DLineEffect.Width, matrix);
                            disposables.Add(skPathEffect);
                            return skPathEffect;
                        }
                    }
                    break;
                case Path2DPathEffect path2DPathEffect:
                    {
                        if (path2DPathEffect.Matrix != null && path2DPathEffect.Path != null)
                        {
                            var geometry = SKPath.ParseSvgPathData(path2DPathEffect.Path);
                            if (geometry != null)
                            {
                                if (geometry.IsEmpty == false)
                                {
                                    var matrix = ToSKMatrix(path2DPathEffect.Matrix);
                                    var skPathEffect = SKPathEffect.Create2DPath(matrix, geometry);
                                    disposables.Add(skPathEffect);
                                    return skPathEffect;
                                }
                                geometry.Dispose();
                            }
                        }
                    }
                    break;
                case PathComposeEffect pathComposeEffect:
                    {
                        if (pathComposeEffect.Outer != null && pathComposeEffect.Inner != null)
                        {
                            var composeDisposables = new List<IDisposable>();
                            var skPathEffectOuter = ToSKPathEffect(pathComposeEffect.Outer, strokeWidth, composeDisposables);
                            var skPathEffectInner = ToSKPathEffect(pathComposeEffect.Inner, strokeWidth, composeDisposables);

                            if (skPathEffectOuter != null && skPathEffectInner != null)
                            {
                                foreach (var disposable in composeDisposables)
                                {
                                    disposables.Add(disposable);
                                }

                                var skPathEffect = SKPathEffect.CreateCompose(skPathEffectOuter, skPathEffectInner);
                                disposables.Add(skPathEffect);
                                return skPathEffect;
                            }
                            else
                            {
                                foreach (var disposable in composeDisposables)
                                {
                                    disposable.Dispose();
                                }
                            }
                        }
                    }
                    break;
                case PathCornerEffect pathCornerEffect:
                    {
                        if (pathCornerEffect.Radius > 0)
                        {
                            var skPathEffect = SKPathEffect.CreateCorner((float)pathCornerEffect.Radius);
                            disposables.Add(skPathEffect);
                            return skPathEffect;
                        }
                    }
                    break;
                case PathDashEffect pathDashEffect:
                    {
                        var intervals = ToIntervals(pathDashEffect.Intervals, strokeWidth);
                        if (intervals != null)
                        {
                            var skPathEffect = SKPathEffect.CreateDash(intervals, (float)pathDashEffect.Phase);
                            disposables.Add(skPathEffect);
                            return skPathEffect;
                        }
                    }
                    break;
                case PathDiscreteEffect pathDiscreteEffect:
                    {
                        var skPathEffect = SKPathEffect.CreateDiscrete(
                            (float)pathDiscreteEffect.SegLength,
                            (float)pathDiscreteEffect.Deviation,
                            pathDiscreteEffect.SeedAssist);
                        disposables.Add(skPathEffect);
                        return skPathEffect;
                    }
                case PathSumEffect pathSumEffect:
                    {
                        if (pathSumEffect.First != null && pathSumEffect.Second != null)
                        {
                            var composeDisposables = new List<IDisposable>();
                            var skPathEffectFirst = ToSKPathEffect(pathSumEffect.First, strokeWidth, composeDisposables);
                            var skPathEffectSecond = ToSKPathEffect(pathSumEffect.Second, strokeWidth, composeDisposables);

                            if (skPathEffectFirst != null && skPathEffectSecond != null)
                            {
                                foreach (var disposable in composeDisposables)
                                {
                                    disposables.Add(disposable);
                                }

                                var skPathEffect = SKPathEffect.CreateSum(skPathEffectFirst, skPathEffectSecond);
                                disposables.Add(skPathEffect);
                                return skPathEffect;
                            }
                            else
                            {
                                foreach (var disposable in composeDisposables)
                                {
                                    disposable?.Dispose();
                                }
                            }
                        }
                    }
                    break;
                case PathTrimEffect pathTrimEffect:
                    {
                        var skPathEffect = SKPathEffect.CreateTrim(
                            (float)pathTrimEffect.Start,
                            (float)pathTrimEffect.Stop,
                            ToSKTrimPathEffectMode(pathTrimEffect.Mode));
                        disposables.Add(skPathEffect);
                        return skPathEffect;
                    }
                default:
                    return null;
            }
            return null;
        }

        internal static SKColorFilter ToSKColorFilter(IColorFilter colorFilter, IList<IDisposable> disposables)
        {
            switch (colorFilter)
            {
                case BlendModeColorFilter blendModeColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case ColorCubeColorFilter colorCubeColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case ColorMatrixColorFilter colorMatrixColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case ComposeColorFilter composeColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case GammaColorFilter gammaColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case HighContrastColorFilter highContrastColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case LightingColorFilter lightingColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case LumaColorColorFilter lumaColorColorFilter:
                    {
                        // TODO:
                    }
                    break;
                case TableColorFilter tableColorFilter:
                    {
                        // TODO:
                    }
                    break;
                default:
                    return null;
            }
            return null;
        }

        internal static SKImageFilter ToSKImageFilter(IImageFilter imageFilter, IList<IDisposable> disposables)
        {
            switch (imageFilter)
            {
                case AlphaThresholdImageFilter alphaThresholdImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case ArithmeticImageFilter arithmeticImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case BlendModeImageFilter blendModeImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case BlurImageFilter blurImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case ColorFilterImageFilter colorFilterImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case ComposeImageFilter composeImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case DilateImageFilter dilateImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case DisplacementMapEffectImageFilter displacementMapEffectImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case DistantLitDiffuseImageFilter distantLitDiffuseImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case DistantLitSpecularImageFilter distantLitSpecularImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case DropShadowImageFilter dropShadowImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case ErodeImageFilter erodeImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case ImageImageFilter imageImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case MagnifierImageFilter magnifierImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case MatrixImageFilter matrixImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case MatrixConvolutionImageFilter matrixConvolutionImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case MergeImageFilter mergeImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case OffsetImageFilter offsetImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case PaintImageFilter paintImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case PictureImageFilter pictureImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case PointLitDiffuseImageFilter pointLitDiffuseImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case PointLitSpecularImageFilter pointLitSpecularImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case SpotLitDiffuseImageFilter spotLitDiffuseImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case SpotLitSpecularImageFilter spotLitSpecularImageFilter:
                    {
                        // TODO:
                    }
                    break;
                case TileImageFilter tileImageFilter:
                    {
                        // TODO:
                    }
                    break;
                default:
                    return null;
            }
            return null;
        }

        internal static SKMaskFilter ToSKMaskFilter(IMaskFilter maskFilter, IList<IDisposable> disposables)
        {
            switch (maskFilter)
            {
                case BlurMaskFilter blurMaskFilter:
                    {
                        // TODO:
                    }
                    break;
                case ClipMaskFilter clipMaskFilter:
                    {
                        // TODO:
                    }
                    break;
                case GammaMaskFilter gammaMaskFilter:
                    {
                        // TODO:
                    }
                    break;
                case TableMaskFilter tableMaskFilter:
                    {
                        // TODO:
                    }
                    break;
                default:
                    return null;
            }
            return null;
        }

        internal static SKShader ToSKShader(IShader shader, IList<IDisposable> disposables)
        {
            switch (shader)
            {
                case BitmapShader bitmapShader:
                    {
                        // TODO:
                    }
                    break;
                case ColorShader colorShader:
                    {
                        if (colorShader.Color != null)
                        {
                            var skShader = SKShader.CreateColor(ToSKColor(colorShader.Color));
                            disposables.Add(skShader);
                            return skShader;
                        }
                    }
                    break;
                case ColorFilterShader colorFilterShader:
                    {
                        // TODO:
                    }
                    break;
                case ComposeShader composeShader:
                    {
                        // TODO:
                    }
                    break;
                case EmptyShader emptyShader:
                    {
                        // TODO:
                    }
                    break;
                case LinearGradientShader linearGradientShader:
                    {
                        // TODO:
                    }
                    break;
                case LocalMatrixShader localMatrixShader:
                    {
                        // TODO:
                    }
                    break;
                case PerlinNoiseFractalNoiseShader perlinNoiseFractalNoiseShader:
                    {
                        // TODO:
                    }
                    break;
                case PerlinNoiseTurbulenceShader perlinNoiseTurbulenceShader:
                    {
                        // TODO:
                    }
                    break;
                case RadialGradientShader radialGradientShader:
                    {
                        // TODO:
                    }
                    break;
                case SweepGradientShader sweepGradientShader:
                    {
                        // TODO:
                    }
                    break;
                case TwoPointConicalGradientShader twoPointConicalGradientShader:
                    {
                        // TODO:
                    }
                    break;
                default:
                    return null;
            }
            return null;
        }

        internal static SKColor ToSKColor(ArgbColor color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        internal static void SetSKPaintEffects(SKPaint paint, IPaintEffects paintEffects, IPaintEffects overrideEffects, IList<IDisposable> disposables)
        {
            var sKBlendMode = overrideEffects?.BlendMode == BlendMode.SrcOver ?
                ToSKBlendMode(paintEffects?.BlendMode ?? BlendMode.SrcOver) :
                ToSKBlendMode(overrideEffects?.BlendMode ?? BlendMode.SrcOver);

            var sKColorFilter = (overrideEffects?.ColorFilter == null) ?
                ToSKColorFilter(paintEffects?.ColorFilter, disposables) :
                ToSKColorFilter(overrideEffects?.ColorFilter, disposables);

            var sKImageFilter = (overrideEffects?.ImageFilter == null) ?
                ToSKImageFilter(paintEffects?.ImageFilter, disposables) :
                ToSKImageFilter(overrideEffects?.ImageFilter, disposables);

            var sKMaskFilter = (overrideEffects?.MaskFilter == null) ?
                ToSKMaskFilter(paintEffects?.MaskFilter, disposables) :
                ToSKMaskFilter(overrideEffects?.MaskFilter, disposables);

            var sKPathEffect = (overrideEffects?.PathEffect == null) ?
                ToSKPathEffect(paintEffects?.PathEffect, paint.StrokeWidth, disposables) :
                ToSKPathEffect(overrideEffects?.PathEffect, paint.StrokeWidth, disposables);

            var sKShader = overrideEffects?.Shader != null ?
                ToSKShader(overrideEffects?.Shader, disposables) :
                ToSKShader(paintEffects?.Shader, disposables);

            paint.BlendMode = sKBlendMode;
            paint.ColorFilter = sKColorFilter;
            paint.ImageFilter = sKImageFilter;
            paint.MaskFilter = sKMaskFilter;
            paint.PathEffect = sKPathEffect;
            paint.Shader = sKShader;
        }

        internal static SKPaint ToSKPaint(IPaint paint, IPaintEffects effects, double scale, IList<IDisposable> disposables)
        {
            var skPaint = new SKPaint()
            {
                Style = ToSKPaintStyle(paint.PaintStyle),
                Color = ToSKColor(paint.Color),
                IsAntialias = paint.IsAntialias,
                TextAlign = SKTextAlign.Left
            };

            switch (paint.PaintStyle)
            {
                default:
                case PaintStyle.Fill:
                    {
                    }
                    break;
                case PaintStyle.Stroke:
                case PaintStyle.StrokeAndFill:
                    {
                        // Stroke

                        double strokeWidth = paint.StrokeWidth;
                        double strokeMiter = paint.StrokeMiter;
                        if (paint.IsScaled)
                        {
                            strokeWidth /= scale;
                            strokeMiter /= scale;
                        }
                        skPaint.StrokeWidth = (float)(strokeWidth);
                        skPaint.StrokeCap = ToSKStrokeCap(paint.StrokeCap);
                        skPaint.StrokeJoin = ToSKStrokeJoin(paint.StrokeJoin);
                        skPaint.StrokeMiter = (float)(strokeMiter);
                    }
                    break;
            }

            // Text

            skPaint.LcdRenderText = paint.LcdRenderText;
            skPaint.SubpixelText = paint.SubpixelText;
            skPaint.TextEncoding = SKTextEncoding.Utf16;
            skPaint.TextSize = (float)paint.FontSize;

            switch (paint.HAlign)
            {
                default:
                case HAlign.Left:
                    skPaint.TextAlign = SKTextAlign.Left;
                    break;
                case HAlign.Center:
                    skPaint.TextAlign = SKTextAlign.Center;
                    break;
                case HAlign.Right:
                    skPaint.TextAlign = SKTextAlign.Right;
                    break;
            }

            // Effects

            SetSKPaintEffects(skPaint, paint.Effects, effects, disposables);

            return skPaint;
        }

        internal static void ToSKPaintUpdate(SKPaint skPaint, IPaint paint, IPaintEffects effects, double scale, IList<IDisposable> disposables)
        {
            double strokeWidth = paint.StrokeWidth;
            double strokeMiter = paint.StrokeMiter;
            if (paint.IsScaled)
            {
                strokeWidth /= scale;
                strokeMiter /= scale;
            }
            skPaint.StrokeWidth = (float)(strokeWidth);
            skPaint.StrokeMiter = (float)(strokeMiter);
        }

        internal static void AddLine(IToolContext context, LineShape line, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(line.StartPoint, dx, dy));
            geometry.LineTo(ToSKPoint(line.Point, dx, dy));
        }

        internal static void AddCubic(IToolContext context, CubicBezierShape cubicBezier, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(cubicBezier.StartPoint, dx, dy));
            geometry.CubicTo(
                ToSKPoint(cubicBezier.Point1, dx, dy),
                ToSKPoint(cubicBezier.Point2, dx, dy),
                ToSKPoint(cubicBezier.Point3, dx, dy));
        }

        internal static void AddQuad(IToolContext context, QuadraticBezierShape quadraticBezier, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(quadraticBezier.StartPoint, dx, dy));
            geometry.QuadTo(
                ToSKPoint(quadraticBezier.Point1, dx, dy),
                ToSKPoint(quadraticBezier.Point2, dx, dy));
        }

        internal static void AddConic(IToolContext context, ConicShape conic, double dx, double dy, SKPath geometry)
        {
            geometry.MoveTo(ToSKPoint(conic.StartPoint, dx, dy));
            geometry.ConicTo(
                ToSKPoint(conic.Point1, dx, dy),
                ToSKPoint(conic.Point2, dx, dy),
                (float)conic.Weight);
        }

        internal static void AddRect(IToolContext context, RectangleShape rectangle, double dx, double dy, SKPath geometry)
        {
            var rect = ToSKRect(rectangle.StartPoint, rectangle.Point, dx, dy);
            if (rectangle.RadiusX > 0.0 && rectangle.RadiusY > 0.0)
            {
                geometry.AddRoundRect(rect, (float)rectangle.RadiusX, (float)rectangle.RadiusY, SKPathDirection.Clockwise);
            }
            else
            {
                geometry.AddRect(rect, SKPathDirection.Clockwise);
            }
        }

        internal static void AddCircle(IToolContext context, CircleShape circle, double dx, double dy, SKPath geometry)
        {
            var distance = circle.StartPoint.DistanceTo(circle.Point);
            geometry.AddCircle((float)circle.StartPoint.X, (float)circle.StartPoint.Y, (float)distance, SKPathDirection.Clockwise);
        }

        internal static void AddArc(IToolContext context, ArcShape arc, double dx, double dy, SKPath geometry)
        {
            var distance = arc.StartPoint.DistanceTo(arc.Point);
            var rect = ToSKRect(arc.StartPoint, distance, dx, dy);
            geometry.AddArc(rect, (float)arc.StartAngle, (float)arc.SweepAngle);
        }

        internal static void AddOval(IToolContext context, OvalShape oval, double dx, double dy, SKPath geometry)
        {
            var rect = ToSKRect(oval.StartPoint, oval.Point, dx, dy);
            geometry.AddOval(rect, SKPathDirection.Clockwise);
        }

        internal static void AddText(IToolContext context, Text text, IPointShape startPoint, IPointShape point, IPaint paint, double dx, double dy, SKPath geometry)
        {
            using (var typeface = ToSKTypeface(paint.Typeface))
            using (var disposable = new CompositeDisposable())
            using (var skPaint = ToSKPaint(paint, null, 1.0, disposable.Disposables))
            {
                skPaint.Typeface = typeface;
                skPaint.TextEncoding = SKTextEncoding.Utf16;
                skPaint.TextSize = (float)paint.FontSize;

                switch (paint.HAlign)
                {
                    default:
                    case HAlign.Left:
                        skPaint.TextAlign = SKTextAlign.Left;
                        break;
                    case HAlign.Center:
                        skPaint.TextAlign = SKTextAlign.Center;
                        break;
                    case HAlign.Right:
                        skPaint.TextAlign = SKTextAlign.Right;
                        break;
                }

                var metrics = skPaint.FontMetrics;
                var mAscent = metrics.Ascent;
                var mDescent = metrics.Descent;
                var rect = ToSKRect(startPoint, point, dx, dy);
                float x = rect.Left;
                float y = rect.Top;
                float width = rect.Width;
                float height = rect.Height;

                switch (paint.VAlign)
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

                switch (paint.HAlign)
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

                using (var outlineGeometry = skPaint.GetTextPath(text.Value, x, y))
                using (var fillGeometry = skPaint.GetFillPath(outlineGeometry))
                {
                    geometry.AddPath(fillGeometry, SKPathAddMode.Append);
                }
            }
        }

        internal static void AddText(IToolContext context, TextShape text, double dx, double dy, SKPath geometry)
        {
            var style = context?.DocumentContainer?.StyleLibrary?.Get(text.StyleId);
            if (style != null)
            {
                AddText(context, text.Text, text.StartPoint, text.Point, style.TextPaint, dx, dy, geometry);
            }
        }

        internal static void AddImage(IToolContext context, ImageShape rectangle, double dx, double dy, SKPath geometry)
        {
            var rect = ToSKRect(rectangle.StartPoint, rectangle.Point, dx, dy);
            geometry.AddRect(rect, SKPathDirection.Clockwise);
        }

        internal static void AddPath(IToolContext context, PathShape path, double dx, double dy, SKPath geometry)
        {
            geometry.FillType = ToSKPathFillType(path.FillType);

            foreach (var shape in path.Shapes)
            {
                if (shape is FigureShape figure)
                {
                    AddFigure(context, figure, dx, dy, geometry);
                }
            }
        }

        internal static void AddFigure(IToolContext context, IList<IBaseShape> shapes, bool isClosed, double dx, double dy, SKPath geometry)
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
                                geometry.MoveTo(ToSKPoint(line.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.LineTo(ToSKPoint(line.Point, dx, dy));
                        }
                        break;
                    case CubicBezierShape cubicBezier:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(cubicBezier.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.CubicTo(
                                ToSKPoint(cubicBezier.Point1, dx, dy),
                                ToSKPoint(cubicBezier.Point2, dx, dy),
                                ToSKPoint(cubicBezier.Point3, dx, dy));
                        }
                        break;
                    case QuadraticBezierShape quadraticBezier:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(quadraticBezier.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.QuadTo(
                                ToSKPoint(quadraticBezier.Point1, dx, dy),
                                ToSKPoint(quadraticBezier.Point2, dx, dy));
                        }
                        break;
                    case ConicShape conic:
                        {
                            if (isFirstShape)
                            {
                                geometry.MoveTo(ToSKPoint(conic.StartPoint, dx, dy));
                                isFirstShape = false;
                            }
                            geometry.ConicTo(
                                ToSKPoint(conic.Point1, dx, dy),
                                ToSKPoint(conic.Point2, dx, dy),
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

        internal static void AddFigure(IToolContext context, FigureShape figure, double dx, double dy, SKPath geometry)
        {
            AddFigure(context, figure.Shapes, figure.IsClosed, dx, dy, geometry);
        }

        internal static void AddFigure(IToolContext context, GroupShape group, double dx, double dy, SKPath geometry)
        {
            AddFigure(context, group.Shapes, false, dx, dy, geometry);
        }

        internal static void AddFigure(IToolContext context, ReferenceShape reference, double dx, double dy, SKPath geometry)
        {
            AddShape(context, reference.Template, dx + reference.X, dy + reference.Y, geometry);
        }

        internal static bool AddShape(IToolContext context, IBaseShape shape, double dx, double dy, SKPath geometry)
        {
            switch (shape)
            {
                case LineShape line:
                    AddLine(context, line, dx, dy, geometry);
                    return true;
                case CubicBezierShape cubicBezier:
                    AddCubic(context, cubicBezier, dx, dy, geometry);
                    return true;
                case QuadraticBezierShape quadraticBezier:
                    AddQuad(context, quadraticBezier, dx, dy, geometry);
                    return true;
                case ConicShape conic:
                    AddConic(context, conic, dx, dy, geometry);
                    return true;
                case FigureShape figure:
                    AddFigure(context, figure, dx, dy, geometry);
                    return true;
                case PathShape path:
                    AddPath(context, path, dx, dy, geometry);
                    return true;
                case RectangleShape rectangle:
                    AddRect(context, rectangle, dx, dy, geometry);
                    return true;
                case CircleShape circle:
                    AddCircle(context, circle, dx, dy, geometry);
                    return true;
                case ArcShape arc:
                    AddArc(context, arc, dx, dy, geometry);
                    return true;
                case OvalShape oval:
                    AddOval(context, oval, dx, dy, geometry);
                    return true;
                case IPointShape point:
                    AddShape(context, point, dx, dy, geometry);
                    return true;
                case TextShape text:
                    AddText(context, text, dx, dy, geometry);
                    return true;
                case ImageShape image:
                    AddImage(context, image, dx, dy, geometry);
                    return true;
                case GroupShape group:
                    AddFigure(context, group, dx, dy, geometry);
                    return true;
                case ReferenceShape reference:
                    AddFigure(context, reference, dx, dy, geometry);
                    return true;
            };
            return false;
        }

        internal static SKPath ToStrokePath(IToolContext context, IPaint paint, IPaintEffects effects, SKPath geometry, IList<IDisposable> disposables)
        {
            using (var skPaint = ToSKPaint(paint, null, 1.0, disposables))
            {
                bool isClipPath = GetInflateOffset(effects?.PathEffect != null ? effects.PathEffect : paint.Effects?.PathEffect, out var inflateX, out var inflateY);
                if (isClipPath)
                {
                    var bounds = geometry.Bounds;
                    var rect = SKRect.Inflate(bounds, (float)inflateX, (float)inflateY);
                    // TODO: Clip path.
                    var path = skPaint.GetFillPath(geometry, 1.0f);
                    return path;
                }
                else
                {
                    return skPaint.GetFillPath(geometry, 1.0f);
                }
            }
        }

        internal static SKPath ToFillPath(IToolContext context, IPaint paint, IPaintEffects effects, SKPath geometry, IList<IDisposable> disposables)
        {
            using (var skPaint = ToSKPaint(paint, null, 1.0, disposables))
            {
                bool isClipPath = GetInflateOffset(effects?.PathEffect != null ? effects.PathEffect : paint.Effects?.PathEffect, out var inflateX, out var inflateY);
                if (isClipPath)
                {
                    var bounds = geometry.Bounds;
                    var rect = SKRect.Inflate(bounds, (float)inflateX, (float)inflateY);
                    // TODO: Clip path.
                    var path = skPaint.GetFillPath(geometry, 1.0f);
                    return path;
                }
                else
                {
                    return skPaint.GetFillPath(geometry, 1.0f);
                }
            }
        }

        internal static SKPath ToPath(string svgPathData)
        {
            return SKPath.ParseSvgPathData(svgPathData);
        }

        internal static SKPath Op(SKPathOp op, IList<SKPath> paths)
        {
            if (paths == null || paths.Count <= 0)
            {
                return null;
            }

            if (paths.Count == 1)
            {
                using (var empty = new SKPath() { FillType = paths[0].FillType })
                {
                    return empty.Op(paths[0], op);
                }
            }
            else
            {
                var haveResult = false;
                var result = new SKPath(paths[0]) { FillType = paths[0].FillType };

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

        internal static PathShape ToPathShape(IToolContext context, SKPath path, IShapeStyle style, IBaseShape pointTemplate)
        {
            var pathShape = new PathShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                FillType = ToPathFillType(path.FillType),
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
                                    Points = new ObservableCollection<IPointShape>(),
                                    Shapes = new ObservableCollection<IBaseShape>(),
                                    IsFilled = true,
                                    IsClosed = false
                                };
                                figureShape.Owner = pathShape;
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
                                lineShape.Owner = figureShape;
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
                                cubicBezierShape.Owner = figureShape;
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
                                quadraticBezierShape.Owner = figureShape;
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
                                conicShape.Owner = figureShape;
                                conicShape.StartPoint.Owner = conicShape;
                                conicShape.Point1.Owner = conicShape;
                                conicShape.Point2.Owner = conicShape;
                                figureShape.Shapes.Add(conicShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Close:
                            {
                                // TODO:
                                //var line = new LineShape()
                                //{
                                //    Points = new ObservableCollection<IPointShape>(),
                                //    StartPoint = pathShape.GetLastPoint(),
                                //    Point = pathShape.GetFirstPoint(),
                                //    Text = new Text(),
                                //    StyleId = style.Title
                                //};
                                //line.Owner = figureShape;
                                //line.StartPoint.Owner = line;
                                //line.Point.Owner = line;
                                //figureShape.Shapes.Add(line);
                                figureShape.IsClosed = true;
                                firstPoint = lastPoint = new SKPoint(0, 0);
                            }
                            break;
                    }
                }
            }

            return pathShape;
        }

        internal static void ToSvgPathData(IToolContext context, Text text, IPointShape startPoint, IPointShape point, IPaint paint, StringBuilder sb, SKPathFillType fillType = SKPathFillType.Winding)
        {
            if (!string.IsNullOrEmpty(text?.Value))
            {
                using (var geometry = new SKPath() { FillType = fillType })
                {
                    AddText(context, text, startPoint, point, paint, 0.0, 0.0, geometry);
                    sb.AppendLine(geometry.ToSvgPathData());
                }
            }
        }

        internal static void ToSvgPathData(IToolContext context, IBaseShape shape, StringBuilder sb, SKPathFillType fillType = SKPathFillType.Winding)
        {
            switch (shape)
            {
                case LineShape line:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddLine(context, line, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case CubicBezierShape cubicBezier:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddCubic(context, cubicBezier, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case QuadraticBezierShape quadraticBezier:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddQuad(context, quadraticBezier, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case ConicShape conic:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddConic(context, conic, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case PathShape pathShape:
                    {
                        using (var geometry = new SKPath() { FillType = SkiaUtil.ToSKPathFillType(pathShape.FillType) })
                        {
                            AddPath(context, pathShape, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case RectangleShape rectangle:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddRect(context, rectangle, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.DocumentContainer?.StyleLibrary?.Get(rectangle.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, rectangle.Text, rectangle.StartPoint, rectangle.Point, style.TextPaint, sb, fillType);
                            }
                        }
                    }
                    break;
                case CircleShape circle:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddCircle(context, circle, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.DocumentContainer?.StyleLibrary?.Get(circle.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, circle.Text, circle.StartPoint, circle.Point, style.TextPaint, sb, fillType);
                            }
                        }
                    }
                    break;
                case ArcShape arc:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddArc(context, arc, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.DocumentContainer?.StyleLibrary?.Get(arc.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, arc.Text, arc.StartPoint, arc.Point, style.TextPaint, sb, fillType);
                            }
                        }
                    }
                    break;
                case OvalShape oval:
                    {
                        using (var geometry = new SKPath() { FillType = fillType })
                        {
                            AddOval(context, oval, 0.0, 0.0, geometry);
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = context?.DocumentContainer?.StyleLibrary?.Get(oval.StyleId);
                            if (style != null)
                            {
                                ToSvgPathData(context, oval.Text, oval.StartPoint, oval.Point, style.TextPaint, sb, fillType);
                            }
                        }
                    }
                    break;
                case GroupShape group:
                    {
                        foreach (var groupShape in group.Shapes)
                        {
                            ToSvgPathData(context, groupShape, sb, fillType);
                        }
                    }
                    break;
                case TextShape text:
                    {
                        var style = context?.DocumentContainer?.StyleLibrary?.Get(text.StyleId);
                        if (style != null)
                        {
                            ToSvgPathData(context, text.Text, text.StartPoint, text.Point, style.TextPaint, sb, fillType);
                        }
                    }
                    break;
            };
        }

        internal static SKPicture SvgToSKPicture(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                using (var stream = new StringReader(xml))
                using (var reader = XmlReader.Create(stream))
                {
                    var svg = new SkiaSharp.Extended.Svg.SKSvg();
                    return svg.Load(reader);
                }
            }
            return null;
        }

        internal static SKPicture ToSKPicture(string path)
        {
            var extension = Path.GetExtension(path);

            if (string.Compare(extension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                var xml = File.ReadAllText(path);
                if (!string.IsNullOrEmpty(xml))
                {
                    return SvgToSKPicture(xml);
                }
                return null;
            }

            using (var recorder = new SKPictureRecorder())
            {
                var image = SKImage.FromEncodedData(path);
                var rect = new SKRect(0f, 0f, (float)image.Width, (float)image.Height);
                var canvas = recorder.BeginRecording(rect);
                canvas.DrawImage(image, rect);
                return recorder.EndRecording();
            }
        }

        internal static bool GetInflateOffset(IPathEffect pathEffect, out double inflateX, out double inflateY)
        {
            inflateX = 0;
            inflateY = 0;
            if (pathEffect == null)
            {
                return false;
            }
            switch (pathEffect)
            {
                case Path1DPathEffect path1DPathEffect:
                    {
                        return false;
                    }
                case Path2DLineEffect path2DLineEffect:
                    {
                        if (path2DLineEffect.Matrix != null)
                        {
                            inflateX = Math.Max(Math.Abs(path2DLineEffect.Matrix.ScaleX), Math.Abs(path2DLineEffect.Matrix.SkewX));
                            inflateY = Math.Max(Math.Abs(path2DLineEffect.Matrix.ScaleY), Math.Abs(path2DLineEffect.Matrix.SkewY));
                            return true;
                        }
                        return false;
                    }
                case Path2DPathEffect path2DPathEffect:
                    {
                        if (path2DPathEffect.Matrix != null)
                        {
                            inflateX = Math.Max(Math.Abs(path2DPathEffect.Matrix.ScaleX), Math.Abs(path2DPathEffect.Matrix.SkewX));
                            inflateY = Math.Max(Math.Abs(path2DPathEffect.Matrix.ScaleY), Math.Abs(path2DPathEffect.Matrix.SkewY));
                            return true;
                        }
                        return false;
                    }
                case PathComposeEffect pathComposeEffect:
                    {
                        bool isClipPathInner = GetInflateOffset(pathComposeEffect.Inner, out var inflateInnerX, out var inflateInnerY);
                        bool isClipPathOuter = GetInflateOffset(pathComposeEffect.Outer, out var inflateOuterX, out var inflateOuterY);
                        if (isClipPathOuter || isClipPathInner)
                        {
                            inflateX = Math.Max(Math.Abs(inflateInnerX), Math.Abs(inflateOuterX));
                            inflateY = Math.Max(Math.Abs(inflateInnerY), Math.Abs(inflateOuterY));
                            return true;
                        }
                        return false;
                    }
                case PathCornerEffect pathCornerEffect:
                    {
                        return false;
                    }
                case PathDashEffect pathDashEffect:
                    {
                        return false;
                    }
                case PathDiscreteEffect pathDiscreteEffect:
                    {
                        return false;
                    }
                case PathSumEffect pathSumEffect:
                    {
                        bool isClipPathFirst = GetInflateOffset(pathSumEffect.First, out var inflateFirstX, out var inflateFirstY);
                        bool isClipPathSecond = GetInflateOffset(pathSumEffect.Second, out var inflateSecondX, out var inflateSecondY);
                        if (isClipPathFirst || isClipPathSecond)
                        {
                            inflateX = Math.Max(Math.Abs(inflateFirstX), Math.Abs(inflateSecondX));
                            inflateY = Math.Max(Math.Abs(inflateFirstY), Math.Abs(inflateSecondY));
                            return true;
                        }
                        return false;
                    }
                case PathTrimEffect pathTrimEffect:
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        internal static void GetStretchModeTransform(StretchMode mode, SKRect element, SKRect panel, out double ox, out double oy, out double zx, out double zy)
        {
            ox = element.Left;
            oy = element.Top;
            zx = 1.0;
            zy = 1.0;
            switch (mode)
            {
                default:
                case StretchMode.None:
                    break;
                case StretchMode.Center:
                    ox = element.Left + (element.Width - panel.Width) / 2;
                    oy = element.Top + (element.Height - panel.Height) / 2;
                    break;
                case StretchMode.Fill:
                    zx = element.Width / panel.Width;
                    zy = element.Height / panel.Height;
                    ox = element.Left + (element.Width - panel.Width * zx) / 2;
                    oy = element.Top + (element.Height - panel.Height * zy) / 2;
                    break;
                case StretchMode.Uniform:
                    zx = element.Width / panel.Width;
                    zy = element.Height / panel.Height;
                    zx = Math.Min(zx, zy);
                    zy = zx;
                    ox = element.Left + (element.Width - panel.Width * zx) / 2;
                    oy = element.Top + (element.Height - panel.Height * zy) / 2;
                    break;
                case StretchMode.UniformToFill:
                    zx = element.Width / panel.Width;
                    zy = element.Height / panel.Height;
                    zx = Math.Max(zx, zy);
                    zy = zx;
                    ox = element.Left + (element.Width - panel.Width * zx) / 2;
                    oy = element.Top + (element.Height - panel.Height * zy) / 2;
                    break;
            }
        }
    }
}
