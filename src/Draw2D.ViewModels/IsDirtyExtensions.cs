﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Style.PathEffects;

namespace Draw2D.ViewModels
{
    public static class IsDirtyExtensions
    {
        public static bool IsPathEffectDirty(this IPathEffect pathEffect)
        {
            if (pathEffect == null)
            {
                return false;
            }

            if (pathEffect.IsDirty)
            {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsPathEffectDirty: true");
#endif
                return true;
            }

            switch (pathEffect)
            {
                case PathComposeEffect pathComposeEffect:
                    {
                        if ((pathComposeEffect.Outer?.IsPathEffectDirty() ?? false)
                         || (pathComposeEffect.Inner?.IsPathEffectDirty() ?? false))
                        {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsPathEffectDirty: true");
#endif
                        }
                    }
                    break;
                case PathSumEffect pathSumEffect:
                    {
                        if ((pathSumEffect.First?.IsPathEffectDirty() ?? false)
                         || (pathSumEffect.Second?.IsPathEffectDirty() ?? false))
                        {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsPathEffectDirty: true");
#endif
                        }
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
             || (strokePaint.PathEffect?.IsPathEffectDirty() ?? false))
            {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsStrokePaintDirty: true");
#endif
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
             || (fillPaint.PathEffect?.IsPathEffectDirty() ?? false))
            {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsFillPaintDirty: true");
#endif
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
             || (textPaint.PathEffect?.IsPathEffectDirty() ?? false))
            {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsTextPaintDirty: true");
#endif
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
#if USE_DEBUG_DIRTY
                Log.WriteLine($"IsShapeStyleDirty: true");
#endif
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
#if USE_DEBUG_DIRTY
                Log.WriteLine($"styleLibrary.IsDirty: true");
#endif
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

        public static bool IsCanvasContainerDirty(this ICanvasContainer canvasContainer)
        {
            if (canvasContainer == null)
            {
                return false;
            }

            if (canvasContainer.IsDirty)
            {
#if USE_DEBUG_DIRTY
                Log.WriteLine($"canvasContainer.IsDirty: true");
#endif
                return true;
            }

            if (canvasContainer.Shapes != null)
            {
                foreach (var shape in canvasContainer.Shapes)
                {
                    if (shape.IsDirty)
                    {
#if USE_DEBUG_DIRTY
                        Log.WriteLine($"shape.IsDirty: true");
#endif
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
#if USE_DEBUG_DIRTY
                    Log.WriteLine($"point.IsDirty: true");
#endif
                    return true;
                }

                if (point.Template != null)
                {
                    if (point.Template.IsDirty)
                    {
#if USE_DEBUG_DIRTY
                        Log.WriteLine($"point.Template.IsDirty: true");
#endif
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
