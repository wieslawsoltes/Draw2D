// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Export
{
    public static class AvaloniaXamlConverter
    {
        private static char[] NewLine() => Environment.NewLine.ToCharArray();

        public static string FormatXml(string xml)
        {
            var sb = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = false;

            using (var writer = XmlWriter.Create(sb, settings))
            {
                element.Save(writer);
            }

            return sb.ToString();
        }

        public static string ConvertToGeometryDrawing(IToolContext context, IContainerView containerView)
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
                    var geometry = context.PathConverter?.ToSvgPathData(context, new[] { shape })?.TrimEnd(NewLine());
                    if (geometry != null)
                    {
                        var style = context.StyleLibrary?.Get(shape.StyleId);
                        if (style != null)
                        {
                            if (style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"<GeometryDrawing Brush=\"{style.Fill.ToHex()}\" Geometry=\"{geometry}\">");
                                sb.AppendLine($"<GeometryDrawing.Pen>");
                                sb.AppendLine($"<Pen Brush=\"{style.Stroke.ToHex()}\" Thickness=\"{style.StrokeWidth}\" LineCap=\"{style.StrokeCap}\" LineJoin=\"{style.StrokeJoin}\" MiterLimit=\"{style.StrokeMiter}\"/>");
                                sb.AppendLine($"</GeometryDrawing.Pen>");
                                sb.AppendLine($"</GeometryDrawing>");
                            }
                            else if (style.IsStroked && !style.IsFilled)
                            {
                                sb.AppendLine($"<GeometryDrawing Geometry=\"{geometry}\">");
                                sb.AppendLine($"<GeometryDrawing.Pen>");
                                sb.AppendLine($"<Pen Brush=\"{style.Stroke.ToHex()}\" Thickness=\"{style.StrokeWidth}\" LineCap=\"{style.StrokeCap}\" LineJoin=\"{style.StrokeJoin}\" MiterLimit=\"{style.StrokeMiter}\"/>");
                                sb.AppendLine($"</GeometryDrawing.Pen>");
                                sb.AppendLine($"</GeometryDrawing>");
                            }
                            else if (!style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"<GeometryDrawing Brush=\"{style.Fill.ToHex()}\" Geometry=\"{geometry}\" />");
                            }
                        }
                    }
                }

                return sb.ToString().TrimEnd(NewLine());
            }
            return null;
        }

        public static string ConvertToDrawingGroup(IToolContext context, IContainerView containerView)
        {
            var geometryDrawing = ConvertToGeometryDrawing(context, containerView);
            if (!string.IsNullOrEmpty(geometryDrawing))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"<DrawingGroup>");
                sb.AppendLine($"{geometryDrawing}");
                sb.AppendLine($"</DrawingGroup>");

                return sb.ToString().TrimEnd(NewLine());
            }
            return null;
        }

        public static string ConvertToDrawingPresenter(IToolContext context, IContainerView containerView)
        {
            var drawingGroup = ConvertToDrawingGroup(context, containerView);
            if (!string.IsNullOrEmpty(drawingGroup))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"<DrawingPresenter Width=\"{containerView.Width}\" Height=\"{containerView.Height}\" Stretch=\"Uniform\">");
                sb.AppendLine($"{drawingGroup}");
                sb.AppendLine($"</DrawingPresenter>");

                return sb.ToString().TrimEnd(NewLine());
            }
            return null;
        }

        public static string ConvertToPath(IToolContext context, IContainerView containerView)
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
                    var geometry = context.PathConverter?.ToSvgPathData(context, new[] { shape })?.TrimEnd(NewLine());
                    if (geometry != null)
                    {
                        var style = context.StyleLibrary?.Get(shape.StyleId);
                        if (style != null)
                        {
                            if (style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"<Path Fill=\"{style.Fill.ToHex()}\" Stroke=\"{style.Stroke.ToHex()}\" StrokeThickness=\"{style.StrokeWidth}\" StrokeLineCap=\"{style.StrokeCap}\" StrokeJoin=\"{style.StrokeJoin}\" Data=\"{geometry}\"/>");
                            }
                            else if (style.IsStroked && !style.IsFilled)
                            {
                                sb.AppendLine($"<Path Stroke=\"{style.Stroke.ToHex()}\" StrokeThickness=\"{style.StrokeWidth}\" StrokeLineCap=\"{style.StrokeCap}\" StrokeJoin=\"{style.StrokeJoin}\" Data=\"{geometry}\"/>");
                            }
                            else if (!style.IsStroked && style.IsFilled)
                            {
                                sb.AppendLine($"<Path Fill=\"{style.Fill.ToHex()}\" Data=\"{geometry}\"/>");
                            }
                        }
                    }
                }

                return sb.ToString().TrimEnd(NewLine());
            }
            return null;
        }

        public static string ConvertToCanvas(IToolContext context, IContainerView containerView)
        {
            var path = ConvertToPath(context, containerView);
            if (!string.IsNullOrEmpty(path))
            {
                var sb = new StringBuilder();

                sb.AppendLine($"<Canvas Width=\"{containerView.Width}\" Height=\"{containerView.Height}\">");
                sb.AppendLine($"{path}");
                sb.AppendLine($"</Canvas>");

                return sb.ToString().TrimEnd(NewLine());
            }
            return null;
        }
    }
}
