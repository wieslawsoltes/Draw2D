using System;
using Svg;
using Svg.DataTypes;
using Svg.Document_Structure;
using Svg.FilterEffects;
using Svg.Pathing;

namespace SvgDemo
{
    public static class SvgDebug
    {
        public static ConsoleColor s_errorColor = ConsoleColor.Yellow;

        public static ConsoleColor s_elementColor = ConsoleColor.Red;

        public static ConsoleColor s_groupColor = ConsoleColor.White;

        public static ConsoleColor s_attributeColor = ConsoleColor.Blue;

        public static string s_indentTab = "    ";

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void WriteLine(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }

        public static void PrintSvgPaintServerServer(SvgPaintServer svgPaintServer, string attribute, string indentLine, string indentAttribute)
        {
            switch (svgPaintServer)
            {
                case SvgColourServer colourServer:
                    // TODO:
                    WriteLine($"{indentLine}{indentAttribute}[{attribute}]={colourServer.ToString()}", s_attributeColor);
                    break;
                case SvgDeferredPaintServer deferredPaintServer:
                    // TODO:
                    WriteLine($"{indentLine}{indentAttribute}[{attribute}]={deferredPaintServer.GetType()}", s_attributeColor);
                    break;
                case SvgFallbackPaintServer fallbackPaintServer:
                    // TODO:
                    WriteLine($"{indentLine}{indentAttribute}[{attribute}]={fallbackPaintServer.GetType()}", s_attributeColor);
                    break;
                case SvgGradientServer gradientServer:
                    // TODO:
                    WriteLine($"{indentLine}{indentAttribute}[{attribute}]={gradientServer.GetType()}", s_attributeColor);
                    break;
                case SvgPatternServer patternServer:
                    // TODO:
                    WriteLine($"{indentLine}{indentAttribute}[{attribute}]={patternServer.GetType()}", s_attributeColor);
                    break;
                default:
                    WriteLine($"{indentLine}{indentAttribute}Unknown paint server type: {svgPaintServer.GetType()}", s_errorColor);
                    break;
            }
        }

        public static void PrintAttributes(SvgClipPath svgClipPath, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgFragment svgFragment, string indentLine, string indentAttribute)
        {
            if (svgFragment.X != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[x]={svgFragment.X}", s_attributeColor);
            }

            if (svgFragment.Y != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[y]={svgFragment.Y}", s_attributeColor);
            }

            if (svgFragment.Width != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indentLine}{indentAttribute}[width]={svgFragment.Width}", s_attributeColor);
            }

            if (svgFragment.Height != new SvgUnit(SvgUnitType.Percentage, 100f))
            {
                WriteLine($"{indentLine}{indentAttribute}[height]={svgFragment.Height}", s_attributeColor);
            }

            if (svgFragment.Overflow != SvgOverflow.Inherit && svgFragment.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indentLine}{indentAttribute}[overflow]={svgFragment.Overflow}", s_attributeColor);
            }

            if (svgFragment.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = svgFragment.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}[viewBox]={viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", s_attributeColor);
            }

            if (svgFragment.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgFragment.AspectRatio.Align != @default.Align
                 || svgFragment.AspectRatio.Slice != @default.Slice
                 || svgFragment.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}[preserveAspectRatio]={svgFragment.AspectRatio}", s_attributeColor);
                }
            }

            if (svgFragment.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-size]={svgFragment.FontSize}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgFragment.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}[font-family]={svgFragment.FontFamily}", s_attributeColor);
            }

            if (svgFragment.SpaceHandling != XmlSpaceHandling.@default && svgFragment.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[space]={svgFragment.SpaceHandling}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgMask svgMask, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgDefinitionList svgDefinitionList, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgDescription svgDescription, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgDocumentMetadata svgDocumentMetadata, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgTitle svgTitle, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgMergeNode svgMergeNode, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgFilter svgFilter, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(NonSvgElement nonSvgElement, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgGradientStop svgGradientStop, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgUnknownElement svgUnknownElement, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgFont svgFont, string indentLine, string indentAttribute)
        {
            if (svgFont.HorizAdvX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[horiz-adv-x]={svgFont.HorizAdvX}", s_attributeColor);
            }

            if (svgFont.HorizOriginX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[horiz-origin-x]={svgFont.HorizOriginX}", s_attributeColor);
            }

            if (svgFont.HorizOriginY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[horiz-origin-y]={svgFont.HorizOriginY}", s_attributeColor);
            }

            if (svgFont.VertAdvY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[vert-adv-y]={svgFont.VertAdvY}", s_attributeColor);
            }

            if (svgFont.VertOriginX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[vert-origin-x]={svgFont.VertOriginX}", s_attributeColor);
            }

            if (svgFont.VertOriginY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[vert-origin-y]={svgFont.VertOriginY}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgFontFace svgFontFace, string indentLine, string indentAttribute)
        {
            if (svgFontFace.Alphabetic != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[alphabetic]={svgFontFace.Alphabetic}", s_attributeColor);
            }

            if (svgFontFace.Ascent != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[ascent]={svgFontFace.Ascent}", s_attributeColor);
            }

            if (svgFontFace.AscentHeight != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[ascent-height]={svgFontFace.AscentHeight}", s_attributeColor);
            }

            if (svgFontFace.Descent != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[descent]={svgFontFace.Descent}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgFontFace.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}[font-family]={svgFontFace.FontFamily}", s_attributeColor);
            }

            if (svgFontFace.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-size]={svgFontFace.FontSize}", s_attributeColor);
            }

            if (svgFontFace.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-style]={svgFontFace.FontStyle}", s_attributeColor);
            }

            if (svgFontFace.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-variant]={svgFontFace.FontVariant}", s_attributeColor);
            }

            if (svgFontFace.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-weight]={svgFontFace.FontWeight}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgFontFace.Panose1))
            {
                WriteLine($"{indentLine}{indentAttribute}[panose-1]={svgFontFace.Panose1}", s_attributeColor);
            }

            if (svgFontFace.UnitsPerEm != 1000f)
            {
                WriteLine($"{indentLine}{indentAttribute}[units-per-em]={svgFontFace.UnitsPerEm}", s_attributeColor);
            }

            if (svgFontFace.XHeight != float.MinValue)
            {
                WriteLine($"{indentLine}{indentAttribute}[x-height]={svgFontFace.XHeight}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgFontFaceSrc svgFontFaceSrc, string indentLine, string indentAttribute)
        {
        }

        public static void PrintAttributes(SvgFontFaceUri svgFontFaceUri, string indentLine, string indentAttribute)
        {
            if (svgFontFaceUri.ReferencedElement != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[href]={svgFontFaceUri.ReferencedElement}", s_attributeColor);
            }
        }

        public static void PrintSvgVisualElementAttributes(SvgVisualElement svgVisualElement, string indentLine, string indentAttribute)
        {
            if (!string.IsNullOrEmpty(svgVisualElement.Clip))
            {
                WriteLine($"{indentLine}{indentAttribute}[clip]={svgVisualElement.Clip}", s_attributeColor);
            }

            if (svgVisualElement.ClipPath != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[clip-path]={svgVisualElement.ClipPath}", s_attributeColor);
            }

            if (svgVisualElement.ClipRule != SvgClipRule.NonZero)
            {
                WriteLine($"{indentLine}{indentAttribute}[clip-rule]={svgVisualElement.ClipRule}", s_attributeColor);
            }

            if (svgVisualElement.Filter != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[filter]={svgVisualElement.Filter}", s_attributeColor);
            }

            // Style

            if (svgVisualElement.Visible != true)
            {
                WriteLine($"{indentLine}{indentAttribute}[visibility]={svgVisualElement.Visible}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgVisualElement.Display))
            {
                WriteLine($"{indentLine}{indentAttribute}[display]={svgVisualElement.Display}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgVisualElement.EnableBackground))
            {
                WriteLine($"{indentLine}{indentAttribute}[enable-background]={svgVisualElement.EnableBackground}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgImage svgImage, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgImage, indentLine, indentAttribute);

            // TODO:
        }

        public static void PrintAttributes(SvgSwitch svgSwitch, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgSwitch, indentLine, indentAttribute);

            // TODO:
        }

        public static void PrintAttributes(SvgSymbol svgSymbol, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgSymbol, indentLine, indentAttribute);

            // TODO:
        }

        public static void PrintAttributes(SvgUse svgUse, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgUse, indentLine, indentAttribute);

            // TODO:
        }

        public static void PrintAttributes(SvgForeignObject svgForeignObject, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgForeignObject, indentLine, indentAttribute);

            // TODO:
        }

        public static void PrintSvgPathBasedElementAttributes(SvgPathBasedElement svgPathBasedElement, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgPathBasedElement, indentLine, indentAttribute);
        }

        public static void PrintAttributes(SvgCircle svgCircle, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgCircle, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}[cx]={svgCircle.CenterX}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[cy]={svgCircle.CenterY}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[r]={svgCircle.Radius}", s_attributeColor);
        }

        public static void PrintAttributes(SvgEllipse svgEllipse, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgEllipse, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}[cx]={svgEllipse.CenterX}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[cy]={svgEllipse.CenterY}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[rx]={svgEllipse.RadiusX}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[ry]={svgEllipse.RadiusY}", s_attributeColor);
        }

        public static void PrintAttributes(SvgRectangle svgRectangle, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgRectangle, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}[x]={svgRectangle.X}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[y]={svgRectangle.Y}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[width]={svgRectangle.Width}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[height]={svgRectangle.Height}", s_attributeColor);

            if (svgRectangle.CornerRadiusX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[rx]={svgRectangle.CornerRadiusX}", s_attributeColor);
            }

            if (svgRectangle.CornerRadiusY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[ry]={svgRectangle.CornerRadiusY}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgMarker svgMarker, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgMarker, indentLine, indentAttribute);

            if (svgMarker.RefX != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}[refX]={svgMarker.RefX}", s_attributeColor);
            }

            if (svgMarker.RefY != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}[refY]={svgMarker.RefY}", s_attributeColor);
            }

            if (svgMarker.Orient != null)
            {
                var orient = svgMarker.Orient;
                if (orient.IsAuto == false)
                {
                    if (orient.Angle != 0f)
                    {
                        WriteLine($"{indentLine}{indentAttribute}[orient]={orient.Angle}", s_attributeColor);
                    }
                }
                else
                {
                    WriteLine($"{indentLine}{indentAttribute}[orient]={(orient.IsAutoStartReverse ? "auto-start-reverse" : "auto")}", s_attributeColor);
                }
            }

            if (svgMarker.Overflow != SvgOverflow.Hidden)
            {
                WriteLine($"{indentLine}{indentAttribute}[overflow]={svgMarker.Overflow}", s_attributeColor);
            }

            if (svgMarker.ViewBox != SvgViewBox.Empty)
            {
                var viewBox = svgMarker.ViewBox;
                WriteLine($"{indentLine}{indentAttribute}[viewBox]={viewBox.MinX} {viewBox.MinY} {viewBox.Width} {viewBox.Height}", s_attributeColor);
            }

            if (svgMarker.AspectRatio != null)
            {
                var @default = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid);
                if (svgMarker.AspectRatio.Align != @default.Align
                 || svgMarker.AspectRatio.Slice != @default.Slice
                 || svgMarker.AspectRatio.Defer != @default.Defer)
                {
                    WriteLine($"{indentLine}{indentAttribute}[preserveAspectRatio]={svgMarker.AspectRatio}", s_attributeColor);
                }
            }

            if (svgMarker.MarkerWidth != 3f)
            {
                WriteLine($"{indentLine}{indentAttribute}[markerWidth]={svgMarker.MarkerWidth}", s_attributeColor);
            }

            if (svgMarker.MarkerHeight != 3f)
            {
                WriteLine($"{indentLine}{indentAttribute}[markerHeight]={svgMarker.MarkerHeight}", s_attributeColor);
            }

            if (svgMarker.MarkerUnits != SvgMarkerUnits.StrokeWidth)
            {
                WriteLine($"{indentLine}{indentAttribute}[markerUnits]={svgMarker.MarkerUnits}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgGlyph svgGlyph, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgGlyph, indentLine, indentAttribute);

            if (svgGlyph.PathData != null)
            {
                PrintAttributes(svgGlyph.PathData, indentLine, indentAttribute);
            }

            if (!string.IsNullOrEmpty(svgGlyph.GlyphName))
            {
                WriteLine($"{indentLine}{indentAttribute}[glyph-name]={svgGlyph.GlyphName}", s_attributeColor);
            }

            if (svgGlyph.HorizAdvX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[horiz-adv-x]={svgGlyph.HorizAdvX}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgGlyph.Unicode))
            {
                WriteLine($"{indentLine}{indentAttribute}[unicode]={svgGlyph.Unicode}", s_attributeColor);
            }

            if (svgGlyph.VertAdvY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[vert-adv-y]={svgGlyph.VertAdvY}", s_attributeColor);
            }

            if (svgGlyph.VertOriginX != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[vert-origin-x]={svgGlyph.VertOriginX}", s_attributeColor);
            }

            if (svgGlyph.VertOriginY != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[vert-origin-y]={svgGlyph.VertOriginY}", s_attributeColor);
            }
        }

        public static void PrintSvgMarkerElementAttributes(SvgMarkerElement svgMarkerElement, string indentLine, string indentAttribute)
        {
            PrintSvgPathBasedElementAttributes(svgMarkerElement, indentLine, indentAttribute);

            if (svgMarkerElement.MarkerEnd != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[marker-end]={svgMarkerElement.MarkerEnd}", s_attributeColor);
            }

            if (svgMarkerElement.MarkerMid != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[marker-mid]={svgMarkerElement.MarkerMid}", s_attributeColor);
            }

            if (svgMarkerElement.MarkerStart != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[marker-start]={svgMarkerElement.MarkerStart}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgGroup svgGroup, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgGroup, indentLine, indentAttribute);
        }

        public static void PrintAttributes(SvgLine svgLine, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgLine, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}[x1]={svgLine.StartX}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[y1]={svgLine.StartY}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[x2]={svgLine.EndX}", s_attributeColor);
            WriteLine($"{indentLine}{indentAttribute}[y2]={svgLine.EndY}", s_attributeColor);
        }

        public static void PrintAttributes(SvgPathSegmentList svgPathSegmentList, string indentLine, string indentAttribute)
        {
            if (svgPathSegmentList != null)
            {
                /// The <see cref="SvgPathSegment"/> object graph.
                /// +---abstract class <see cref="SvgPathSegment"/>
                ///     +---class <see cref="SvgArcSegment"/>
                ///     +---class <see cref="SvgClosePathSegment"/>
                ///     +---class <see cref="SvgCubicCurveSegment"/>
                ///     +---class <see cref="SvgLineSegment"/>
                ///     +---class <see cref="SvgMoveToSegment"/>
                ///     \---class <see cref="SvgQuadraticCurveSegment"/>

                WriteLine($"{indentLine}{indentAttribute}[d]=", s_attributeColor);

                string segmentIndent = "    ";

                foreach (var svgSegment in svgPathSegmentList)
                {
                    switch (svgSegment)
                    {
                        case SvgArcSegment svgArcSegment:
                            // TODO:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}{indentAttribute}", s_attributeColor);
                            break;
                        case SvgClosePathSegment svgClosePathSegment:
                            // TODO:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}{svgClosePathSegment}", s_attributeColor);
                            break;
                        case SvgCubicCurveSegment svgCubicCurveSegment:
                            // TODO:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}{svgCubicCurveSegment}", s_attributeColor);
                            break;
                        case SvgLineSegment svgLineSegment:
                            // TODO:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}{svgLineSegment}", s_attributeColor);
                            break;
                        case SvgMoveToSegment svgMoveToSegment:
                            // TODO:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}{svgMoveToSegment}", s_attributeColor);
                            break;
                        case SvgQuadraticCurveSegment svgQuadraticCurveSegment:
                            // TODO:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}{svgQuadraticCurveSegment}", s_attributeColor);
                            break;
                        default:
                            WriteLine($"{indentLine}{indentAttribute}{segmentIndent}Unknown path segment type: {svgSegment.GetType()}", s_errorColor);
                            break;
                    }
                }
            }
        }

        public static void PrintAttributes(SvgPath svgPath, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgPath, indentLine, indentAttribute);

            if (svgPath.PathData != null)
            {
                PrintAttributes(svgPath.PathData, indentLine, indentAttribute);
            }

            if (svgPath.PathLength != 0f)
            {
                WriteLine($"{indentLine}{indentAttribute}[pathLength]={svgPath.PathLength}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgPolygon svgPolygon, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgPolygon, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}[points]={svgPolygon.Points}", s_attributeColor);
        }

        public static void PrintAttributes(SvgPolyline svgPolyline, string indentLine, string indentAttribute)
        {
            PrintSvgMarkerElementAttributes(svgPolyline, indentLine, indentAttribute);

            WriteLine($"{indentLine}{indentAttribute}[points]={svgPolyline.Points}", s_attributeColor);
        }

        public static void PrintSvgTextBaseAttributes(SvgTextBase svgTextBase, string indentLine, string indentAttribute)
        {
            PrintSvgVisualElementAttributes(svgTextBase, indentLine, indentAttribute);

            if (!string.IsNullOrEmpty(svgTextBase.Text))
            {
                WriteLine($"{indentLine}{indentAttribute}[Content]={svgTextBase.Text}", s_attributeColor);
            }

            if (svgTextBase.X != null && svgTextBase.X.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}[x]={svgTextBase.X}", s_attributeColor);
            }

            if (svgTextBase.Dx != null && svgTextBase.Dx.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}[dx]={svgTextBase.Dx}", s_attributeColor);
            }

            if (svgTextBase.Y != null && svgTextBase.Y.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}[y]={svgTextBase.Y}", s_attributeColor);
            }

            if (svgTextBase.Dy != null && svgTextBase.Dy.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}[dy]={svgTextBase.Dy}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgTextBase.Rotate))
            {
                WriteLine($"{indentLine}{indentAttribute}[rotate]={svgTextBase.Rotate}", s_attributeColor);
            }

            if (svgTextBase.TextLength != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}[textLength]={svgTextBase.TextLength}", s_attributeColor);
            }

            if (svgTextBase.LengthAdjust != SvgTextLengthAdjust.Spacing)
            {
                WriteLine($"{indentLine}{indentAttribute}[lengthAdjust]={svgTextBase.LengthAdjust}", s_attributeColor);
            }

            if (svgTextBase.LetterSpacing != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}[letter-spacing]={svgTextBase.LetterSpacing}", s_attributeColor);
            }

            if (svgTextBase.WordSpacing != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}[word-spacing]={svgTextBase.WordSpacing}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgText svgText, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgText, indentLine, indentAttribute);
        }

        public static void PrintAttributes(SvgTextPath svgTextPath, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgTextPath, indentLine, indentAttribute);

            if (svgTextPath.StartOffset != SvgUnit.None)
            {
                WriteLine($"{indentLine}{indentAttribute}[startOffset]={svgTextPath.StartOffset}", s_attributeColor);
            }

            if (svgTextPath.Method != SvgTextPathMethod.Align)
            {
                WriteLine($"{indentLine}{indentAttribute}[method]={svgTextPath.Method}", s_attributeColor);
            }

            if (svgTextPath.Spacing != SvgTextPathSpacing.Exact)
            {
                WriteLine($"{indentLine}{indentAttribute}[spacing]={svgTextPath.Spacing}", s_attributeColor);
            }

            if (svgTextPath.ReferencedPath != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[href]={svgTextPath.ReferencedPath}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgTextRef svgTextRef, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgTextRef, indentLine, indentAttribute);

            if (svgTextRef.ReferencedElement != null)
            {
                WriteLine($"{indentLine}{indentAttribute}[href]={svgTextRef.ReferencedElement}", s_attributeColor);
            }
        }

        public static void PrintAttributes(SvgTextSpan svgTextSpan, string indentLine, string indentAttribute)
        {
            PrintSvgTextBaseAttributes(svgTextSpan, indentLine, indentAttribute);
        }

        public static void PrintAttributes(SvgColourMatrix svgColourMatrix, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgGaussianBlur svgGaussianBlur, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgMerge svgMerge, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgOffset svgOffset, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgColourServer svgColourServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgDeferredPaintServer svgDeferredPaintServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgFallbackPaintServer svgFallbackPaintServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgPatternServer svgPatternServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgLinearGradientServer svgLinearGradientServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgRadialGradientServer svgRadialGradientServer, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgVerticalKern svgVerticalKern, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintAttributes(SvgHorizontalKern svgHorizontalKern, string indentLine, string indentAttribute)
        {
            // TODO:
        }

        public static void PrintSvgElementAttributes(SvgElement svgElement, string indentLine, string indentAttribute)
        {
            // Transforms Attributes

            if (svgElement.Transforms.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}<Transforms>", s_groupColor);
                WriteLine($"{indentLine}{indentAttribute}[transform]=", s_attributeColor);

                string transformIndent = "            ";

                foreach (var transform in svgElement.Transforms)
                {
                    WriteLine($"{indentLine}{indentAttribute}{transformIndent}{transform}", s_attributeColor);
                }
            }

            // Attributes

            if (!string.IsNullOrEmpty(svgElement.ID))
            {
                WriteLine($"{indentLine}{indentAttribute}[id]={svgElement.ID}", s_attributeColor);
            }

            if (svgElement.SpaceHandling != XmlSpaceHandling.@default && svgElement.SpaceHandling != XmlSpaceHandling.inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[space]={svgElement.SpaceHandling}", s_attributeColor);
            }

            if (svgElement.Color != null && svgElement.Color != SvgColourServer.NotSet)
            {
                PrintSvgPaintServerServer(svgElement.Color, "color", indentLine, indentAttribute);
            }

            // Style Attributes

            if (svgElement.Fill != null && svgElement.Fill != SvgColourServer.NotSet)
            {
                PrintSvgPaintServerServer(svgElement.Fill, "fill", indentLine, indentAttribute);
            }

            if (svgElement.Stroke != null)
            {
                PrintSvgPaintServerServer(svgElement.Stroke, "stroke", indentLine, indentAttribute);
            }

            if (svgElement.FillRule != SvgFillRule.NonZero)
            {
                WriteLine($"{indentLine}{indentAttribute}[fill-rule]={svgElement.FillRule}", s_attributeColor);
            }

            if (svgElement.FillOpacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[fill-opacity]={svgElement.FillOpacity}", s_attributeColor);
            }

            if (svgElement.StrokeWidth != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-width]={svgElement.StrokeWidth}", s_attributeColor);
            }

            if (svgElement.StrokeLineCap != SvgStrokeLineCap.Butt)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-linecap]={svgElement.StrokeLineCap}", s_attributeColor);
            }

            if (svgElement.StrokeLineJoin != SvgStrokeLineJoin.Miter)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-linejoin]={svgElement.StrokeLineJoin}", s_attributeColor);
            }

            if (svgElement.StrokeMiterLimit != 4f)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-miterlimit]={svgElement.StrokeMiterLimit}", s_attributeColor);
            }

            if (svgElement.StrokeDashArray != null && svgElement.StrokeDashArray.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-dasharray]={svgElement.StrokeDashArray}", s_attributeColor);
            }

            if (svgElement.StrokeDashOffset != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-dashoffset]={svgElement.StrokeDashOffset}", s_attributeColor);
            }

            if (svgElement.StrokeOpacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[stroke-opacity]={svgElement.StrokeOpacity}", s_attributeColor);
            }

            if (svgElement.StopColor != null)
            {
                PrintSvgPaintServerServer(svgElement.StopColor, "stop-color", indentLine, indentAttribute);
            }

            if (svgElement.Opacity != 1f)
            {
                WriteLine($"{indentLine}{indentAttribute}[opacity]={svgElement.Opacity}", s_attributeColor);
            }

            if (svgElement.ShapeRendering != SvgShapeRendering.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[shape-rendering]={svgElement.ShapeRendering}", s_attributeColor);
            }

            if (svgElement.TextAnchor != SvgTextAnchor.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[text-anchor]={svgElement.TextAnchor}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgElement.BaselineShift))
            {
                WriteLine($"{indentLine}{indentAttribute}[baseline-shift]={svgElement.BaselineShift}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgElement.FontFamily))
            {
                WriteLine($"{indentLine}{indentAttribute}[font-family]={svgElement.FontFamily}", s_attributeColor);
            }

            if (svgElement.FontSize != SvgUnit.Empty)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-size]={svgElement.FontSize}", s_attributeColor);
            }

            if (svgElement.FontStyle != SvgFontStyle.All)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-style]={svgElement.FontStyle}", s_attributeColor);
            }

            if (svgElement.FontVariant != SvgFontVariant.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-variant]={svgElement.FontVariant}", s_attributeColor);
            }

            if (svgElement.TextDecoration != SvgTextDecoration.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[text-decoration]={svgElement.TextDecoration}", s_attributeColor);
            }

            if (svgElement.FontWeight != SvgFontWeight.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[font-weight]={svgElement.FontWeight}", s_attributeColor);
            }

            if (svgElement.TextTransformation != SvgTextTransformation.Inherit)
            {
                WriteLine($"{indentLine}{indentAttribute}[text-transform]={svgElement.TextTransformation}", s_attributeColor);
            }

            if (!string.IsNullOrEmpty(svgElement.Font))
            {
                WriteLine($"{indentLine}{indentAttribute}[font]={svgElement.Font}", s_attributeColor);
            }
        }

        public static void PrintSvgElementCustomAttributes(SvgElement svgElement, string indentLine, string indentAttribute)
        {
            if (svgElement.CustomAttributes.Count > 0)
            {
                WriteLine($"{indentLine}{indentAttribute}<CustomAttributes>", s_groupColor);

                foreach (var attribute in svgElement.CustomAttributes)
                {
                    WriteLine($"{indentLine}{indentAttribute}[{attribute.Key}]={attribute.Value}", s_attributeColor);
                }
            }
        }

        public static void PrintSvgElementChildren(SvgElement svgElement, string indentLine, string indentAttribute)
        {
            if (svgElement.Children.Count > 0)
            {
                WriteLine($"{indentLine}<Children>", s_groupColor);

                foreach (var child in svgElement.Children)
                {
                    PrintSvgElement(child, indentLine + s_indentTab, indentAttribute);
                }
            }
        }

        public static void PrintSvgElementNodes(SvgElement svgElement, string indentLine, string indentAttribute)
        {
            if (svgElement.Nodes.Count > 0)
            {
                WriteLine($"{indentLine}<Nodes>", s_groupColor);

                foreach (var node in svgElement.Nodes)
                {
                    WriteLine($"{indentLine}{node.Content}", s_attributeColor);
                }
            }
        }

        public static void PrintSvgElement(SvgElement svgElement, string indentLine, string indentAttribute)
        {
            WriteLine($"{indentLine}{svgElement.GetType()}", s_elementColor);

            PrintSvgElementAttributes(svgElement, indentLine, indentAttribute);

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
            /// |   +---abstract class <see cref="SvgVisualElement"/>
            /// |   |   +---class <see cref="SvgImage"/>
            /// |   |   +---class <see cref="SvgSwitch"/>
            /// |   |   +---class <see cref="SvgSymbol"/>
            /// |   |   +---class <see cref="SvgUse"/>
            /// |   |   +---class <see cref="SvgForeignObject"/>
            /// |   |   +---abstract class <see cref="SvgPathBasedElement"/>
            /// |   |       +---<see cref="SvgCircle"/>
            /// |   |       +---<see cref="SvgEllipse"/>
            /// |   |       +---<see cref="SvgRectangle"/>
            /// |   |       +---<see cref="SvgMarker"/>
            /// |   |       +---<see cref="SvgGlyph"/>
            /// |   |       +---abstract class <see cref="SvgMarkerElement"/>
            /// |   |           +---class <see cref="SvgGroup"/>
            /// |   |           +---class <see cref="SvgLine"/>
            /// |   |           +---class <see cref="SvgPath"/>
            /// |   |           \---class <see cref="SvgPolygon"/>
            /// |   |               \---class <see cref="SvgPolyline"/>
            /// |   \-------abstract class <see cref="SvgTextBase"/>
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

            switch (svgElement)
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
                case SvgPolyline svgPolyline:
                    PrintAttributes(svgPolyline, indentLine, indentAttribute);
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
                    WriteLine($"{indentLine}Unknown elemen type: {svgElement.GetType()}", s_errorColor);
                    break;
            }

            PrintSvgElementCustomAttributes(svgElement, indentLine, indentAttribute);

            PrintSvgElementChildren(svgElement, indentLine, indentAttribute);

            PrintSvgElementNodes(svgElement, indentLine, indentAttribute);
        }

        public static void Run(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string path = args[i];
                WriteLine($"Path: {path}", s_groupColor);

                var svgDocument = SvgDocument.Open<SvgDocument>(path, null);
                if (svgDocument != null)
                {
                    svgDocument.FlushStyles(true);
                    PrintSvgElement(svgDocument, s_indentTab, "");
                    ResetColor();
                }
            }
        }

        public static void Error(Exception ex)
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
