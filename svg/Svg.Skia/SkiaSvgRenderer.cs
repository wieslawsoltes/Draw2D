using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using SkiaSharp;
using Svg;
using Svg.Document_Structure;
using Svg.Pathing;
using Svg.Transforms;

namespace Svg.Skia
{
    public static class SkiaSvgRenderer
    {
        private static T GetReference<T>(SvgElement svgElement, Uri uri) where T : SvgElement
        {
            return svgElement.OwnerDocument.GetElementById(uri.ToString()) as T;
        }

        private static SKColor GetColor(SvgColourServer svgColourServer, float opacity, bool forStroke = false)
        {
            if (svgColourServer == SvgPaintServer.None)
            {
                return SKColors.Transparent;
            }

            if (svgColourServer == SvgColourServer.NotSet && forStroke)
            {
                return SKColors.Transparent;
            }

            var colour = svgColourServer.Colour;
            byte alpha = (byte)Math.Round((opacity * (svgColourServer.Colour.A / 255.0)) * 255);

            return new SKColor(colour.R, colour.G, colour.B, alpha);
        }

        private static string ToSvgPathData(SvgPathSegmentList svgPathSegmentList)
        {
            var sb = new StringBuilder();
            foreach (var svgSegment in svgPathSegmentList)
            {
                sb.AppendLine(svgSegment.ToString());
            }
            return sb.ToString();
        }

        private static SKPath ToSKPath(SvgPathSegmentList svgPathSegmentList, SvgFillRule svgFillRule)
        {
            var skPath = new SKPath()
            {
                FillType = (svgFillRule == SvgFillRule.EvenOdd) ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            foreach (var svgSegment in svgPathSegmentList)
            {
                switch (svgSegment)
                {
                    case SvgMoveToSegment svgMoveToSegment:
                        {
                            float x = (float)svgMoveToSegment.Start.X;
                            float y = (float)svgMoveToSegment.Start.Y;
                            skPath.MoveTo(x, y);
                        }
                        break;
                    case SvgLineSegment svgLineSegment:
                        {
                            float x = (float)svgLineSegment.End.X;
                            float y = (float)svgLineSegment.End.Y;
                            skPath.LineTo(x, y);
                        }
                        break;
                    case SvgCubicCurveSegment svgCubicCurveSegment:
                        {
                            float x0 = (float)svgCubicCurveSegment.FirstControlPoint.X;
                            float y0 = (float)svgCubicCurveSegment.FirstControlPoint.Y;
                            float x1 = (float)svgCubicCurveSegment.SecondControlPoint.X;
                            float y1 = (float)svgCubicCurveSegment.SecondControlPoint.Y;
                            float x2 = (float)svgCubicCurveSegment.End.X;
                            float y2 = (float)svgCubicCurveSegment.End.Y;
                            skPath.CubicTo(x0, y0, x1, y1, x2, y2);
                        }
                        break;
                    case SvgQuadraticCurveSegment svgQuadraticCurveSegment:
                        {
                            float x0 = (float)svgQuadraticCurveSegment.ControlPoint.X;
                            float y0 = (float)svgQuadraticCurveSegment.ControlPoint.Y;
                            float x1 = (float)svgQuadraticCurveSegment.End.X;
                            float y1 = (float)svgQuadraticCurveSegment.End.Y;
                            skPath.QuadTo(x0, y0, x1, y1);
                        }
                        break;
                    case SvgArcSegment svgArcSegment:
                        {
                            float rx = svgArcSegment.RadiusX;
                            float ry = svgArcSegment.RadiusY;
                            float xAxisRotate = (float)(svgArcSegment.Angle * Math.PI / 180.0);
                            var largeArc = svgArcSegment.Size == SvgArcSize.Small ? SKPathArcSize.Small : SKPathArcSize.Large;
                            var sweep = svgArcSegment.Sweep == SvgArcSweep.Negative ? SKPathDirection.CounterClockwise : SKPathDirection.Clockwise;
                            float x = (float)svgArcSegment.End.X;
                            float y = (float)svgArcSegment.End.Y;
                            skPath.ArcTo(rx, ry, xAxisRotate, largeArc, sweep, x, y);
                        }
                        break;
                    case SvgClosePathSegment svgClosePathSegment:
                        {
                            skPath.Close();
                        }
                        break;
                }
            }

            return skPath;
        }

        private static SKPath ToSKPath(SvgPointCollection svgPointCollection, SvgFillRule svgFillRule, bool isClosed)
        {
            var skPath = new SKPath()
            {
                FillType = (svgFillRule == SvgFillRule.EvenOdd) ? SKPathFillType.EvenOdd : SKPathFillType.Winding
            };

            var skPoints = new SKPoint[svgPointCollection.Count / 2];

            for (int i = 0; (i + 1) < svgPointCollection.Count; i += 2)
            {
                float x = (float)svgPointCollection[i];
                float y = (float)svgPointCollection[i + 1];
                skPoints[i / 2] = new SKPoint(x, y);
            }

            skPath.AddPoly(skPoints, false);

            if (isClosed)
            {
                skPath.Close();
            }

            return skPath;
        }

        private static SKMatrix Multiply(ref SKMatrix skMatrix1, ref SKMatrix skMatrix2)
        {
            return new SKMatrix()
            {
                ScaleX = (skMatrix1.ScaleX * skMatrix2.ScaleX) + (skMatrix1.SkewY * skMatrix2.SkewX),
                SkewY = (skMatrix1.ScaleX * skMatrix2.SkewY) + (skMatrix1.SkewY * skMatrix2.ScaleY),
                SkewX = (skMatrix1.SkewX * skMatrix2.ScaleX) + (skMatrix1.ScaleY * skMatrix2.SkewX),
                ScaleY = (skMatrix1.SkewX * skMatrix2.SkewY) + (skMatrix1.ScaleY * skMatrix2.ScaleY),
                TransX = (skMatrix1.TransX * skMatrix2.ScaleX) + (skMatrix1.TransY * skMatrix2.SkewX) + skMatrix2.TransX,
                TransY = (skMatrix1.TransX * skMatrix2.SkewY) + (skMatrix1.TransY * skMatrix2.ScaleY) + skMatrix2.TransY,
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1
            };
        }

        private static float AdjustSvgOpacity(float opacity)
        {
            return Math.Min(Math.Max(opacity, 0), 1);
        }

        private static SKPaint GetSKPaintOpacity(float opacity)
        {
            var paint = new SKPaint()
            {
                IsAntialias = true,
            };

            paint.Color = new SKColor(255, 255, 255, (byte)Math.Round(opacity * 255));
            paint.Style = SKPaintStyle.Fill;

            return paint;
        }

        private static SvgUnit NormalizeSvgUnit(SvgUnit svgUnit, SvgCoordinateUnits svgCoordinateUnits)
        {
            return (svgUnit.Type == SvgUnitType.Percentage && svgCoordinateUnits == SvgCoordinateUnits.ObjectBoundingBox ?
                    new SvgUnit(SvgUnitType.User, svgUnit.Value / 100) :
                    svgUnit);
        }

        public static SKShader CreateLinearGradient(SvgLinearGradientServer svgLinearGradientServer, SKSize size)
        {
            // TODO:

            var start = SvgUnit.GetDevicePoint(
                NormalizeSvgUnit(svgLinearGradientServer.X1, svgLinearGradientServer.GradientUnits),
                NormalizeSvgUnit(svgLinearGradientServer.Y1, svgLinearGradientServer.GradientUnits),
                null,
                svgLinearGradientServer);
            var end = SvgUnit.GetDevicePoint(
                NormalizeSvgUnit(svgLinearGradientServer.X2, svgLinearGradientServer.GradientUnits),
                NormalizeSvgUnit(svgLinearGradientServer.Y2, svgLinearGradientServer.GradientUnits),
                null,
                svgLinearGradientServer);

            var colors = new List<SKColor>();
            var colorPos = new List<float>();

            foreach (var child in svgLinearGradientServer.Children)
            {
                if (child is SvgGradientStop svgGradientStop)
                {
                    if (svgGradientStop.StopColor is SvgColourServer stopColorSvgColourServer)
                    {
                        var stopColor = GetColor(stopColorSvgColourServer, AdjustSvgOpacity(svgGradientStop.Opacity), false);
                        float offset = svgGradientStop.Offset.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLinearGradientServer);
                        offset /= size.Width;
                        colors.Add(stopColor);
                        colorPos.Add(offset);
                    }
                }
            }

            // TODO: Handle correctly all GradientUnits modes.

            switch (svgLinearGradientServer.GradientUnits)
            {
                default:
                case SvgCoordinateUnits.ObjectBoundingBox:
                    // TODO:
                    break;
                case SvgCoordinateUnits.UserSpaceOnUse:
                    // TODO:
                    break;
            }

            SKShaderTileMode shaderTileMode;
            switch (svgLinearGradientServer.SpreadMethod)
            {
                default:
                case SvgGradientSpreadMethod.Pad:
                    shaderTileMode = SKShaderTileMode.Clamp;
                    break;
                case SvgGradientSpreadMethod.Reflect:
                    shaderTileMode = SKShaderTileMode.Mirror;
                    break;
                case SvgGradientSpreadMethod.Repeat:
                    shaderTileMode = SKShaderTileMode.Repeat;
                    break;
            }

            SKPoint skStart = new SKPoint(start.X, start.Y);
            SKPoint skEnd = new SKPoint(end.X, end.Y);
            var skColors = colors.ToArray();
            float[] skColorPos = colorPos.ToArray();

            if (svgLinearGradientServer.GradientTransform.Count > 0)
            {
                var gradientTransform = GetSKMatrix(svgLinearGradientServer.GradientTransform);
                return SKShader.CreateLinearGradient(skStart, skEnd, skColors, skColorPos, shaderTileMode, gradientTransform);
            }
            else
            {
                return SKShader.CreateLinearGradient(skStart, skEnd, skColors, skColorPos, shaderTileMode);
            }
        }

        private static SKPaint GetFillSKPaint(SvgElement svgElement, SKSize skSize)
        {
            var skPaint = new SKPaint()
            {
                IsAntialias = true
            };

            switch (svgElement.Fill)
            {
                case SvgColourServer svgColourServer:
                    {
                        skPaint.Color = GetColor(svgColourServer, AdjustSvgOpacity(svgElement.FillOpacity), false);
                    }
                    break;
                case SvgPatternServer svgPatternServer:
                    {
                        // TODO:
                    }
                    break;
                case SvgLinearGradientServer svgLinearGradientServer:
                    {
                        // TODO: Dispose SKShader.
                        skPaint.Shader = CreateLinearGradient(svgLinearGradientServer, skSize);
                    }
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    {
                        // TODO:
                    }
                    break;
                case SvgDeferredPaintServer svgDeferredPaintServer:
                    {
                        // Not used.
                    }
                    break;
                case SvgFallbackPaintServer svgFallbackPaintServer:
                    {
                        // Not used.
                    }
                    break;
                default:
                    break;
            }

            skPaint.Style = SKPaintStyle.Fill;

            return skPaint;
        }

        private static SKPaint GetStrokeSKPaint(SvgElement svgElement, SKSize skSize)
        {
            var skPaint = new SKPaint()
            {
                IsAntialias = true
            };

            switch (svgElement.Stroke)
            {
                case SvgColourServer svgColourServer:
                    {
                        skPaint.Color = GetColor(svgColourServer, AdjustSvgOpacity(svgElement.StrokeOpacity), true);
                    }
                    break;
                case SvgPatternServer svgPatternServer:
                    {
                        // TODO:
                    }
                    break;
                case SvgLinearGradientServer svgLinearGradientServer:
                    {
                        // TODO: Dispose SKShader.
                        skPaint.Shader = CreateLinearGradient(svgLinearGradientServer, skSize);
                    }
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    {
                        // TODO:
                    }
                    break;
                case SvgDeferredPaintServer svgDeferredPaintServer:
                    {
                        // Not used.
                    }
                    break;
                case SvgFallbackPaintServer svgFallbackPaintServer:
                    {
                        // Not used.
                    }
                    break;
                default:
                    break;
            }

            skPaint.StrokeWidth = svgElement.StrokeWidth.ToDeviceValue(null, UnitRenderingType.Other, svgElement);
            skPaint.Style = SKPaintStyle.Stroke;

            return skPaint;
        }

        private static SKMatrix GetSKMatrix(SvgTransformCollection svgTransformCollection)
        {
            var totalSKMatrix = SKMatrix.MakeIdentity();

            foreach (var svgTransform in svgTransformCollection)
            {
                switch (svgTransform)
                {
                    case SvgMatrix svgMatrix:
                        {
                            var skMatrix = new SKMatrix()
                            {
                                ScaleX = svgMatrix.Points[0],
                                SkewY = svgMatrix.Points[1],
                                SkewX = svgMatrix.Points[2],
                                ScaleY = svgMatrix.Points[3],
                                TransX = svgMatrix.Points[4],
                                TransY = svgMatrix.Points[5],
                                Persp0 = 0,
                                Persp1 = 0,
                                Persp2 = 1
                            };
                            totalSKMatrix = Multiply(ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgRotate svgRotate:
                        {
                            var skMatrix = SKMatrix.MakeRotationDegrees(svgRotate.Angle, svgRotate.CenterX, svgRotate.CenterY);
                            totalSKMatrix = Multiply(ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgScale svgScale:
                        {
                            var skMatrix = SKMatrix.MakeScale(svgScale.X, svgScale.Y);
                            totalSKMatrix = Multiply(ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgShear svgShear:
                        {
                            // Not in the svg specification.
                        }
                        break;
                    case SvgSkew svgSkew:
                        {
                            var skMatrix = SKMatrix.MakeSkew(svgSkew.AngleX, svgSkew.AngleY);
                            totalSKMatrix = Multiply(ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgTranslate svgTranslate:
                        {
                            var skMatrix = SKMatrix.MakeTranslation(svgTranslate.X, svgTranslate.Y);
                            totalSKMatrix = Multiply(ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                }
            }

            return totalSKMatrix;
        }

        private static void SetSvgViewBoxTransform(SKCanvas skCanvas, SvgViewBox svgViewBox, SvgAspectRatio svgAspectRatio, float x, float y, float width, float height)
        {
            if (svgViewBox.Equals(SvgViewBox.Empty))
            {
                skCanvas.Translate(x, y);
                return;
            }

            float fScaleX = width / svgViewBox.Width;
            float fScaleY = height / svgViewBox.Height;
            float fMinX = -svgViewBox.MinX * fScaleX;
            float fMinY = -svgViewBox.MinY * fScaleY;

            if (svgAspectRatio == null)
            {
                svgAspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.xMidYMid, false);
            }

            if (svgAspectRatio.Align != SvgPreserveAspectRatio.none)
            {
                if (svgAspectRatio.Slice)
                {
                    fScaleX = Math.Max(fScaleX, fScaleY);
                    fScaleY = Math.Max(fScaleX, fScaleY);
                }
                else
                {
                    fScaleX = Math.Min(fScaleX, fScaleY);
                    fScaleY = Math.Min(fScaleX, fScaleY);
                }
                float fViewMidX = (svgViewBox.Width / 2) * fScaleX;
                float fViewMidY = (svgViewBox.Height / 2) * fScaleY;
                float fMidX = width / 2;
                float fMidY = height / 2;
                fMinX = -svgViewBox.MinX * fScaleX;
                fMinY = -svgViewBox.MinY * fScaleY;

                switch (svgAspectRatio.Align)
                {
                    case SvgPreserveAspectRatio.xMinYMin:
                        break;
                    case SvgPreserveAspectRatio.xMidYMin:
                        fMinX += fMidX - fViewMidX;
                        break;
                    case SvgPreserveAspectRatio.xMaxYMin:
                        fMinX += width - svgViewBox.Width * fScaleX;
                        break;
                    case SvgPreserveAspectRatio.xMinYMid:
                        fMinY += fMidY - fViewMidY;
                        break;
                    case SvgPreserveAspectRatio.xMidYMid:
                        fMinX += fMidX - fViewMidX;
                        fMinY += fMidY - fViewMidY;
                        break;
                    case SvgPreserveAspectRatio.xMaxYMid:
                        fMinX += width - svgViewBox.Width * fScaleX;
                        fMinY += fMidY - fViewMidY;
                        break;
                    case SvgPreserveAspectRatio.xMinYMax:
                        fMinY += height - svgViewBox.Height * fScaleY;
                        break;
                    case SvgPreserveAspectRatio.xMidYMax:
                        fMinX += fMidX - fViewMidX;
                        fMinY += height - svgViewBox.Height * fScaleY;
                        break;
                    case SvgPreserveAspectRatio.xMaxYMax:
                        fMinX += width - svgViewBox.Width * fScaleX;
                        fMinY += height - svgViewBox.Height * fScaleY;
                        break;
                    default:
                        break;
                }
            }

            skCanvas.Translate(x, y);
            skCanvas.Translate(fMinX, fMinY);
            skCanvas.Scale(fScaleX, fScaleY);
        }

        private static void SetTransform(SKCanvas skCanvas, SvgTransformCollection svgTransformCollection)
        {
            var skMatrix = GetSKMatrix(svgTransformCollection);
            var totalSKMatrix = skCanvas.TotalMatrix;
            totalSKMatrix = Multiply(ref totalSKMatrix, ref skMatrix);
            skCanvas.SetMatrix(totalSKMatrix);
        }

        private static SKPaint SetOpacity(SKCanvas skCanvas, SvgElement svgElement)
        {
            float opacity = AdjustSvgOpacity(svgElement.Opacity);
            bool canSetOpacity = true;

            if (svgElement.Parent != null)
            {
                float parentOpacity = AdjustSvgOpacity(svgElement.Parent.Opacity);
                canSetOpacity = opacity != parentOpacity;
            }

            if (opacity < 1f && canSetOpacity)
            {
                var paint = GetSKPaintOpacity(opacity);
                skCanvas.SaveLayer(paint);
                return paint; // TODO: Dispose;
            }

            return null;
        }

        private static void DrawSvgFragment(SKCanvas skCanvas, SKSize skSize, SvgFragment svgFragment)
        {
            if (SetOpacity(skCanvas, svgFragment) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgFragment.Transforms);

            float x = svgFragment.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
            float y = svgFragment.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
            float width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
            float height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);

            SetSvgViewBoxTransform(skCanvas, svgFragment.ViewBox, svgFragment.AspectRatio, x, y, width, height);

            DrawSvgElementCollection(skCanvas, svgFragment.Children, skSize);

            skCanvas.Restore();
        }

        private static void DrawSvgSymbol(SKCanvas skCanvas, SKSize skSize, SvgSymbol svgSymbol)
        {
            if (SetOpacity(skCanvas, svgSymbol) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgSymbol.Transforms);

            float x = 0f;
            float y = 0f;
            float width = svgSymbol.ViewBox.Width;
            float height = svgSymbol.ViewBox.Height;

            if (svgSymbol.CustomAttributes.TryGetValue("width", out string _widthString))
            {
                if (new SvgUnitConverter().ConvertFrom(_widthString) is SvgUnit _width)
                {
                    width = _width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgSymbol);
                }
            }

            if (svgSymbol.CustomAttributes.TryGetValue("height", out string heightString))
            {
                if (new SvgUnitConverter().ConvertFrom(heightString) is SvgUnit _height)
                {
                    height = _height.ToDeviceValue(null, UnitRenderingType.Vertical, svgSymbol);
                }
            }

            SetSvgViewBoxTransform(skCanvas, svgSymbol.ViewBox, svgSymbol.AspectRatio, x, y, width, height);

            DrawSvgElementCollection(skCanvas, svgSymbol.Children, skSize);

            skCanvas.Restore();
        }

        private static void DrawSvgUse(SKCanvas skCanvas, SKSize skSize, SvgUse svgUse)
        {
            var svgVisualElement = GetReference<SvgVisualElement>(svgUse, svgUse.ReferencedElement);
            if (svgVisualElement != null)
            {
                var parent = svgUse.Parent;
                //svgVisualElement.Parent = svgUse;
                var _parent = svgUse.GetType().GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance);
                _parent.SetValue(svgVisualElement, svgUse);
                svgVisualElement.InvalidateChildPaths();

                if (SetOpacity(skCanvas, svgUse) == null)
                {
                    skCanvas.Save();
                }
                SetTransform(skCanvas, svgUse.Transforms);

                float x = svgUse.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgUse);
                float y = svgUse.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgUse);
                skCanvas.Translate(x, y);

                var ew = svgUse.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgUse);
                var eh = svgUse.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgUse);
                if (ew > 0 && eh > 0)
                {
                    var _attributes = svgVisualElement.GetType().GetField("_attributes", BindingFlags.NonPublic | BindingFlags.Instance);
                    var attributes = _attributes.GetValue(_attributes) as SvgAttributeCollection;
                    var viewBox = attributes.GetAttribute<SvgViewBox>("viewBox");
                    //var viewBox = svgVisualElement.Attributes.GetAttribute<SvgViewBox>("viewBox");
                    if (viewBox != SvgViewBox.Empty && Math.Abs(ew - viewBox.Width) > float.Epsilon && Math.Abs(eh - viewBox.Height) > float.Epsilon)
                    {
                        var sw = ew / viewBox.Width;
                        var sh = eh / viewBox.Height;
                        skCanvas.Translate(sw, sh);
                    }
                }

                if (svgVisualElement is SvgSymbol)
                {
                    DrawSvgSymbol(skCanvas, skSize, svgVisualElement as SvgSymbol);
                }
                else
                {
                    DrawSvgElement(skCanvas, skSize, svgVisualElement);
                }

                //svgVisualElement.Parent = parent;
                _parent.SetValue(svgVisualElement, parent);

                skCanvas.Restore();
            }
        }

        private static void DrawSvgCircle(SKCanvas skCanvas, SKSize skSize, SvgCircle svgCircle)
        {
            if (SetOpacity(skCanvas, svgCircle) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgCircle.Transforms);

            float cx = svgCircle.CenterX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgCircle);
            float cy = svgCircle.CenterY.ToDeviceValue(null, UnitRenderingType.Vertical, svgCircle);
            float radius = svgCircle.Radius.ToDeviceValue(null, UnitRenderingType.Other, svgCircle);

            var skBounds = SKRect.Create(cx - radius, cy - radius, radius + radius, radius + radius);

            if (svgCircle.Fill != null)
            {
                using (var skPaint = GetFillSKPaint(svgCircle, skSize))
                {
                    skCanvas.DrawCircle(cx, cy, radius, skPaint);
                }
            }

            if (svgCircle.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgCircle, skSize))
                {
                    skCanvas.DrawCircle(cx, cy, radius, skPaint);
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgEllipse(SKCanvas skCanvas, SKSize skSize, SvgEllipse svgEllipse)
        {
            if (SetOpacity(skCanvas, svgEllipse) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgEllipse.Transforms);

            float cx = svgEllipse.CenterX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgEllipse);
            float cy = svgEllipse.CenterY.ToDeviceValue(null, UnitRenderingType.Vertical, svgEllipse);
            float rx = svgEllipse.RadiusX.ToDeviceValue(null, UnitRenderingType.Other, svgEllipse);
            float ry = svgEllipse.RadiusY.ToDeviceValue(null, UnitRenderingType.Other, svgEllipse);

            var skBounds = SKRect.Create(cx - rx, cy - ry, rx + rx, ry + ry);

            if (svgEllipse.Fill != null)
            {
                using (var skPaint = GetFillSKPaint(svgEllipse, skSize))
                {
                    skCanvas.DrawOval(cx, cy, rx, ry, skPaint);
                }
            }

            if (svgEllipse.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgEllipse, skSize))
                {
                    skCanvas.DrawOval(cx, cy, rx, ry, skPaint);
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgRectangle(SKCanvas skCanvas, SKSize skSize, SvgRectangle svgRectangle)
        {
            if (SetOpacity(skCanvas, svgRectangle) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgRectangle.Transforms);

            float x = svgRectangle.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
            float y = svgRectangle.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
            float width = svgRectangle.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
            float height = svgRectangle.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
            float rx = svgRectangle.CornerRadiusX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
            float ry = svgRectangle.CornerRadiusY.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
            bool isRound = rx > 0f && ry > 0f;

            var skBounds = SKRect.Create(x, y, width, height);

            if (svgRectangle.Fill != null)
            {
                using (var skPaint = GetFillSKPaint(svgRectangle, skSize))
                {
                    if (isRound)
                    {
                        skCanvas.DrawRoundRect(skBounds, rx, ry, skPaint);
                    }
                    else
                    {
                        skCanvas.DrawRect(skBounds, skPaint);
                    }
                }
            }

            if (svgRectangle.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgRectangle, skSize))
                {
                    if (isRound)
                    {
                        skCanvas.DrawRoundRect(skBounds, rx, ry, skPaint);
                    }
                    else
                    {
                        skCanvas.DrawRect(skBounds, skPaint);
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgGroup(SKCanvas skCanvas, SKSize skSize, SvgGroup svgGroup)
        {
            if (SetOpacity(skCanvas, svgGroup) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgGroup.Transforms);

            DrawSvgElementCollection(skCanvas, svgGroup.Children, skSize);

            skCanvas.Restore();
        }

        private static void DrawSvgLine(SKCanvas skCanvas, SKSize skSize, SvgLine svgLine)
        {
            if (SetOpacity(skCanvas, svgLine) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgLine.Transforms);

            float x0 = svgLine.StartX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLine);
            float y0 = svgLine.StartY.ToDeviceValue(null, UnitRenderingType.Vertical, svgLine);
            float x1 = svgLine.EndX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLine);
            float y1 = svgLine.EndY.ToDeviceValue(null, UnitRenderingType.Vertical, svgLine);

            float x = Math.Min(x0, x1);
            float y = Math.Min(y0, y1);
            float width = Math.Abs(x0 - x1);
            float height = Math.Abs(y0 - y1);
            var skBounds = SKRect.Create(x, y, width, height);

            if (svgLine.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgLine, skSize))
                {
                    skCanvas.DrawLine(x0, y0, x1, y1, skPaint);
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgPath(SKCanvas skCanvas, SKSize skSize, SvgPath svgPath)
        {
            if (SetOpacity(skCanvas, svgPath) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgPath.Transforms);

            using (var skPath = ToSKPath(svgPath.PathData, svgPath.FillRule))
            {
                if (skPath != null && !skPath.IsEmpty)
                {
                    var skBounds = skPath.Bounds;

                    if (svgPath.Fill != null)
                    {
                        using (var skPaint = GetFillSKPaint(svgPath, skSize))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }

                    if (svgPath.Stroke != null)
                    {
                        using (var skPaint = GetStrokeSKPaint(svgPath, skSize))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgPolyline(SKCanvas skCanvas, SKSize skSize, SvgPolyline svgPolyline)
        {
            if (SetOpacity(skCanvas, svgPolyline) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgPolyline.Transforms);

            using (var skPath = ToSKPath(svgPolyline.Points, svgPolyline.FillRule, false))
            {
                if (skPath != null && !skPath.IsEmpty)
                {
                    var skBounds = skPath.Bounds;

                    if (svgPolyline.Fill != null)
                    {
                        using (var skPaint = GetFillSKPaint(svgPolyline, skSize))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }

                    if (svgPolyline.Stroke != null)
                    {
                        using (var skPaint = GetStrokeSKPaint(svgPolyline, skSize))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgPolygon(SKCanvas skCanvas, SKSize skSize, SvgPolygon svgPolygon)
        {
            if (SetOpacity(skCanvas, svgPolygon) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgPolygon.Transforms);

            using (var skPath = ToSKPath(svgPolygon.Points, svgPolygon.FillRule, true))
            {
                if (skPath != null && !skPath.IsEmpty)
                {
                    var skBounds = skPath.Bounds;

                    if (svgPolygon.Fill != null)
                    {
                        using (var skPaint = GetFillSKPaint(svgPolygon, skSize))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }

                    if (svgPolygon.Stroke != null)
                    {
                        using (var skPaint = GetStrokeSKPaint(svgPolygon, skSize))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgElement(SKCanvas skCanvas, SKSize skSize, SvgElement svgElement)
        {
            switch (svgElement)
            {
                case SvgFragment svgFragment:
                    {
                        DrawSvgFragment(skCanvas, skSize, svgFragment);
                    }
                    break;
                case SvgSymbol svgSymbol:
                    {
                        // The symbol defs are not rendered.
                    }
                    break;
                case SvgUse svgUse:
                    {
                        DrawSvgUse(skCanvas, skSize, svgUse);
                    }
                    break;
                case SvgCircle svgCircle:
                    {
                        DrawSvgCircle(skCanvas, skSize, svgCircle);
                    }
                    break;
                case SvgEllipse svgEllipse:
                    {
                        DrawSvgEllipse(skCanvas, skSize, svgEllipse);
                    }
                    break;
                case SvgRectangle svgRectangle:
                    {
                        DrawSvgRectangle(skCanvas, skSize, svgRectangle);
                    }
                    break;
                case SvgGroup svgGroup:
                    {
                        DrawSvgGroup(skCanvas, skSize, svgGroup);
                    }
                    break;
                case SvgLine svgLine:
                    {
                        DrawSvgLine(skCanvas, skSize, svgLine);
                    }
                    break;
                case SvgPath svgPath:
                    {
                        DrawSvgPath(skCanvas, skSize, svgPath);
                    }
                    break;
                case SvgPolyline svgPolyline:
                    {
                        DrawSvgPolyline(skCanvas, skSize, svgPolyline);
                    }
                    break;
                case SvgPolygon svgPolygon:
                    {
                        DrawSvgPolygon(skCanvas, skSize, svgPolygon);
                    }
                    break;
            }
        }

        private static void DrawSvgElementCollection(SKCanvas canvas, SvgElementCollection svgElementCollection, SKSize skSize)
        {
            foreach (var svgElement in svgElementCollection)
            {
                DrawSvgElement(canvas, skSize, svgElement);
            }
        }

        public static void SaveImage(SvgElement svgElement, string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100, float scaleX = 1f, float scaleY = 1f)
        {
            if (svgElement is SvgFragment svgFragment)
            {
                float width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
                float height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
                var skSize = new SKSize(width, height);
                var skImageInfo = new SKImageInfo((int)(width * scaleX), (int)(height * scaleY));

                using (var skBitmap = new SKBitmap(skImageInfo))
                {
                    using (var skCanvas = new SKCanvas(skBitmap))
                    {
                        skCanvas.Save();
                        skCanvas.Scale(scaleX, scaleY);
                        skCanvas.Clear(SKColors.Transparent);

                        DrawSvgElement(skCanvas, skSize, svgFragment);

                        skCanvas.Restore();

                        using (var skImage = SKImage.FromBitmap(skBitmap))
                        using (var skData = skImage.Encode(format, quality))
                        {
                            if (skData != null)
                            {
                                using (var stream = File.OpenWrite(path))
                                {
                                    skData.SaveTo(stream);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
