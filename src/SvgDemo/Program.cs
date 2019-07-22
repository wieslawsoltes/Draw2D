using System;
using Svg;

namespace SvgDemo
{
    class SvgDebug
    {
        internal static ConsoleColor ErrorColor = ConsoleColor.Red;
        internal static ConsoleColor ElementColor = ConsoleColor.Blue;
        internal static ConsoleColor GroupColor = ConsoleColor.Gray;
        internal static ConsoleColor AttributeColor = ConsoleColor.DarkRed;

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
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={colourServer.ToString()}", AttributeColor);
                    }
                    break;
                case SvgDeferredPaintServer deferredPaintServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={deferredPaintServer.GetType()}", AttributeColor);
                    }
                    break;
                case SvgFallbackPaintServer fallbackPaintServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={fallbackPaintServer.GetType()}", AttributeColor);
                    }
                    break;
                case SvgGradientServer gradientServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={gradientServer.GetType()}", AttributeColor);
                    }
                    break;
                case SvgPatternServer patternServer:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={patternServer.GetType()}", AttributeColor);
                    }
                    break;
                default:
                    {
                        WriteLine($"{indent}{indentAttribute}[{attribute}]={paintServer.GetType()}", AttributeColor);
                    }
                    break;
            }
        }

        internal static void PrintElementAttributes(SvgElement element, string indent = "", string indentAttribute = "")
        {
            // Transforms

            if (element.Transforms.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<Transforms>", GroupColor);
                WriteLine($"{indent}{indentAttribute}[transform]", AttributeColor);

                foreach (var transform in element.Transforms)
                {
                    WriteLine($"{indent}{indentAttribute}{transform}", AttributeColor);
                }
            }

            // Attributes

            if (!string.IsNullOrEmpty(element.ID))
            {
                WriteLine($"{indent}{indentAttribute}[id]={element.ID}", AttributeColor);
            }

            if (element.SpaceHandling != XmlSpaceHandling.@default && element.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indent}{indentAttribute}[space]={element.SpaceHandling}", AttributeColor);
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
                WriteLine($"{indent}{indentAttribute}[fill-rule]={element.FillRule}", AttributeColor);
            }

            if (element.FillOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[fill-opacity]={element.FillOpacity}", AttributeColor);
            }

            if (element.StrokeWidth != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-width]={element.StrokeWidth}", AttributeColor);
            }

            if (element.StrokeLineCap != SvgStrokeLineCap.Butt)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-linecap]={element.StrokeLineCap}", AttributeColor);
            }

            if (element.StrokeLineJoin != SvgStrokeLineJoin.Miter)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-linejoin]={element.StrokeLineJoin}", AttributeColor);
            }

            if (element.StrokeMiterLimit != 4f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-miterlimit]={element.StrokeMiterLimit}", AttributeColor);
            }

            if (element.StrokeDashArray != null)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-dasharray]={element.StrokeDashArray}", AttributeColor);
            }

            if (element.StrokeDashOffset != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-dashoffset]={element.StrokeDashOffset}", AttributeColor);
            }

            if (element.StrokeOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-opacity]={element.StrokeOpacity}", AttributeColor);
            }

            if (element.StopColor != null)
            {
                PrintPaintServer(element.StopColor, "stop-color", indent, indentAttribute);
            }

            if (element.Opacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[opacity]={element.Opacity}", AttributeColor);
            }

            if (element.ShapeRendering != SvgShapeRendering.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[shape-rendering]={element.ShapeRendering}", AttributeColor);
            }

            if (element.TextAnchor != SvgTextAnchor.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-anchor]={element.TextAnchor}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(element.BaselineShift))
            {
                WriteLine($"{indent}{indentAttribute}[baseline-shift]={element.BaselineShift}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(element.FontFamily))
            {
                WriteLine($"{indent}{indentAttribute}[font-family]={element.FontFamily}", AttributeColor);
            }

            if (element.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[font-size]={element.FontSize}", AttributeColor);
            }

            if (element.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indent}{indentAttribute}[font-style]={element.FontStyle}", AttributeColor);
            }

            if (element.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[font-variant]={element.FontVariant}", AttributeColor);
            }

            if (element.TextDecoration != SvgTextDecoration.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-decoration]={element.TextDecoration}", AttributeColor);
            }

            if (element.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[font-weight]={element.FontWeight}", AttributeColor);
            }

            if (element.TextTransformation != SvgTextTransformation.Inherit)
            {
                WriteLine($"{indent}{indentAttribute}[text-transform]={element.TextTransformation}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(element.Font))
            {
                WriteLine($"{indent}{indentAttribute}[font]={element.Font}", AttributeColor);
            }

            // CustomAttributes

            if (element.CustomAttributes.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<CustomAttributes>", GroupColor);

                foreach (var attribute in element.CustomAttributes)
                {
                    WriteLine($"{indent}{indentAttribute}[{attribute.Key}]={attribute.Value}", AttributeColor);
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
                WriteLine($"{indent}<Children>", GroupColor);

                foreach (var child in element.Children)
                {
                    WriteLine($"{indent}{child}", ElementColor);
                    PrintElement(child, indent + "    ", indentAttribute);
                }
            }

            // Nodes

            if (element.Nodes.Count > 0)
            {
                WriteLine($"{indent}<Nodes>", GroupColor);

                foreach (var node in element.Nodes)
                {
                    WriteLine($"{indent}{node.Content}", AttributeColor);
                }
            }
        }

        internal static void PrintFragmentAttributes(SvgFragment fragment, string indent = "", string indentAttribute = "")
        {
            if (fragment.X != 0f)
            {
                WriteLine($"{indent}{indentAttribute}[x]={fragment.X}", AttributeColor);
            }

            if (fragment.Y != 0f)
            {
                WriteLine($"{indent}{indentAttribute}[y]={fragment.Y}", AttributeColor);
            }

            if (fragment.Width != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indent}{indentAttribute}[width]={fragment.Width}", AttributeColor);
            }

            if (fragment.Height != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indent}{indentAttribute}[height]={fragment.Height}", AttributeColor);
            }

            if (fragment.Overflow != SvgOverflow.Inherit && fragment.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indent}{indentAttribute}[overflow]={fragment.Overflow}", AttributeColor);
            }

            if (fragment.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = fragment.ViewBox;
                WriteLine($"{indent}{indentAttribute}[viewBox]={viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", AttributeColor);
            }

            if (fragment.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (fragment.AspectRatio.Align != @default.Align
                 || fragment.AspectRatio.Slice != @default.Slice
                 || fragment.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indent}{indentAttribute}[preserveAspectRatio]={fragment.AspectRatio}", AttributeColor);
                }
            }

            if (fragment.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indent}{indentAttribute}[font-size]={fragment.FontSize}", AttributeColor);
            }

            if (!string.IsNullOrEmpty(fragment.FontFamily))
            {
                WriteLine($"{indent}{indentAttribute}[font-family]={fragment.FontFamily}", AttributeColor);
            }

            if (fragment.SpaceHandling != XmlSpaceHandling.@default && fragment.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indent}{indentAttribute}[space]={fragment.SpaceHandling}", AttributeColor);
            }
        }

        internal static void PrintFragment(SvgFragment fragment, string indent = "    ", string indentAttribute = "")
        {
            WriteLine($"{fragment}", ElementColor);
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
                WriteLine($"Path: {path}", AttributeColor);

                var document = SvgDocument.Open<SvgDocument>(path, null);
                document.FlushStyles(true);
                PrintFragment(document);
            }
        }

        internal static void Error(Exception ex)
        {
            WriteLine($"{ex.Message}", ErrorColor);
            WriteLine($"{ex.StackTrace}", AttributeColor);
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
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                SvgDebug.Error(ex);
            }
        }
    }
}
