// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;

namespace Draw2D.Presenters
{
    internal static class IsDirtyExtensions
    {
        public static bool IsShapeStyleDirty(this ShapeStyle style)
        {
            if (style == null)
            {
                return false;
            }

            if (style.IsDirty
             || style.Stroke.IsDirty
             || style.Fill.IsDirty
             || style.TextStyle.IsDirty
             || style.TextStyle.Stroke.IsDirty)
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

            if (styleLibrary.Styles != null)
            {
                foreach (var style in styleLibrary.Styles)
                {
                    if (IsShapeStyleDirty(style))
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
