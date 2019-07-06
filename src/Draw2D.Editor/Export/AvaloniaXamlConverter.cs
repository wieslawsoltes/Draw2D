// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Text;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Export
{
    public static class AvaloniaXamlConverter
    {
        internal static char[] s_newLine = Environment.NewLine.ToCharArray();

        public static string ConvertToGeometryDrawing(IToolContext context, IContainerView containerView, string indent = "")
        {
            if (containerView.SelectionState?.Shapes != null && containerView.SelectionState?.Shapes.Count > 0)
            {
                var shapes = new List<IBaseShape>(containerView.SelectionState?.Shapes);
                var sb = new StringBuilder();

                foreach (var shape in shapes)
                {
                    if (shape is IPointShape)
                    {
                        continue;
                    }
                    var geometry = context.PathConverter?.ToSvgPathData(context, new[] { shape })?.TrimEnd(s_newLine);
                    if (geometry != null)
                    {
                        var style = context.StyleLibrary?.Get(shape.StyleId);
                        if (style != null)
                        {
                            if (style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"{indent}<GeometryDrawing Brush=\"{style.Fill.ToHex()}\" Geometry=\"{geometry}\">");
                                sb.AppendLine($"{indent}    <GeometryDrawing.Pen>");
                                sb.AppendLine($"{indent}        <Pen Brush=\"{style.Stroke.ToHex()}\" Thickness=\"{style.StrokeWidth}\" LineCap=\"{style.StrokeCap}\" LineJoin=\"{style.StrokeJoin}\" MiterLimit=\"{style.StrokeMiter}\"/>");
                                sb.AppendLine($"{indent}    </GeometryDrawing.Pen>");
                                sb.AppendLine($"{indent}</GeometryDrawing>");
                            }
                            else if (style.IsStroked && !style.IsFilled)
                            {
                                sb.AppendLine($"{indent}<GeometryDrawing Geometry=\"{geometry}\">");
                                sb.AppendLine($"{indent}    <GeometryDrawing.Pen>");
                                sb.AppendLine($"{indent}        <Pen Brush=\"{style.Stroke.ToHex()}\" Thickness=\"{style.StrokeWidth}\" LineCap=\"{style.StrokeCap}\" LineJoin=\"{style.StrokeJoin}\" MiterLimit=\"{style.StrokeMiter}\"/>");
                                sb.AppendLine($"{indent}    </GeometryDrawing.Pen>");
                                sb.AppendLine($"{indent}</GeometryDrawing>");
                            }
                            else if (!style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"{indent}<GeometryDrawing Brush=\"{style.Fill.ToHex()}\" Geometry=\"{geometry}\" />");
                            }
                        }
                    }
                }

                return sb.ToString().TrimEnd(s_newLine);
            }
            return null;
        }

        public static string ConvertToDrawingGroup(IToolContext context, IContainerView containerView, string indent = "")
        {
            var geometryDrawing = ConvertToGeometryDrawing(context, containerView, indent + "    ");
            if (!string.IsNullOrEmpty(geometryDrawing))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"{indent}<DrawingGroup>");
                sb.AppendLine($"{geometryDrawing}");
                sb.AppendLine($"{indent}</DrawingGroup>");

                return sb.ToString().TrimEnd(s_newLine);
            }
            return null;
        }

        public static string ConvertToDrawingPresenter(IToolContext context, IContainerView containerView, string indent = "")
        {
            var drawingGroup = ConvertToDrawingGroup(context, containerView, indent + "    ");
            if (!string.IsNullOrEmpty(drawingGroup))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"{indent}<DrawingPresenter Width=\"{containerView.Width}\" Height=\"{containerView.Height}\" Stretch=\"Uniform\">");
                sb.AppendLine($"{drawingGroup}");
                sb.AppendLine($"{indent}</DrawingPresenter>");

                return sb.ToString().TrimEnd(s_newLine);
            }
            return null;
        }

        public static string ConvertToPath(IToolContext context, IContainerView containerView, string indent = "")
        {
            if (containerView.SelectionState?.Shapes != null && containerView.SelectionState?.Shapes.Count > 0)
            {
                var shapes = new List<IBaseShape>(containerView.SelectionState?.Shapes);
                var sb = new StringBuilder();

                foreach (var shape in shapes)
                {
                    if (shape is IPointShape)
                    {
                        continue;
                    }
                    var geometry = context.PathConverter?.ToSvgPathData(context, new[] { shape })?.TrimEnd(s_newLine);
                    if (geometry != null)
                    {
                        var style = context.StyleLibrary?.Get(shape.StyleId);
                        if (style != null)
                        {
                            if (style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"{indent}<Path Fill=\"{style.Fill.ToHex()}\" Stroke=\"{style.Stroke.ToHex()}\" StrokeThickness=\"{style.StrokeWidth}\" StrokeLineCap=\"{style.StrokeCap}\" StrokeJoin=\"{style.StrokeJoin}\" Data=\"{geometry}\"/>");
                            }
                            else if (style.IsStroked && !style.IsFilled)
                            {
                                sb.AppendLine($"{indent}<Path Stroke=\"{style.Stroke.ToHex()}\" StrokeThickness=\"{style.StrokeWidth}\" StrokeLineCap=\"{style.StrokeCap}\" StrokeJoin=\"{style.StrokeJoin}\" Data=\"{geometry}\"/>");
                            }
                            else if (!style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"{indent}<Path Fill=\"{style.Fill.ToHex()}\" Data=\"{geometry}\"/>");
                            }
                        }
                    }
                }

                return sb.ToString().TrimEnd(s_newLine);
            }
            return null;
        }

        public static string ConvertToCanvas(IToolContext context, IContainerView containerView, string indent = "")
        {
            var path = ConvertToPath(context, containerView, indent + "    ");
            if (!string.IsNullOrEmpty(path))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"{indent}<Canvas Width=\"{containerView.Width}\" Height=\"{containerView.Height}\">");
                sb.AppendLine($"{path}");
                sb.AppendLine($"{indent}</Canvas>");

                return sb.ToString().TrimEnd(s_newLine);
            }
            return null;
        }
    }
}
