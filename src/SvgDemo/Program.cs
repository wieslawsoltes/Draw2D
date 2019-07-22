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

        internal static void PrintSvgPaintServerServer(SvgPaintServer paintServer, string attribute, string indentLine, string indentAttribute)
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

        internal static void PrintSvgElementAttributes(SvgElement element, string indentLine, string indentAttribute)
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
                PrintSvgPaintServerServer(element.Color, "color", indentLine, indentAttribute);
            }

            // Style

            if (element.Fill != null && element.Fill != SvgColourServer.NotSet)
            {
                PrintSvgPaintServerServer(element.Fill, "fill", indentLine, indentAttribute);
            }

            if (element.Stroke != null)
            {
                PrintSvgPaintServerServer(element.Stroke, "stroke", indentLine, indentAttribute);
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
                PrintSvgPaintServerServer(element.StopColor, "stop-color", indentLine, indentAttribute);
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

        internal static void PrintSvgElementChildren(SvgElement element, string indentLine, string indentAttribute)
        {
            if (element.Children.Count > 0)
            {
                WriteLine($"{indentLine}<Children>", s_groupColor);

                foreach (var child in element.Children)
                {
                    WriteLine($"{indentLine}{child}", s_elementColor);
                    PrintSvgElementElement(child, indentLine + "    ", indentAttribute);
                }
            }
        }

        internal static void PrintSvgElementNodes(SvgElement element, string indentLine, string indentAttribute)
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

        internal static void PrintAttributes(SvgClipPath svgClipPath, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFragment svgFragment, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgMask svgMask, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgDefinitionList svgDefinitionList, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgDescription svgDescription, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgDocumentMetadata svgDocumentMetadata, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgTitle svgTitle, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgMergeNode svgMergeNode, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFilter svgFilter, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(NonSvgElement nonSvgElement, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgGradientStop svgGradientStop, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgUnknownElement svgUnknownElement, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFont svgFont, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFontFace svgFontFace, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFontFaceSrc svgFontFaceSrc, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFontFaceUri svgFontFaceUri, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgImage svgImage, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgSwitch svgSwitch, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgSymbol svgSymbol, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgUse svgUse, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgForeignObject svgForeignObject, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgCircle svgCircle, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgEllipse svgEllipse, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgRectangle svgRectangle, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgMarker svgMarker, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgGlyph svgGlyph, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgGroup svgGroup, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgLine svgLine, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgPath svgPath, string indentLine, string indentAttribute)
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

        internal static void PrintAttributes(SvgPolygon svgPolygon, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgText svgText, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgTextPath svgTextPath, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgTextRef svgTextRef, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgTextSpan svgTextSpan, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgColourMatrix svgColourMatrix, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgGaussianBlur svgGaussianBlur, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgMerge svgMerge, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgOffset svgOffset, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgColourServer svgColourServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgDeferredPaintServer svgDeferredPaintServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgFallbackPaintServer svgFallbackPaintServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgPatternServer svgPatternServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgLinearGradientServer svgLinearGradientServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgRadialGradientServer svgRadialGradientServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgVerticalKern svgVerticalKern, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintAttributes(SvgHorizontalKern svgHorizontalKern, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        internal static void PrintSvgElementElement(SvgElement element, string indentLine, string indentAttribute)
        {
            PrintSvgElementAttributes(element, indentLine, indentAttribute);

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
                case SvgClipPath svgClipPath:
                    PrintAttributes(svgClipPath, indentLine, indentAttribute);
                    break;
                case SvgDocument svgDocument:
                    PrintAttributes(svgDocument, indentLine, indentAttribute);
                    break;
                case SvgFragment svgFragment:
                    PrintAttributes(svgFragment, indentLine, indentAttribute);
                    break;
                case SvgMask svgMask:
                    PrintAttributes(svgMask, indentLine, indentAttribute);
                    break;
                case SvgDefinitionList svgDefinitionList:
                    PrintAttributes(svgDefinitionList, indentLine, indentAttribute);
                    break;
                case SvgDescription svgDescription:
                    PrintAttributes(svgDescription, indentLine, indentAttribute);
                    break;
                case SvgDocumentMetadata svgDocumentMetadata:
                    PrintAttributes(svgDocumentMetadata, indentLine, indentAttribute);
                    break;
                case SvgTitle svgTitle:
                    PrintAttributes(svgTitle, indentLine, indentAttribute);
                    break;
                case SvgMergeNode svgMergeNode:
                    PrintAttributes(svgMergeNode, indentLine, indentAttribute);
                    break;
                case SvgFilter svgFilter:
                    PrintAttributes(svgFilter, indentLine, indentAttribute);
                    break;
                case NonSvgElement nonSvgElement:
                    PrintAttributes(nonSvgElement, indentLine, indentAttribute);
                    break;
                case SvgGradientStop svgGradientStop:
                    PrintAttributes(svgGradientStop, indentLine, indentAttribute);
                    break;
                case SvgUnknownElement svgUnknownElement:
                    PrintAttributes(svgUnknownElement, indentLine, indentAttribute);
                    break;
                case SvgFont svgFont:
                    PrintAttributes(svgFont, indentLine, indentAttribute);
                    break;
                case SvgFontFace svgFontFace:
                    PrintAttributes(svgFontFace, indentLine, indentAttribute);
                    break;
                case SvgFontFaceSrc svgFontFaceSrc:
                    PrintAttributes(svgFontFaceSrc, indentLine, indentAttribute);
                    break;
                case SvgFontFaceUri svgFontFaceUri:
                    PrintAttributes(svgFontFaceUri, indentLine, indentAttribute);
                    break;
                case SvgImage svgImage:
                    PrintAttributes(svgImage, indentLine, indentAttribute);
                    break;
                case SvgSwitch svgSwitch:
                    PrintAttributes(svgSwitch, indentLine, indentAttribute);
                    break;
                case SvgSymbol svgSymbol:
                    PrintAttributes(svgSymbol, indentLine, indentAttribute);
                    break;
                case SvgUse svgUse:
                    PrintAttributes(svgUse, indentLine, indentAttribute);
                    break;
                case SvgForeignObject svgForeignObject:
                    PrintAttributes(svgForeignObject, indentLine, indentAttribute);
                    break;
                case SvgCircle svgCircle:
                    PrintAttributes(svgCircle, indentLine, indentAttribute);
                    break;
                case SvgEllipse svgEllipse:
                    PrintAttributes(svgEllipse, indentLine, indentAttribute);
                    break;
                case SvgRectangle svgRectangle:
                    PrintAttributes(svgRectangle, indentLine, indentAttribute);
                    break;
                case SvgMarker svgMarker:
                    PrintAttributes(svgMarker, indentLine, indentAttribute);
                    break;
                case SvgGlyph svgGlyph:
                    PrintAttributes(svgGlyph, indentLine, indentAttribute);
                    break;
                case SvgGroup svgGroup:
                    PrintAttributes(svgGroup, indentLine, indentAttribute);
                    break;
                case SvgLine svgLine:
                    PrintAttributes(svgLine, indentLine, indentAttribute);
                    break;
                case SvgPath svgPath:
                    PrintAttributes(svgPath, indentLine, indentAttribute);
                    break;
                case SvgPolygon svgPolygon:
                    PrintAttributes(svgPolygon, indentLine, indentAttribute);
                    break;
                case SvgText svgText:
                    PrintAttributes(svgText, indentLine, indentAttribute);
                    break;
                case SvgTextPath svgTextPath:
                    PrintAttributes(svgTextPath, indentLine, indentAttribute);
                    break;
                case SvgTextRef svgTextRef:
                    PrintAttributes(svgTextRef, indentLine, indentAttribute);
                    break;
                case SvgTextSpan svgTextSpan:
                    PrintAttributes(svgTextSpan, indentLine, indentAttribute);
                    break;
                case SvgColourMatrix svgColourMatrix:
                    PrintAttributes(svgColourMatrix, indentLine, indentAttribute);
                    break;
                case SvgGaussianBlur svgGaussianBlur:
                    PrintAttributes(svgGaussianBlur, indentLine, indentAttribute);
                    break;
                case SvgMerge svgMerge:
                    PrintAttributes(svgMerge, indentLine, indentAttribute);
                    break;
                case SvgOffset svgOffset:
                    PrintAttributes(svgOffset, indentLine, indentAttribute);
                    break;
                case SvgColourServer svgColourServer:
                    PrintAttributes(svgColourServer, indentLine, indentAttribute);
                    break;
                case SvgDeferredPaintServer svgDeferredPaintServer:
                    PrintAttributes(svgDeferredPaintServer, indentLine, indentAttribute);
                    break;
                case SvgFallbackPaintServer svgFallbackPaintServer:
                    PrintAttributes(svgFallbackPaintServer, indentLine, indentAttribute);
                    break;
                case SvgPatternServer svgPatternServer:
                    PrintAttributes(svgPatternServer, indentLine, indentAttribute);
                    break;
                case SvgLinearGradientServer svgLinearGradientServer:
                    PrintAttributes(svgLinearGradientServer, indentLine, indentAttribute);
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    PrintAttributes(svgRadialGradientServer, indentLine, indentAttribute);
                    break;
                case SvgVerticalKern svgVerticalKern:
                    PrintAttributes(svgVerticalKern, indentLine, indentAttribute);
                    break;
                case SvgHorizontalKern svgHorizontalKern:
                    PrintAttributes(svgHorizontalKern, indentLine, indentAttribute);
                    break;
                default:
                    break;
            }

            PrintSvgElementChildren(element, indentLine, indentAttribute);
            PrintSvgElementNodes(element, indentLine, indentAttribute);
        }

        internal static void PrintSvgFragmentAttributes(SvgFragment fragment, string indentLine, string indentAttribute)
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

        internal static void PrintSvgFragment(SvgFragment fragment, string indentLine, string indentAttribute)
        {
            WriteLine($"{fragment}", s_elementColor);
            PrintSvgFragmentAttributes(fragment, indentLine, indentAttribute);
            PrintSvgElementElement(fragment, indentLine, indentAttribute);
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
                PrintSvgFragment(document, "    ", "");
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
