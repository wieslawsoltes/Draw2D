// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Text;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Renderers
{
    internal static class SkiaPathHelper
    {
        public static void InsertAndSelectShape(IToolContext context, IBaseShape shape)
        {
            context.ContainerView?.CurrentContainer?.Shapes.Add(shape);
            context.ContainerView?.CurrentContainer?.MarkAsDirty(true);

            context.ContainerView?.SelectionState?.Dehover();
            context.ContainerView?.SelectionState?.Clear();

            shape.Select(context.ContainerView?.SelectionState);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public static ShapeStyle GetShapeStyle(IToolContext context, string styleId)
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

        public static void ToGeometry(Text text, IPointShape topLeft, IPointShape bottomRight, TextStyle style, StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(text?.Value))
            {
                using (var geometry = SkiaHelper.ToGeometry(text, topLeft, bottomRight, style, 0.0, 0.0))
                {
                    sb.AppendLine(geometry.ToSvgPathData());
                }
            }
        }

        public static void ToSvgPathData(IToolContext context, IBaseShape shape, StringBuilder sb)
        {
            switch (shape)
            {
                case LineShape line:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(line, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case CubicBezierShape cubicBezier:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(cubicBezier, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case QuadraticBezierShape quadraticBezier:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(quadraticBezier, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case ConicShape conic:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(conic, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case PathShape pathShape:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(pathShape, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());
                        }
                    }
                    break;
                case RectangleShape rectangle:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(rectangle, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = GetShapeStyle(context, rectangle.StyleId);
                            if (style != null)
                            {
                                ToGeometry(rectangle.Text, rectangle.TopLeft, rectangle.BottomRight, style.TextStyle, sb);
                            }
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        using (var geometry = SkiaHelper.ToGeometry(ellipse, 0.0, 0.0))
                        {
                            sb.AppendLine(geometry.ToSvgPathData());

                            var style = GetShapeStyle(context, ellipse.StyleId);
                            if (style != null)
                            {
                                ToGeometry(ellipse.Text, ellipse.TopLeft, ellipse.BottomRight, style.TextStyle, sb);
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
                            ToGeometry(text.Text, text.TopLeft, text.BottomRight, style.TextStyle, sb);
                        }
                    }
                    break;
            };
        }

        public static string ToSvgPathData(IToolContext context, ISet<IBaseShape> selected)
        {
            var sb = new StringBuilder();

            foreach (var shape in selected)
            {
                ToSvgPathData(context, shape, sb);
            }

            return sb.ToString();
        }

        public static void FromSvgPathData(IToolContext context, string svgPathData)
        {
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                var path = SkiaHelper.ToGeometry(svgPathData);
                var pathShape = SkiaHelper.FromGeometry(path, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                if (pathShape != null)
                {
                    InsertAndSelectShape(context, pathShape);
                }
            }
        }

        public static SKPath ToGeometry(IToolContext context, IBaseShape shape)
        {
            switch (shape)
            {
                case LineShape line:
                    return SkiaHelper.ToGeometry(line, 0.0, 0.0);
                case CubicBezierShape cubicBezier:
                    return SkiaHelper.ToGeometry(cubicBezier, 0.0, 0.0);
                case QuadraticBezierShape quadraticBezier:
                    return SkiaHelper.ToGeometry(quadraticBezier, 0.0, 0.0);
                case ConicShape conic:
                    return SkiaHelper.ToGeometry(conic, 0.0, 0.0);
                case PathShape pathShape:
                    return SkiaHelper.ToGeometry(pathShape, 0.0, 0.0);
                case RectangleShape rectangle:
                    return SkiaHelper.ToGeometry(rectangle, 0.0, 0.0);
                case EllipseShape ellipse:
                    return SkiaHelper.ToGeometry(ellipse, 0.0, 0.0);
                case TextShape text:
                    {
                        var style = GetShapeStyle(context, text.StyleId);
                        if (style != null)
                        {
                            return SkiaHelper.ToGeometry(text.Text, text.TopLeft, text.BottomRight, style.TextStyle, 0.0, 0.0);
                        }
                    }
                    return null;
            };
            return null;
        }

        public static SKPath PathOp(SKPathOp op, IList<SKPath> paths)
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

        public static IList<SKPath> ToGeometries(IToolContext context, IList<IBaseShape> shapes)
        {
            if (shapes == null || shapes.Count <= 0)
            {
                return null;
            }

            var paths = new List<SKPath>();

            for (int i = 0; i < shapes.Count; i++)
            {
                var path = ToGeometry(context, shapes[i]);
                if (path != null)
                {
                    if (!path.IsEmpty)
                    {
                        paths.Add(path);
                    }
                    else
                    {
                        path.Dispose();
                    }
                }
            }

            return paths;
        }

        public static IList<IBaseShape> GetShapes(ISet<IBaseShape> selected)
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

        public static void PathOp(IToolContext context, SKPathOp op, ISet<IBaseShape> selected)
        {
            var shapes = GetShapes(selected);
            if (shapes != null && shapes.Count > 0)
            {
                var paths = ToGeometries(context, shapes);
                if (paths != null && paths.Count > 0)
                {
                    var result = PathOp(op, paths);
                    if (result != null)
                    {
                        if (!result.IsEmpty)
                        {
                            var pathShape = SkiaHelper.FromGeometry(result, context.StyleLibrary?.CurrentStyle, context.PointTemplate);
                            if (pathShape != null)
                            {
                                InsertAndSelectShape(context, pathShape);
                            }
                        }
                        result.Dispose();
                    }

                    for (int i = 0; i < paths.Count; i++)
                    {
                        paths[i].Dispose();
                    }
                }
            }
        }
    }
}
