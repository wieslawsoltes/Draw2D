using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Svg;

namespace SvgDemo
{
    class SvgDebug
    {
        internal static void ResetColor()
        {
            Console.ResetColor();
        }

        internal static void WriteLine(string value, ConsoleColor color = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }

        internal static void PrintPaintServer(SvgPaintServer paintServer, string attribute, string indent, string indentAttribute)
        {
            switch (paintServer)
            {
                case SvgColourServer colourServer:
                    {
                        string value = "";
                        if (colourServer == SvgPaintServer.None)
                        {
                            value = "None";
                        }
                        else if (colourServer == SvgColourServer.NotSet)
                        {
                            value = "NotSet";
                        }
                        else if (colourServer == SvgColourServer.Inherit)
                        {
                            value = "Inherit";
                        }
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={colourServer.ToString()} ({value})", ConsoleColor.DarkGray);
                    }
                    break;
                case SvgDeferredPaintServer deferredPaintServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={deferredPaintServer.GetType()}", ConsoleColor.DarkGray);
                    }
                    break;
                case SvgFallbackPaintServer fallbackPaintServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={fallbackPaintServer.GetType()}", ConsoleColor.DarkGray);
                    }
                    break;
                case SvgGradientServer gradientServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={gradientServer.GetType()}", ConsoleColor.DarkGray);
                    }
                    break;
                case SvgPatternServer patternServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={patternServer.GetType()}", ConsoleColor.DarkGray);
                    }
                    break;
                default:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={paintServer.GetType()}", ConsoleColor.DarkGray);
                    }
                    break;
            }
        }

        internal static void PrintElementAttributes(SvgElement element, string indent = "", string indentAttribute = "")
        {
            // Transforms

            if (element.Transforms.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<Transforms>", ConsoleColor.Gray);
                WriteLine($"{indent}{indentAttribute}[transform]", ConsoleColor.DarkGray);

                foreach (var transform in element.Transforms)
                {
                    WriteLine($"{indent}{indentAttribute}{transform}", ConsoleColor.DarkGray);
                }
            }

            // Attributes

            if (!string.IsNullOrEmpty(element.ID))
            {
                WriteLine($"{indent}{indentAttribute}[id]={element.ID}", ConsoleColor.DarkGray);
            }

            if (element.SpaceHandling != XmlSpaceHandling.@default && element.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indent}{indentAttribute}[space]={element.SpaceHandling}", ConsoleColor.DarkGray);
            }

            if (element.Color != null && element.Color != SvgColourServer.NotSet)
            {
                PrintPaintServer(element.Color, "color", indent, indentAttribute);
            }

            // Style

            if (element.Fill != null && element.Fill != SvgColourServer.NotSet)
            {
                PrintPaintServer(element.Fill, "fill", indent, indentAttribute);
            }

            if (element.Stroke != null)
            {
                PrintPaintServer(element.Stroke, "stroke", indent, indentAttribute);
            }

            if (element.FillRule != SvgFillRule.NonZero)
            {
                WriteLine($"{indent}{indentAttribute}[fill-rule]={element.FillRule}", ConsoleColor.DarkGray);
            }

            if (element.FillOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[fill-opacity]={element.FillOpacity}", ConsoleColor.DarkGray);
            }

            if (element.StrokeWidth != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-width]={element.StrokeWidth}", ConsoleColor.DarkGray);
            }

            if (element.StrokeLineCap != SvgStrokeLineCap.Butt)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-linecap]={element.StrokeLineCap}", ConsoleColor.DarkGray);
            }

            if (element.StrokeLineJoin != SvgStrokeLineJoin.Miter)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-linejoin]={element.StrokeLineJoin}", ConsoleColor.DarkGray);
            }

            if (element.StrokeMiterLimit != 4f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-miterlimit]={element.StrokeMiterLimit}", ConsoleColor.DarkGray);
            }

            if (element.StrokeDashArray != null)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-dasharray]={element.StrokeDashArray}", ConsoleColor.DarkGray);
            }

            if (element.StrokeDashOffset != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-dashoffset]={element.StrokeDashOffset}", ConsoleColor.DarkGray);
            }

            if (element.StrokeOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-opacity]={element.StrokeOpacity}", ConsoleColor.DarkGray);
            }

            if (element.StopColor != null)
            {
                PrintPaintServer(element.StopColor, "stop-color", indent, indentAttribute);
            }

            if (element.Opacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[opacity]={element.Opacity}", ConsoleColor.DarkGray);
            }

            if (element.ShapeRendering != SvgShapeRendering.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[shape-rendering]={element.ShapeRendering}", ConsoleColor.DarkGray);
            }

            if (element.TextAnchor != SvgTextAnchor.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-anchor]={element.TextAnchor}", ConsoleColor.DarkGray);
            }

            if (!string.IsNullOrEmpty(element.BaselineShift))
            {
                WriteLine($"{indent}{indentAttribute}[baseline-shift]={element.BaselineShift}", ConsoleColor.DarkGray);
            }

            if (!string.IsNullOrEmpty(element.FontFamily))
            {
                WriteLine($"{indent}{indentAttribute}[font-family]={element.FontFamily}", ConsoleColor.DarkGray);
            }

            if (element.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[font-size]={element.FontSize}", ConsoleColor.DarkGray);
            }

            if (element.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indent}{indentAttribute}[font-style]={element.FontStyle}", ConsoleColor.DarkGray);
            }

            if (element.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[font-variant]={element.FontVariant}", ConsoleColor.DarkGray);
            }

            if (element.TextDecoration != SvgTextDecoration.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-decoration]={element.TextDecoration}", ConsoleColor.DarkGray);
            }

            if (element.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[font-weight]={element.FontWeight}", ConsoleColor.DarkGray);
            }

            if (element.TextTransformation != SvgTextTransformation.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-transform]={element.TextTransformation}", ConsoleColor.DarkGray);
            }

            if (!string.IsNullOrEmpty(element.Font))
            {
                WriteLine($"{indent}{indentAttribute}[font]={element.Font}", ConsoleColor.DarkGray);
            }

            // CustomAttributes

            if (element.CustomAttributes.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<CustomAttributes>", ConsoleColor.Gray);

                foreach (var attribute in element.CustomAttributes)
                {
                    WriteLine($"{indent}{indentAttribute}[{attribute.Key}]={attribute.Value}", ConsoleColor.DarkGray);
                }
            }
        }

        internal static void PrintElement(SvgElement element, string indent = "", string indentAttribute = "")
        {
            // Attributes

            PrintElementAttributes(element, indent, indentAttribute);

            // Children

            if (element.Children.Count > 0)
            {
                WriteLine($"{indent}<Children>", ConsoleColor.Gray);

                foreach (var child in element.Children)
                {
                    WriteLine($"{indent}{child}", ConsoleColor.Blue);
                    PrintElement(child, indent + "    ", indentAttribute);
                }
            }

            // Nodes

            if (element.Nodes.Count > 0)
            {
                WriteLine($"{indent}<Nodes>", ConsoleColor.Gray);

                foreach (var node in element.Nodes)
                {
                    WriteLine($"{indent}{node.Content}", ConsoleColor.DarkGray);
                }
            }
        }

        internal static void PrintFragmentAttributes(SvgFragment fragment, string indent = "", string indentAttribute = "")
        {
            if (fragment.X != 0f)
            {
                WriteLine($"{indent}{indentAttribute}[x]={fragment.X}", ConsoleColor.DarkGray);
            }

            if (fragment.Y != 0f)
            {
                WriteLine($"{indent}{indentAttribute}[y]={fragment.Y}", ConsoleColor.DarkGray);
            }

            if (fragment.Width != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indent}{indentAttribute}[width]={fragment.Width}", ConsoleColor.DarkGray);
            }

            if (fragment.Height != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indent}{indentAttribute}[height]={fragment.Height}", ConsoleColor.DarkGray);
            }

            if (fragment.Overflow != SvgOverflow.Inherit && fragment.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indent}{indentAttribute}[overflow]={fragment.Overflow}", ConsoleColor.DarkGray);
            }

            if (fragment.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = fragment.ViewBox;
                WriteLine($"{indent}{indentAttribute}[viewBox]={viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", ConsoleColor.DarkGray);
            }

            if (fragment.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (fragment.AspectRatio.Align != @default.Align
                 || fragment.AspectRatio.Slice != @default.Slice
                 || fragment.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indent}{indentAttribute}[preserveAspectRatio]={fragment.AspectRatio}", ConsoleColor.DarkGray);
                }
            }

            if (fragment.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[font-size]={fragment.FontSize}", ConsoleColor.DarkGray);
            }

            if (!string.IsNullOrEmpty(fragment.FontFamily))
            {
                WriteLine($"{indent}{indentAttribute}[font-family]={fragment.FontFamily}", ConsoleColor.DarkGray);
            }

            if (fragment.SpaceHandling != XmlSpaceHandling.@default && fragment.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indent}{indentAttribute}[space]={fragment.SpaceHandling}", ConsoleColor.DarkGray);
            }
        }

        internal static void PrintFragment(SvgFragment fragment, string indent = "    ", string indentAttribute = "")
        {
            WriteLine($"{fragment}", ConsoleColor.Blue);
            PrintFragmentAttributes(fragment, indent, indentAttribute);
            PrintElement(fragment, indent, indentAttribute);
            ResetColor();
        }

        internal static void Run(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string path = args[i];
                WriteLine($"Path: {path}", ConsoleColor.DarkGray);

                var document = SvgDocument.Open<SvgDocument>(path, null);
                document.FlushStyles(true);
                PrintFragment(document);
            }
        }

        internal static void Error(Exception ex)
        {
            WriteLine($"{ex.Message}", ConsoleColor.Red);
            WriteLine($"{ex.StackTrace}", ConsoleColor.DarkGray);
            if (ex.InnerException != null)
            {
                Error(ex.InnerException);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SvgDebug.Run(args);
            }
            catch (Exception ex)
            {
                SvgDebug.Error(ex);
            }
        }
    }
}
