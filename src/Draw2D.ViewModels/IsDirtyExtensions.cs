// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Style.ColorFilters;
using Draw2D.ViewModels.Style.ImageFilters;
using Draw2D.ViewModels.Style.MaskFilters;
using Draw2D.ViewModels.Style.PathEffects;
using Draw2D.ViewModels.Style.Shaders;

namespace Draw2D.ViewModels
{
    public static class IsDirtyExtensions
    {
        public static bool IsColorFilterDirty(this IColorFilter colorFilter)
        {
            if (colorFilter == null)
            {
                return false;
            }

            if (colorFilter.IsDirty)
            {
                return true;
            }

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
                    break;
            }

            return false;
        }

        public static bool IsImageFilterDirty(this IImageFilter imageFilter)
        {
            if (imageFilter == null)
            {
                return false;
            }

            if (imageFilter.IsDirty)
            {
                return true;
            }

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
                    break;
            }

            return false;
        }

        public static bool IsMaskFilterDirty(this IMaskFilter maskFilter)
        {
            if (maskFilter == null)
            {
                return false;
            }

            if (maskFilter.IsDirty)
            {
                return true;
            }

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
                    break;
            }

            return false;
        }

        public static bool IsPathEffectDirty(this IPathEffect pathEffect)
        {
            if (pathEffect == null)
            {
                return false;
            }

            if (pathEffect.IsDirty)
            {
                return true;
            }

            switch (pathEffect)
            {
                case Path1DPathEffect path1DPathEffect:
                    {
                    }
                    break;
                case Path2DLineEffect path2DLineEffect:
                    {
                        if (path2DLineEffect.Matrix?.IsDirty ?? false)
                        {
                            return true;
                        }
                    }
                    break;
                case Path2DPathEffect path2DPathEffect:
                    {
                        if (path2DPathEffect.Matrix?.IsDirty ?? false)
                        {
                            return true;
                        }
                    }
                    break;
                case PathComposeEffect pathComposeEffect:
                    {
                        if ((pathComposeEffect.Outer?.IsPathEffectDirty() ?? false)
                         || (pathComposeEffect.Inner?.IsPathEffectDirty() ?? false))
                        {
                            return true;
                        }
                    }
                    break;
                case PathCornerEffect pathCornerEffect:
                    {
                    }
                    break;
                case PathDashEffect pathDashEffect:
                    {
                    }
                    break;
                case PathDiscreteEffect pathDiscreteEffect:
                    {
                    }
                    break;
                case PathSumEffect pathSumEffect:
                    {
                        if ((pathSumEffect.First?.IsPathEffectDirty() ?? false)
                         || (pathSumEffect.Second?.IsPathEffectDirty() ?? false))
                        {
                            return true;
                        }
                    }
                    break;
                case PathTrimEffect pathTrimEffect:
                    {
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        public static bool IsShaderDirty(this IShader shader)
        {
            if (shader == null)
            {
                return false;
            }

            if (shader.IsDirty)
            {
                return true;
            }

            switch (shader)
            {
                case BitmapShader bitmapShader:
                    {
                        // TODO:
                    }
                    break;
                case ColorShader colorShader:
                    {
                        // TODO:
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
                    break;
            }

            return false;
        }

        public static bool IsPaintEffectsDirty(this IPaintEffects paintEffects)
        {
            if (paintEffects == null)
            {
                return false;
            }

            if (paintEffects.IsDirty
             || (paintEffects?.ColorFilter?.IsColorFilterDirty() ?? false)
             || (paintEffects?.ImageFilter?.IsImageFilterDirty() ?? false)
             || (paintEffects?.MaskFilter?.IsMaskFilterDirty() ?? false)
             || (paintEffects?.PathEffect?.IsPathEffectDirty() ?? false)
             || (paintEffects?.Shader?.IsShaderDirty() ?? false))
            {
                return true;
            }

            return false;
        }

        public static bool IsPaintDirty(this IPaint paint)
        {
            if (paint == null)
            {
                return false;
            }

            if (paint.IsDirty
             || (paint.Color?.IsDirty ?? false)
             || (paint.Typeface?.IsDirty ?? false)
             || (paint.Effects?.IsPaintEffectsDirty() ?? false))
            {
                return true;
            }

            return false;
        }

        public static bool IsShapeStyleDirty(this IShapeStyle style)
        {
            if (style == null)
            {
                return false;
            }

            if (style.IsDirty
             || (style.StrokePaint?.IsPaintDirty() ?? false)
             || (style.FillPaint?.IsPaintDirty() ?? false)
             || (style.TextPaint?.IsPaintDirty() ?? false))
            {
                return true;
            }

            return false;
        }

        public static bool IsStyleLibraryDirty(this IStyleLibrary styleLibrary)
        {
            if (styleLibrary == null)
            {
                return false;
            }

            if (styleLibrary.IsDirty)
            {
                return true;
            }

            if (styleLibrary.Items != null)
            {
                foreach (var style in styleLibrary.Items)
                {
                    if (style?.IsShapeStyleDirty() ?? false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsPointsDirty(this ICanvasContainer canvasContainer)
        {
            if (canvasContainer == null)
            {
                return false;
            }

            var points = new List<IPointShape>();
            canvasContainer.GetPoints(points);

            foreach (var point in points)
            {
                if (point.IsDirty)
                {
                    return true;
                }

                if (point.Template != null)
                {
                    if (point.Template.IsDirty)
                    {
                        return true;
                    }
                }
                
                if (point.Effects?.IsPaintEffectsDirty() ?? false)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCanvasContainerDirty(this ICanvasContainer canvasContainer)
        {
            if (canvasContainer == null)
            {
                return false;
            }

            if (canvasContainer.IsDirty)
            {
                return true;
            }

            if (canvasContainer.Shapes != null)
            {
                foreach (var shape in canvasContainer.Shapes)
                {
                    if (shape.IsDirty)
                    {
                        return true;
                    }

                    if (shape.Effects?.IsPaintEffectsDirty() ?? false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
