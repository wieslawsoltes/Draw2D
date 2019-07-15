// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Style.PathEffects;
using Draw2D.ViewModels.Style.Shaders;

namespace Draw2D.ViewModels
{
    public static class IsDirtyExtensions
    {
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

        public static bool IsStrokePaintDirty(this IStrokePaint strokePaint)
        {
            if (strokePaint == null)
            {
                return false;
            }

            if (strokePaint.IsDirty
             || (strokePaint.Color?.IsDirty ?? false)
             || (strokePaint.PathEffect?.IsPathEffectDirty() ?? false)
             || (strokePaint.Shader?.IsShaderDirty() ?? false))
            {
                return true;
            }

            return false;
        }

        public static bool IsFillPaintDirty(this IFillPaint fillPaint)
        {
            if (fillPaint == null)
            {
                return false;
            }

            if (fillPaint.IsDirty
             || (fillPaint.Color?.IsDirty ?? false)
             || (fillPaint.PathEffect?.IsPathEffectDirty() ?? false)
             || (fillPaint.Shader?.IsShaderDirty() ?? false))
            {
                return true;
            }

            return false;
        }

        public static bool IsTextPaintDirty(this ITextPaint textPaint)
        {
            if (textPaint == null)
            {
                return false;
            }

            if (textPaint.IsDirty
             || (textPaint.Color?.IsDirty ?? false)
             || (textPaint.Typeface?.IsDirty ?? false)
             || (textPaint.PathEffect?.IsPathEffectDirty() ?? false)
             || (textPaint.Shader?.IsShaderDirty() ?? false))
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
             || (style.StrokePaint?.IsStrokePaintDirty() ?? false)
             || (style.FillPaint?.IsFillPaintDirty() ?? false)
             || (style.TextPaint?.IsTextPaintDirty() ?? false))
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
                }
            }

            return false;
        }
    }
}
