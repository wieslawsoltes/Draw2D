using System;
using Svg;
using Svg.Document_Structure;
using Svg.FilterEffects;
using Svg.Pathing;

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

        internal static void PrintPaintServer(SvgPaintServer paintServer, string attribute, string indentLine, string indentAttribute)
        {
            switch (paintServer)
            {
                case SvgColourServer colourServer:
                    {
                        WriteLine($"{indentLine}{indentAttribute}[{attribute}]={colourServer.ToString()}", s_attributeColor);
                    }
                    break;
                case SvgDeferredPaintServer deferredPaintServer:
                    {
                        WriteLine($"{indentLine}{indentAttribute}[{attribute}]={deferredPaintServer.GetType()}", s_attributeColor);
                    }
                    break;
                case SvgFallbackPaintServer fallbackPaintServer:
                    {
                        WriteLine($"{indentLine}{indentAttribute}[{attribute}]={fallbackPaintServer.GetType()}", s_attributeColor);
                    }
                    break;
                case SvgGradientServer gradientServer:
                    {
                        WriteLine($"{indentLine}{indentAttribute}[{attribute}]={gradientServer.GetType()}", s_attributeColor);
                    }
                    break;
                case SvgPatternServer patternServer:
                    {
                        WriteLine($"{indentLine}{indentAttribute}[{attribute}]={patternServer.GetType()}", s_attributeColor);
                    }
                    break;
                default:
                    {
                        WriteLine($"{indentLine}{indentAttribute}[{attribute}]={paintServer.GetType()}", s_attributeColor);
                    }
                    break;
            }
        }

        internal static void PrintElementAttributes(SvgElement element, string indentLine, string indentAttribute)
        {
            // Transforms

            if (element.Transforms.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}<Transforms>", s_groupColor);
                WriteLine($"{indentLine}{indentAttribute}[transform]", s_attributeColor);

                foreach (var transform in element.Transforms)
                {
                    WriteLine($"{indentLine}{indentAttribute}{transform}", s_attributeColor);
                }
            }

            // Attributes

            if (!string.IsNullOrEmpty(element.ID))
            {
                WriteLine($"{indentLine}{indentAttribute}[id]={element.ID}", s_attributeColor);
            }

            if (element.SpaceHandling != XmlSpaceHandling.@default && element.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[space]={element.SpaceHandling}", s_attributeColor);
            }

            if (element.Color != null && element.Color != SvgColourServer.NotSet)
            {
                PrintPaintServer(element.Color, "color", indentLine, indentAttribute);
            }

            // Style

            if (element.Fill != null && element.Fill != SvgColourServer.NotSet)
            {
                PrintPaintServer(element.Fill, "fill", indentLine, indentAttribute);
            }

            if (element.Stroke != null)
            {
                PrintPaintServer(element.Stroke, "stroke", indentLine, indentAttribute);
            }

            if (element.FillRule != SvgFillRule.NonZero)
            {
                WriteLine($"{indentLine}{indentAttribute}[fill-rule]={element.FillRule}", s_attributeColor);
            }

            if (element.FillOpacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[fill-opacity]={element.FillOpacity}", s_attributeColor);
            }

            if (element.StrokeWidth != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-width]={element.StrokeWidth}", s_attributeColor);
            }

            if (element.StrokeLineCap != SvgStrokeLineCap.Butt)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-linecap]={element.StrokeLineCap}", s_attributeColor);
            }

            if (element.StrokeLineJoin != SvgStrokeLineJoin.Miter)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-linejoin]={element.StrokeLineJoin}", s_attributeColor);
            }

            if (element.StrokeMiterLimit != 4f)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-miterlimit]={element.StrokeMiterLimit}", s_attributeColor);
            }

            if (element.StrokeDashArray != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-dasharray]={element.StrokeDashArray}", s_attributeColor);
            }

            if (element.StrokeDashOffset != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-dashoffset]={element.StrokeDashOffset}", s_attributeColor);
            }

            if (element.StrokeOpacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-opacity]={element.StrokeOpacity}", s_attributeColor);
            }

            if (element.StopColor != null)
            {
                PrintPaintServer(element.StopColor, "stop-color", indentLine, indentAttribute);
            }

            if (element.Opacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[opacity]={element.Opacity}", s_attributeColor);
            }

            if (element.ShapeRendering != SvgShapeRendering.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[shape-rendering]={element.ShapeRendering}", s_attributeColor);
            }

            if (element.TextAnchor != SvgTextAnchor.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[text-anchor]={element.TextAnchor}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(element.BaselineShift))
            {
                WriteLine($"{indentLine}{indentAttribute}[baseline-shift]={element.BaselineShift}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(element.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}[font-family]={element.FontFamily}", s_attributeColor);
            }

            if (element.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-size]={element.FontSize}", s_attributeColor);
            }

            if (element.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-style]={element.FontStyle}", s_attributeColor);
            }

            if (element.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-variant]={element.FontVariant}", s_attributeColor);
            }

            if (element.TextDecoration != SvgTextDecoration.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[text-decoration]={element.TextDecoration}", s_attributeColor);
            }

            if (element.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-weight]={element.FontWeight}", s_attributeColor);
            }

            if (element.TextTransformation != SvgTextTransformation.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[text-transform]={element.TextTransformation}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(element.Font))
            {
                WriteLine($"{indentLine}{indentAttribute}[font]={element.Font}", s_attributeColor);
            }

            // CustomAttributes

            if (element.CustomAttributes.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}<CustomAttributes>", s_groupColor);

                foreach (var attribute in element.CustomAttributes)
                {
                    WriteLine($"{indentLine}{indentAttribute}[{attribute.Key}]={attribute.Value}", s_attributeColor);
                }
            }
        }

        internal static void PrintElementChildren(SvgElement element, string indentLine, string indentAttribute)
        {
            if (element.Children.Count > 0)
            {
                WriteLine($"{indentLine}<Children>", s_groupColor);

                foreach (var child in element.Children)
                {
                    WriteLine($"{indentLine}{child}", s_elementColor);
                    PrintElement(child, indentLine + "    ", indentAttribute);
                }
            }
        }

        internal static void PrintElementNodes(SvgElement element, string indentLine, string indentAttribute)
        {
            if (element.Nodes.Count > 0)
            {
                WriteLine($"{indentLine}<Nodes>", s_groupColor);

                foreach (var node in element.Nodes)
                {
                    WriteLine($"{indentLine}{node.Content}", s_attributeColor);
                }
            }
        }

        internal static void PrintSvgPathAttributes(SvgPath svgPath, string indentLine, string indentAttribute)
        {
            if (svgPath.PathData != null)
            {
                /// ----------------------------------------------------------------------------------------
                /// The <see cref="SvgPathSegment"/> object graph.
                /// ----------------------------------------------------------------------------------------
                /// +---abstract class <see cref="SvgPathSegment"/>
                ///     +---class <see cref="SvgArcSegment"/>
                ///     +---class <see cref="SvgClosePathSegment"/>
                ///     +---class <see cref="SvgCubicCurveSegment"/>
                ///     +---class <see cref="SvgLineSegment"/>
                ///     +---class <see cref="SvgMoveToSegment"/>
                ///     \---class <see cref="SvgQuadraticCurveSegment"/>

                WriteLine($"{indentLine}{indentAttribute}[d=", s_attributeColor);
                foreach (var segment in svgPath.PathData)
                {
                    WriteLine($"{indentLine}{indentAttribute}   {segment}", s_attributeColor);
                }
                WriteLine($"{indentLine}{indentAttribute}]", s_attributeColor);
            }

            if (svgPath.PathLength != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[pathLength]={svgPath.PathLength}", s_attributeColor);
            }
        }

        internal static void PrintElement(SvgElement element, string indentLine, string indentAttribute)
        {
            PrintElementAttributes(element, indentLine, indentAttribute);

            /// The <see cref="SvgElement"/> object graph.
            /// +---abstract class <see cref="SvgElement"/>
            /// |   +---class <see cref="SvgClipPath"/>
            /// |   +---class <see cref="SvgFragment"/>
            /// |       \---class <see cref="SvgDocument"/>
            /// |   +---class <see cref="SvgMask"/>
            /// |   +---class <see cref="SvgDefinitionList"/>
            /// |   +---class <see cref="SvgDescription"/>
            /// |   +---class <see cref="SvgDocumentMetadata"/>
            /// |   +---class <see cref="SvgTitle"/>
            /// |   +---class <see cref="SvgMergeNode"/>
            /// |   +---class <see cref="SvgFilter"/>
            /// |   +---class <see cref="NonSvgElement"/>
            /// |   +---class <see cref="SvgGradientStop"/>
            /// |   +---class <see cref="SvgUnknownElement"/>
            /// |   +---class <see cref="SvgFont"/>
            /// |   +---class <see cref="SvgFontFace"/>
            /// |   +---class <see cref="SvgFontFaceSrc"/>
            /// |   +---class <see cref="SvgFontFaceUri"/>
            /// |   \---abstract class <see cref="SvgVisualElement"/>
            /// |       +---class <see cref="SvgImage"/>
            /// |       +---class <see cref="SvgSwitch"/>
            /// |       +---class <see cref="SvgSymbol"/>
            /// |       +---class <see cref="SvgUse"/>
            /// |       +---class <see cref="SvgForeignObject"/>
            /// |       +---abstract class <see cref="SvgPathBasedElement"/>
            /// |       |   +---<see cref="SvgCircle"/>
            /// |       |   +---<see cref="SvgEllipse"/>
            /// |       |   +---<see cref="SvgRectangle"/>
            /// |       |   +---<see cref="SvgMarker"/>
            /// |       |   +---<see cref="SvgGlyph"/>
            /// |       |   +---abstract class <see cref="SvgMarkerElement"/>
            /// |       |       +---class <see cref="SvgGroup"/>
            /// |       |       +---class <see cref="SvgLine"/>
            /// |       |       +---class <see cref="SvgPath"/>
            /// |       |       \---class <see cref="SvgPolygon"/>
            /// |       \---abstract class <see cref="SvgTextBase"/>
            /// |           +----class <see cref="SvgText"/>
            /// |           +----class <see cref="SvgTextPath"/>
            /// |           +----class <see cref="SvgTextRef"/>
            /// |           \----class <see cref="SvgTextSpan"/>
            /// +---abstract class <see cref="SvgFilterPrimitive"/>
            /// |   +---class <see cref="SvgColourMatrix"/>
            /// |   +---class <see cref="SvgGaussianBlur"/>
            /// |   +---class <see cref="SvgMerge"/>
            /// |   \---class <see cref="SvgOffset"/>
            /// +---abstract class <see cref="SvgPaintServer"/>
            /// |   +---class <see cref="SvgColourServer"/>
            /// |   +---class <see cref="SvgDeferredPaintServer"/>
            /// |   +---class <see cref="SvgFallbackPaintServer"/>
            /// |   \---class <see cref="SvgPatternServer"/>
            /// |       \---abstract class <see cref="SvgGradientServer"/>
            /// |           +---class <see cref="SvgLinearGradientServer"/>
            /// |           \---class <see cref="SvgRadialGradientServer"/>
            /// \---abstract class <see cref="SvgKern"/>
            ///     +---class <see cref="SvgVerticalKern"/>
            ///     \---class <see cref="SvgHorizontalKern"/>

            switch (element)
            {
                case SvgCircle svgCircle:
                    // TODO:
                    break;
                case SvgEllipse svgEllipse:
                    // TODO:
                    break;
                case SvgRectangle svgRectangle:
                    // TODO:
                    break;
                case SvgMarker svgMarker:
                    // TODO:
                    break;
                case SvgGlyph svgGlyph:
                    // TODO:
                    break;
                case SvgGroup svgGroup:
                    // TODO:
                    break;
                case SvgLine svgLine:
                    // TODO:
                    break;
                case SvgPath svgPath:
                    PrintSvgPathAttributes(svgPath, indentLine, indentAttribute);
                    break;
                case SvgPolygon svgPolygon:
                    // TODO:
                    break;
                case SvgText svgText:
                    // TODO:
                    break;
                case SvgTextPath svgTextPath:
                    // TODO:
                    break;
                case SvgTextRef svgTextRef:
                    // TODO:
                    break;
                case SvgTextSpan svgTextSpan:
                    // TODO:
                    break;
                // TODO:
                default:
                    break;
            }

            PrintElementChildren(element, indentLine, indentAttribute);
            PrintElementNodes(element, indentLine, indentAttribute);
        }

        internal static void PrintFragmentAttributes(SvgFragment fragment, string indentLine, string indentAttribute)
        {
            if (fragment.X != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[x]={fragment.X}", s_attributeColor);
            }

            if (fragment.Y != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[y]={fragment.Y}", s_attributeColor);
            }

            if (fragment.Width != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indentLine}{indentAttribute}[width]={fragment.Width}", s_attributeColor);
            }

            if (fragment.Height != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indentLine}{indentAttribute}[height]={fragment.Height}", s_attributeColor);
            }

            if (fragment.Overflow != SvgOverflow.Inherit && fragment.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indentLine}{indentAttribute}[overflow]={fragment.Overflow}", s_attributeColor);
            }

            if (fragment.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = fragment.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}[viewBox]={viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", s_attributeColor);
            }

            if (fragment.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (fragment.AspectRatio.Align != @default.Align
                 || fragment.AspectRatio.Slice != @default.Slice
                 || fragment.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}[preserveAspectRatio]={fragment.AspectRatio}", s_attributeColor);
                }
            }

            if (fragment.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-size]={fragment.FontSize}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(fragment.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}[font-family]={fragment.FontFamily}", s_attributeColor);
            }

            if (fragment.SpaceHandling != XmlSpaceHandling.@default && fragment.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[space]={fragment.SpaceHandling}", s_attributeColor);
            }
        }

        internal static void PrintFragment(SvgFragment fragment, string indentLine, string indentAttribute)
        {
            WriteLine($"{fragment}", s_elementColor);
            PrintFragmentAttributes(fragment, indentLine, indentAttribute);
            PrintElement(fragment, indentLine, indentAttribute);
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
                PrintFragment(document, "    ", "");
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
