﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Text;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Export
{
    public class AvaloniaXamlConverter : IAvaloniaXamlConverter
    {
        internal static char[] NewLine = Environment.NewLine.ToCharArray();

        public void ConvertToGeometryDrawing(IToolContext context, IContainerView containerView, StringBuilder sb, string indent)
        {
            if (containerView.SelectionState?.Shapes != null && containerView.SelectionState?.Shapes.Count > 0)
            {
                var shapes = new List<IBaseShape>(containerView.SelectionState?.Shapes);

                foreach (var shape in shapes)
                {
                    if (shape is IPointShape)
                    {
                        continue;
                    }

                    var style = context.StyleLibrary?.Get(shape.StyleId);
                    if (style == null)
                    {
                        continue;
                    }

                    var geometry = context.PathConverter?.ToSvgPathData(context, new[] { shape })?.TrimEnd(NewLine);
                    if (geometry == null)
                    {
                        continue;
                    }

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

        public void ConvertToDrawingGroup(IToolContext context, IContainerView containerView, StringBuilder sb, string indent)
        {
            sb.AppendLine($"{indent}<DrawingGroup>");
            ConvertToGeometryDrawing(context, containerView, sb, indent + "    ");
            sb.AppendLine($"{indent}</DrawingGroup>");
        }

        public void ConvertToDrawingPresenter(IToolContext context, IContainerView containerView, StringBuilder sb, string indent)
        {
            sb.AppendLine($"{indent}<DrawingPresenter Width=\"{containerView.Width}\" Height=\"{containerView.Height}\" Stretch=\"Uniform\">");
            ConvertToDrawingGroup(context, containerView, sb, indent + "    ");
            sb.AppendLine($"{indent}</DrawingPresenter>");
        }

        public void ConvertToPath(IToolContext context, IContainerView containerView, StringBuilder sb, string indent)
        {
            if (containerView.SelectionState?.Shapes != null && containerView.SelectionState?.Shapes.Count > 0)
            {
                var shapes = new List<IBaseShape>(containerView.SelectionState?.Shapes);

                foreach (var shape in shapes)
                {
                    if (shape is IPointShape)
                    {
                        continue;
                    }

                    var style = context.StyleLibrary?.Get(shape.StyleId);
                    if (style == null)
                    {
                        continue;
                    }

                    var geometry = context.PathConverter?.ToSvgPathData(context, new[] { shape })?.TrimEnd(NewLine);
                    if (geometry == null)
                    {
                        continue;
                    }

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

        public void ConvertToCanvas(IToolContext context, IContainerView containerView, StringBuilder sb, string indent)
        {
            sb.AppendLine($"{indent}<Canvas Width=\"{containerView.Width}\" Height=\"{containerView.Height}\">");
            ConvertToPath(context, containerView, sb, indent + "    ");
            sb.AppendLine($"{indent}</Canvas>");
        }
    }
}