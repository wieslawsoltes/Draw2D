using System;
using Svg;

namespace SvgDemo
{
    class SvgDebug
    {
        internal static ConsoleColor s_errorColor = ConsoleColor.Yellow;
        internal static ConsoleColor s_elementColor = ConsoleColor.Red;
        internal static ConsoleColor s_groupColor = ConsoleColor.White;
        internal static ConsoleColor s_attributeColor = ConsoleColor.Blue;

        internal static void ResetColor()
        {
            Console.ResetColor();
        }

        internal static void WriteLine(string value, ConsoleColor color)
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
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={colourServer.ToString()}", s_attributeColor);
                    }
                    break;
                case SvgDeferredPaintServer deferredPaintServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={deferredPaintServer.GetType()}", s_attributeColor);
                    }
                    break;
                case SvgFallbackPaintServer fallbackPaintServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={fallbackPaintServer.GetType()}", s_attributeColor);
                    }
                    break;
                case SvgGradientServer gradientServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={gradientServer.GetType()}", s_attributeColor);
                    }
                    break;
                case SvgPatternServer patternServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={patternServer.GetType()}", s_attributeColor);
                    }
                    break;
                default:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={paintServer.GetType()}", s_attributeColor);
                    }
                    break;
            }
        }

        internal static void PrintElementAttributes(SvgElement element, string indent = "", string indentAttribute = "")
        {
            // Transforms

            if (element.Transforms.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<Transforms>", s_groupColor);
                WriteLine($"{indent}{indentAttribute}[transform]", s_attributeColor);

                foreach (var transform in element.Transforms)
                {
                    WriteLine($"{indent}{indentAttribute}{transform}", s_attributeColor);
                }
            }

            // Attributes

            if (!string.IsNullOrEmpty(element.ID))
            {
                WriteLine($"{indent}{indentAttribute}[id]={element.ID}", s_attributeColor);
            }

            if (element.SpaceHandling != XmlSpaceHandling.@default && element.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indent}{indentAttribute}[space]={element.SpaceHandling}", s_attributeColor);
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
                WriteLine($"{indent}{indentAttribute}[fill-rule]={element.FillRule}", s_attributeColor);
            }

            if (element.FillOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[fill-opacity]={element.FillOpacity}", s_attributeColor);
            }

            if (element.StrokeWidth != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-width]={element.StrokeWidth}", s_attributeColor);
            }

            if (element.StrokeLineCap != SvgStrokeLineCap.Butt)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-linecap]={element.StrokeLineCap}", s_attributeColor);
            }

            if (element.StrokeLineJoin != SvgStrokeLineJoin.Miter)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-linejoin]={element.StrokeLineJoin}", s_attributeColor);
            }

            if (element.StrokeMiterLimit != 4f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-miterlimit]={element.StrokeMiterLimit}", s_attributeColor);
            }

            if (element.StrokeDashArray != null)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-dasharray]={element.StrokeDashArray}", s_attributeColor);
            }

            if (element.StrokeDashOffset != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-dashoffset]={element.StrokeDashOffset}", s_attributeColor);
            }

            if (element.StrokeOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-opacity]={element.StrokeOpacity}", s_attributeColor);
            }

            if (element.StopColor != null)
            {
                PrintPaintServer(element.StopColor, "stop-color", indent, indentAttribute);
            }

            if (element.Opacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[opacity]={element.Opacity}", s_attributeColor);
            }

            if (element.ShapeRendering != SvgShapeRendering.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[shape-rendering]={element.ShapeRendering}", s_attributeColor);
            }

            if (element.TextAnchor != SvgTextAnchor.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-anchor]={element.TextAnchor}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(element.BaselineShift))
            {
                WriteLine($"{indent}{indentAttribute}[baseline-shift]={element.BaselineShift}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(element.FontFamily))
            {
                WriteLine($"{indent}{indentAttribute}[font-family]={element.FontFamily}", s_attributeColor);
            }

            if (element.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[font-size]={element.FontSize}", s_attributeColor);
            }

            if (element.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indent}{indentAttribute}[font-style]={element.FontStyle}", s_attributeColor);
            }

            if (element.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[font-variant]={element.FontVariant}", s_attributeColor);
            }

            if (element.TextDecoration != SvgTextDecoration.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-decoration]={element.TextDecoration}", s_attributeColor);
            }

            if (element.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[font-weight]={element.FontWeight}", s_attributeColor);
            }

            if (element.TextTransformation != SvgTextTransformation.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-transform]={element.TextTransformation}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(element.Font))
            {
                WriteLine($"{indent}{indentAttribute}[font]={element.Font}", s_attributeColor);
            }

            // CustomAttributes

            if (element.CustomAttributes.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<CustomAttributes>", s_groupColor);

                foreach (var attribute in element.CustomAttributes)
                {
                    WriteLine($"{indent}{indentAttribute}[{attribute.Key}]={attribute.Value}", s_attributeColor);
                }
            }
        }

        internal static void PrintElementChildren(SvgElement element, string indent, string indentAttribute)
        {
            if (element.Children.Count > 0)
            {
                WriteLine($"{indent}<Children>", s_groupColor);

                foreach (var child in element.Children)
                {
                    WriteLine($"{indent}{child}", s_elementColor);
                    PrintElement(child, indent + "    ", indentAttribute);
                }
            }
        }

        internal static void PrintElementNodes(SvgElement element, string indent)
        {
            if (element.Nodes.Count > 0)
            {
                WriteLine($"{indent}<Nodes>", s_groupColor);

                foreach (var node in element.Nodes)
                {
                    WriteLine($"{indent}{node.Content}", s_attributeColor);
                }
            }
        }

        internal static void PrintFragmentAttributes(SvgFragment fragment, string indent = "", string indentAttribute = "")
        {
            if (fragment.X != 0f)
            {
                WriteLine($"{indent}{indentAttribute}[x]={fragment.X}", s_attributeColor);
            }

            if (fragment.Y != 0f)
            {
                WriteLine($"{indent}{indentAttribute}[y]={fragment.Y}", s_attributeColor);
            }

            if (fragment.Width != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indent}{indentAttribute}[width]={fragment.Width}", s_attributeColor);
            }

            if (fragment.Height != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indent}{indentAttribute}[height]={fragment.Height}", s_attributeColor);
            }

            if (fragment.Overflow != SvgOverflow.Inherit && fragment.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indent}{indentAttribute}[overflow]={fragment.Overflow}", s_attributeColor);
            }

            if (fragment.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = fragment.ViewBox;
                WriteLine($"{indent}{indentAttribute}[viewBox]={viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", s_attributeColor);
            }

            if (fragment.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (fragment.AspectRatio.Align != @default.Align
                 || fragment.AspectRatio.Slice != @default.Slice
                 || fragment.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indent}{indentAttribute}[preserveAspectRatio]={fragment.AspectRatio}", s_attributeColor);
                }
            }

            if (fragment.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[font-size]={fragment.FontSize}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(fragment.FontFamily))
            {
                WriteLine($"{indent}{indentAttribute}[font-family]={fragment.FontFamily}", s_attributeColor);
            }

            if (fragment.SpaceHandling != XmlSpaceHandling.@default && fragment.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indent}{indentAttribute}[space]={fragment.SpaceHandling}", s_attributeColor);
            }
        }

        internal static void PrintElement(SvgElement element, string indent = "", string indentAttribute = "")
        {
            PrintElementAttributes(element, indent, indentAttribute);
            PrintElementChildren(element, indent, indentAttribute);
            PrintElementNodes(element, indent);
        }

        internal static void PrintFragment(SvgFragment fragment, string indent = "    ", string indentAttribute = "")
        {
            WriteLine($"{fragment}", s_elementColor);
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
                WriteLine($"Path: {path}", s_groupColor);

                var document = SvgDocument.Open<SvgDocument>(path, null);
                document.FlushStyles(true);
                PrintFragment(document);
            }
        }

        internal static void Error(Exception ex)
        {
            WriteLine($"{ex.Message}", s_errorColor);
            WriteLine($"{ex.StackTrace}", s_attributeColor);
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
