// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// Parts of this source file are adapted from the https://github.com/vvvv/SVG
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using SkiaSharp;
using Svg;
using Svg.Document_Structure;
using Svg.FilterEffects;
using Svg.Pathing;
using Svg.Transforms;

[assembly: CLSCompliant(false)]

namespace Svg.Skia
{
    internal class CompositeDisposable : IDisposable
    {
        public IList<IDisposable> Disposables { get; private set; }

        public CompositeDisposable()
        {
            Disposables = new List<IDisposable>();
        }

        public void Dispose()
        {
            foreach (var disposable in Disposables)
            {
                disposable?.Dispose();
            }
            Disposables = null;
        }
    }

    public class Svg : IDisposable
    {
        public SKPicture Picture { get; set; }

        public SvgDocument Document { get; set; }

        private static T GetReference<T>(SvgElement svgElement, Uri uri) where T : SvgElement
        {
            if (uri == null)
            {
                return null;
            }

            var svgElementById = svgElement.OwnerDocument?.GetElementById(uri.ToString());
            if (svgElementById != null)
            {
                return svgElementById as T;
            }

            return null;
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

        private static SKPath ToSKPath(string svgPath)
        {
            return SKPath.ParseSvgPathData(svgPath);
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
            return svgUnit.Type == SvgUnitType.Percentage && svgCoordinateUnits == SvgCoordinateUnits.ObjectBoundingBox ?
                    new SvgUnit(SvgUnitType.User, svgUnit.Value / 100) : svgUnit;
        }

        private static SKPathEffect CreateDash(SvgElement svgElement, float strokeWidth)
        {
            var strokeDashArray = svgElement.StrokeDashArray;
            var count = strokeDashArray.Count;

            if (strokeDashArray != null && count > 0)
            {
                bool isOdd = count % 2 != 0;

                strokeWidth = strokeWidth <= 0 ? 1 : strokeWidth;

                float[] intervals = new float[isOdd ? count * 2 : count];
                for (int i = 0; i < count; i++)
                {
                    var dash = strokeDashArray[i].ToDeviceValue(null, UnitRenderingType.Other, svgElement);
                    var interval = (dash <= 0) ? 1 : dash;
                    intervals[i] = interval / strokeWidth;
                }

                if (isOdd)
                {
                    for (int i = 0; i < count; i++)
                    {
                        intervals[i + count] = intervals[i];
                    }
                }

                var dashOffset = svgElement.StrokeDashOffset != null ? svgElement.StrokeDashOffset : 0;
                var phase = 0f;
                if (dashOffset != 0)
                {
                    var dashOffsetValue = dashOffset.ToDeviceValue(null, UnitRenderingType.Other, svgElement);
                    phase = (dashOffsetValue <= 0) ? 1 : dashOffsetValue / strokeWidth;
                }

                return SKPathEffect.CreateDash(intervals, phase);
            }

            return null;
        }

        private static void GetStops(SvgGradientServer svgGradientServer, SKSize skSize, List<SKColor> colors, List<float> colorPos, SvgVisualElement svgVisualElement)
        {
            foreach (var child in svgGradientServer.Children)
            {
                if (child is SvgGradientStop svgGradientStop)
                {
                    if (svgGradientStop.StopColor is SvgColourServer stopColorSvgColourServer)
                    {
                        var stopColor = GetColor(stopColorSvgColourServer, AdjustSvgOpacity(svgGradientStop.Opacity), false);
                        float offset = svgGradientStop.Offset.ToDeviceValue(null, UnitRenderingType.Horizontal, svgGradientServer);
                        offset /= skSize.Width;
                        colors.Add(stopColor);
                        colorPos.Add(offset);
                    }
                }
            }


            var inheritGradient = SvgDeferredPaintServer.TryGet<SvgGradientServer>(svgGradientServer.InheritGradient, svgVisualElement);
            if (colors.Count == 0 && inheritGradient != null)
            {
                GetStops(inheritGradient, skSize, colors, colorPos, svgVisualElement);
            }

        }

        private static SKShader CreateLinearGradient(SvgLinearGradientServer svgLinearGradientServer, SKSize skSize, SKRect skBounds, SvgVisualElement svgVisualElement)
        {
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

            GetStops(svgLinearGradientServer, skSize, colors, colorPos, svgVisualElement);

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

            SKShader sKShader;

            if (svgLinearGradientServer.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
            {
                var skBoundingBoxTransform = new SKMatrix()
                {
                    ScaleX = skBounds.Width,
                    SkewY = 0f,
                    SkewX = 0f,
                    ScaleY = skBounds.Height,
                    TransX = skBounds.Left,
                    TransY = skBounds.Top,
                    Persp0 = 0,
                    Persp1 = 0,
                    Persp2 = 1
                };

                if (svgLinearGradientServer.GradientTransform != null && svgLinearGradientServer.GradientTransform.Count > 0)
                {
                    var gradientTransform = GetSKMatrix(svgLinearGradientServer.GradientTransform);
                    SKMatrix.Concat(ref skBoundingBoxTransform, ref skBoundingBoxTransform, ref gradientTransform);
                }

                sKShader = SKShader.CreateLinearGradient(skStart, skEnd, skColors, skColorPos, shaderTileMode, skBoundingBoxTransform);
            }
            else
            {
                if (svgLinearGradientServer.GradientTransform != null && svgLinearGradientServer.GradientTransform.Count > 0)
                {
                    var gradientTransform = GetSKMatrix(svgLinearGradientServer.GradientTransform);
                    sKShader = SKShader.CreateLinearGradient(skStart, skEnd, skColors, skColorPos, shaderTileMode, gradientTransform);
                }
                else
                {
                    sKShader = SKShader.CreateLinearGradient(skStart, skEnd, skColors, skColorPos, shaderTileMode);
                }
            }

            return sKShader;
        }

        private static SKShader CreateTwoPointConicalGradient(SvgRadialGradientServer svgRadialGradientServer, SKSize skSize, SKRect skBounds, SvgVisualElement svgVisualElement)
        {
            var skStart = new SKPoint(
                NormalizeSvgUnit(svgRadialGradientServer.CenterX, svgRadialGradientServer.GradientUnits)
                    .ToDeviceValue(null, UnitRenderingType.Horizontal, svgRadialGradientServer),
                NormalizeSvgUnit(svgRadialGradientServer.CenterY, svgRadialGradientServer.GradientUnits)
                    .ToDeviceValue(null, UnitRenderingType.Vertical, svgRadialGradientServer));
            var skEnd = new SKPoint(
                NormalizeSvgUnit(svgRadialGradientServer.FocalX, svgRadialGradientServer.GradientUnits)
                    .ToDeviceValue(null, UnitRenderingType.Horizontal, svgRadialGradientServer),
                NormalizeSvgUnit(svgRadialGradientServer.FocalY, svgRadialGradientServer.GradientUnits)
                    .ToDeviceValue(null, UnitRenderingType.Vertical, svgRadialGradientServer));

            var startRadius = 0f;
            var endRadius = 
                NormalizeSvgUnit(svgRadialGradientServer.Radius, svgRadialGradientServer.GradientUnits)
                    .ToDeviceValue(null, UnitRenderingType.Other, svgRadialGradientServer);

            var colors = new List<SKColor>();
            var colorPos = new List<float>();

            GetStops(svgRadialGradientServer, skSize, colors, colorPos, svgVisualElement);

            SKShaderTileMode shaderTileMode;
            switch (svgRadialGradientServer.SpreadMethod)
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

            var skColors = colors.ToArray();
            float[] skColorPos = colorPos.ToArray();

            SKShader sKShader;

            if (svgRadialGradientServer.GradientUnits == SvgCoordinateUnits.ObjectBoundingBox)
            {
                var skBoundingBoxTransform = new SKMatrix()
                {
                    ScaleX = skBounds.Width,
                    SkewY = 0f,
                    SkewX = 0f,
                    ScaleY = skBounds.Height,
                    TransX = skBounds.Left,
                    TransY = skBounds.Top,
                    Persp0 = 0,
                    Persp1 = 0,
                    Persp2 = 1
                };

                if (svgRadialGradientServer.GradientTransform != null && svgRadialGradientServer.GradientTransform.Count > 0)
                {
                    var gradientTransform = GetSKMatrix(svgRadialGradientServer.GradientTransform);
                    SKMatrix.Concat(ref skBoundingBoxTransform, ref skBoundingBoxTransform, ref gradientTransform);
                }

                sKShader = SKShader.CreateTwoPointConicalGradient(skStart, startRadius, skEnd, endRadius, skColors, skColorPos, shaderTileMode, skBoundingBoxTransform);
            }
            else
            {
                if (svgRadialGradientServer.GradientTransform != null && svgRadialGradientServer.GradientTransform.Count > 0)
                {
                    var gradientTransform = GetSKMatrix(svgRadialGradientServer.GradientTransform);
                    sKShader = SKShader.CreateTwoPointConicalGradient(skStart, startRadius, skEnd, endRadius, skColors, skColorPos, shaderTileMode, gradientTransform);
                }
                else
                {
                    sKShader = SKShader.CreateTwoPointConicalGradient(skStart, startRadius, skEnd, endRadius, skColors, skColorPos, shaderTileMode);
                }
            }

            return sKShader;
        }

        private static void SetFill(SvgVisualElement svgVisualElement, SKSize skSize, SKRect skBounds, SKPaint skPaint, CompositeDisposable disposable)
        {
            switch (svgVisualElement.Fill)
            {
                case SvgColourServer svgColourServer:
                    {
                        skPaint.Color = GetColor(svgColourServer, AdjustSvgOpacity(svgVisualElement.FillOpacity), false);
                    }
                    break;
                case SvgPatternServer svgPatternServer:
                    {
                        // TODO:
                    }
                    break;
                case SvgLinearGradientServer svgLinearGradientServer:
                    {
                        var skShader = CreateLinearGradient(svgLinearGradientServer, skSize, skBounds, svgVisualElement);
                        if (skShader != null)
                        {
                            disposable.Disposables.Add(skShader);
                            skPaint.Shader = skShader;
                        }
                    }
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    {
                        var skShader = CreateTwoPointConicalGradient(svgRadialGradientServer, skSize, skBounds, svgVisualElement);
                        if (skShader != null)
                        {
                            disposable.Disposables.Add(skShader);
                            skPaint.Shader = skShader;
                        }
                    }
                    break;
                case SvgDeferredPaintServer svgDeferredPaintServer:
                    // Not used.
                    break;
                case SvgFallbackPaintServer svgFallbackPaintServer:
                    // Not used.
                    break;
                default:
                    break;
            }
        }

        private static void SetStroke(SvgVisualElement svgVisualElement, SKSize skSize, SKRect skBounds, SKPaint skPaint, CompositeDisposable disposable)
        {
            switch (svgVisualElement.Stroke)
            {
                case SvgColourServer svgColourServer:
                    {
                        skPaint.Color = GetColor(svgColourServer, AdjustSvgOpacity(svgVisualElement.StrokeOpacity), true);
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
                        skPaint.Shader = CreateLinearGradient(svgLinearGradientServer, skSize, skBounds, svgVisualElement);
                    }
                    break;
                case SvgRadialGradientServer svgRadialGradientServer:
                    {
                        var skShader = CreateTwoPointConicalGradient(svgRadialGradientServer, skSize, skBounds, svgVisualElement);
                        if (skShader != null)
                        {
                            disposable.Disposables.Add(skShader);
                            skPaint.Shader = skShader;
                        }
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
        }

        private static void SetDash(SvgVisualElement svgVisualElement, SKPaint skPaint, CompositeDisposable disposable)
        {
            var dash = CreateDash(svgVisualElement, skPaint.StrokeWidth);
            if (dash != null)
            {
                disposable.Disposables.Add(dash);
                skPaint.PathEffect = dash;
            }
        }

        private static void SetFilter(SvgVisualElement svgVisualElement, SKPaint skPaint, CompositeDisposable disposable)
        {
            var svgFilter = GetReference<SvgFilter>(svgVisualElement, svgVisualElement.Filter);
            if (svgFilter == null)
            {
                return;
            }

            foreach (var child in svgFilter.Children)
            {
                if (child is SvgFilterPrimitive svgFilterPrimitive)
                {
                    switch (svgFilterPrimitive)
                    {
                        case SvgColourMatrix svgColourMatrix:
                            {
                                // TODO:
                            }
                            break;
                        case SvgGaussianBlur svgGaussianBlur:
                            {
                                // TODO:
                                var sigma = svgGaussianBlur.StdDeviation;
                                var skImageFilter = SKImageFilter.CreateBlur(sigma, sigma);
                                if (skImageFilter != null)
                                {
                                    disposable.Disposables.Add(skImageFilter);
                                    skPaint.ImageFilter = skImageFilter;
                                }
                            }
                            break;
                        case SvgMerge svgMerge:
                            {
                                // TODO:
                            }
                            break;
                        case SvgOffset svgOffset:
                            {
                                // TODO:
                            }
                            break;
                        default:
                            {
                                // TODO:
                            }
                            break;
                    }
                }
            }
        }

        private static bool IsAntialias(SvgElement svgElement)
        {
            switch (svgElement.ShapeRendering)
            {
                case SvgShapeRendering.Inherit:
                case SvgShapeRendering.Auto:
                default:
                    return true;
                case SvgShapeRendering.OptimizeSpeed:
                case SvgShapeRendering.CrispEdges:
                case SvgShapeRendering.GeometricPrecision:
                    return false;
            }
        }

        private static SKPaint GetFillSKPaint(SvgVisualElement svgVisualElement, SKSize skSize, SKRect skBounds, CompositeDisposable disposable)
        {
            var skPaint = new SKPaint()
            {
                IsAntialias = IsAntialias(svgVisualElement)
            };

            // TODO: SvgElement

            // TODO: SvgElementStyle

            if (svgVisualElement.Fill != null)
            {
                SetFill(svgVisualElement, skSize, skBounds, skPaint, disposable);
            }

            // TODO: SvgVisualElement

            if (svgVisualElement.Filter != null)
            {
                SetFilter(svgVisualElement, skPaint, disposable);
            }

            // TODO: SvgVisualElementStyle

            skPaint.Style = SKPaintStyle.Fill;

            return skPaint;
        }

        private static SKPaint GetStrokeSKPaint(SvgVisualElement svgVisualElement, SKSize skSize, SKRect skBounds, CompositeDisposable disposable)
        {
            var skPaint = new SKPaint()
            {
                IsAntialias = IsAntialias(svgVisualElement)
            };

            // TODO: SvgElement

            // TODO: SvgElementStyle

            if (svgVisualElement.Stroke != null)
            {
                SetStroke(svgVisualElement, skSize, skBounds, skPaint, disposable);
            }

            switch (svgVisualElement.StrokeLineCap)
            {
                case SvgStrokeLineCap.Butt:
                    skPaint.StrokeCap = SKStrokeCap.Butt;
                    break;
                case SvgStrokeLineCap.Round:
                    skPaint.StrokeCap = SKStrokeCap.Round;
                    break;
                case SvgStrokeLineCap.Square:
                    skPaint.StrokeCap = SKStrokeCap.Square;
                    break;
            }

            switch (svgVisualElement.StrokeLineJoin)
            {
                case SvgStrokeLineJoin.Miter:
                    skPaint.StrokeJoin = SKStrokeJoin.Miter;
                    break;
                case SvgStrokeLineJoin.Round:
                    skPaint.StrokeJoin = SKStrokeJoin.Round;
                    break;
                case SvgStrokeLineJoin.Bevel:
                    skPaint.StrokeJoin = SKStrokeJoin.Bevel;
                    break;
            }

            skPaint.StrokeMiter = svgVisualElement.StrokeMiterLimit;

            skPaint.StrokeWidth = svgVisualElement.StrokeWidth.ToDeviceValue(null, UnitRenderingType.Other, svgVisualElement);

            if (svgVisualElement.StrokeDashArray != null)
            {
                SetDash(svgVisualElement, skPaint, disposable);
            }

            // TODO: SvgVisualElement

            if (svgVisualElement.Filter != null)
            {
                SetFilter(svgVisualElement, skPaint, disposable);
            }

            // TODO: SvgVisualElementStyle

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
                            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgRotate svgRotate:
                        {
                            var skMatrix = SKMatrix.MakeRotationDegrees(svgRotate.Angle, svgRotate.CenterX, svgRotate.CenterY);
                            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgScale svgScale:
                        {
                            var skMatrix = SKMatrix.MakeScale(svgScale.X, svgScale.Y);
                            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
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
                            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
                        }
                        break;
                    case SvgTranslate svgTranslate:
                        {
                            var skMatrix = SKMatrix.MakeTranslation(svgTranslate.X, svgTranslate.Y);
                            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
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
            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
            skCanvas.SetMatrix(totalSKMatrix);
        }

        private static void SetTransform(SKCanvas skCanvas, SKMatrix skMatrix)
        {
            var totalSKMatrix = skCanvas.TotalMatrix;
            SKMatrix.Concat(ref totalSKMatrix, ref totalSKMatrix, ref skMatrix);
            skCanvas.SetMatrix(totalSKMatrix);
        }

        private static SKPaint SetOpacity(SKCanvas skCanvas, SvgElement svgElement, CompositeDisposable disposable)
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
                disposable.Disposables.Add(paint);
                return paint;
            }

            return null;
        }

        private static bool ElementReferencesUri(SvgUse svgUse, SvgElement element, List<Uri> elementUris)
        {
            if (element is SvgUse useElement)
            {
                if (elementUris.Contains(useElement.ReferencedElement))
                {
                    return true;
                }
                if (svgUse.OwnerDocument.GetElementById(useElement.ReferencedElement.ToString()) is SvgUse refElement)
                {
                    elementUris.Add(useElement.ReferencedElement);
                }
                return ReferencedElementReferencesUri(useElement, elementUris);
            }
            if (element is SvgGroup groupElement)
            {
                foreach (var child in groupElement.Children)
                {
                    if (ElementReferencesUri(svgUse, child, elementUris))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool ReferencedElementReferencesUri(SvgUse svgUse, List<Uri> elementUris)
        {
            var refElement = svgUse.OwnerDocument.GetElementById(svgUse.ReferencedElement.ToString());
            return ElementReferencesUri(svgUse, refElement, elementUris);
        }

        private static bool HasRecursiveReference(SvgUse svgUse)
        {
            var refElement = svgUse.OwnerDocument.GetElementById(svgUse.ReferencedElement.ToString());
            var uris = new List<Uri>() { svgUse.ReferencedElement };
            return ElementReferencesUri(svgUse, refElement, uris);
        }

        private static void DrawSvgFragment(SKCanvas skCanvas, SKSize skSize, SvgFragment svgFragment, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgFragment, disposable) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgFragment.Transforms);

            float x = svgFragment.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
            float y = svgFragment.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
            float width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
            float height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);

            SetSvgViewBoxTransform(skCanvas, svgFragment.ViewBox, svgFragment.AspectRatio, x, y, width, height);

            DrawSvgElementCollection(skCanvas, skSize, svgFragment.Children, disposable);

            skCanvas.Restore();
        }

        private static void DrawSvgSymbol(SKCanvas skCanvas, SKSize skSize, SvgSymbol svgSymbol, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgSymbol, disposable) == null)
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

            DrawSvgElementCollection(skCanvas, skSize, svgSymbol.Children, disposable);

            skCanvas.Restore();
        }

        private static void DrawSvgImage(SKCanvas skCanvas, SKSize skSize, SvgImage svgImage, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgSwitch(SKCanvas skCanvas, SKSize skSize, SvgSwitch svgSwitch, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgUse(SKCanvas skCanvas, SKSize skSize, SvgUse svgUse, CompositeDisposable disposable)
        {
            var svgVisualElement = GetReference<SvgVisualElement>(svgUse, svgUse.ReferencedElement);
            if (svgVisualElement != null && !HasRecursiveReference(svgUse))
            {
                var parent = svgUse.Parent;
                //svgVisualElement.Parent = svgUse;
                var _parent = svgUse.GetType().GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance);
                if (_parent != null)
                {
                    _parent.SetValue(svgVisualElement, svgUse);
                }
                //else
                //{
                //    throw new Exception("Can not set 'use' referenced element parent.");
                //}

                svgVisualElement.InvalidateChildPaths();

                if (SetOpacity(skCanvas, svgUse, disposable) == null)
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
                    if (_attributes != null)
                    {
                        var attributes = _attributes.GetValue(svgVisualElement) as SvgAttributeCollection;
                        if (attributes != null)
                        {
                            var viewBox = attributes.GetAttribute<SvgViewBox>("viewBox");
                            //var viewBox = svgVisualElement.Attributes.GetAttribute<SvgViewBox>("viewBox");
                            if (viewBox != SvgViewBox.Empty && Math.Abs(ew - viewBox.Width) > float.Epsilon && Math.Abs(eh - viewBox.Height) > float.Epsilon)
                            {
                                var sw = ew / viewBox.Width;
                                var sh = eh / viewBox.Height;
                                skCanvas.Translate(sw, sh);
                            }
                        }
                    }
                    //else
                    //{
                    //    throw new Exception("Can not get 'use' referenced element transform.");
                    //}
                }

                /*
                if (svgUse.ClipPath != null)
                {
                    var svgClipPath = svgVisualElement.OwnerDocument.GetElementById<SvgClipPath>(svgUse.ClipPath.ToString());
                    if (svgClipPath != null && svgClipPath.Children != null)
                    {
                        foreach (var child in svgClipPath.Children)
                        {
                            var skPath = new SKPath();
                        }

                        // TODO:

                        Console.WriteLine($"clip-path: {svgClipPath}");
                    }
                }
                */

                if (svgVisualElement is SvgSymbol)
                {
                    DrawSvgSymbol(skCanvas, skSize, svgVisualElement as SvgSymbol, disposable);
                }
                else
                {
                    DrawSvgElement(skCanvas, skSize, svgVisualElement, disposable);
                }

                //svgVisualElement.Parent = parent;
                if (_parent != null)
                {
                    _parent.SetValue(svgVisualElement, parent);
                }
                //else
                //{
                //    throw new Exception("Can not set 'use' referenced element parent.");
                //}

                skCanvas.Restore();
            }
        }

        private static void DrawSvgForeignObject(SKCanvas skCanvas, SKSize skSize, SvgForeignObject svgForeignObject, CompositeDisposable disposable)
        {
            // TODO:
        }

        public struct Circle
        {
            public float cx;
            public float cy;
            public float radius;
            public SKRect bounds;
            public SKMatrix matrix;

            public Circle(SvgCircle svgCircle)
            {
                cx = svgCircle.CenterX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgCircle);
                cy = svgCircle.CenterY.ToDeviceValue(null, UnitRenderingType.Vertical, svgCircle);
                radius = svgCircle.Radius.ToDeviceValue(null, UnitRenderingType.Other, svgCircle);
                bounds = SKRect.Create(cx - radius, cy - radius, radius + radius, radius + radius);
                matrix = GetSKMatrix(svgCircle.Transforms);
            }
        }

        private static void DrawSvgCircle(SKCanvas skCanvas, SKSize skSize, SvgCircle svgCircle, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgCircle, disposable) == null)
            {
                skCanvas.Save();
            }

            var circle = new Circle(svgCircle);

            SetTransform(skCanvas, circle.matrix);

            if (svgCircle.Fill != null)
            {
                using (var skPaint = GetFillSKPaint(svgCircle, skSize, circle.bounds, disposable))
                {
                    skCanvas.DrawCircle(circle.cx, circle.cy, circle.radius, skPaint);
                }
            }

            if (svgCircle.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgCircle, skSize, circle.bounds, disposable))
                {
                    skCanvas.DrawCircle(circle.cx, circle.cy, circle.radius, skPaint);
                }
            }

            skCanvas.Restore();
        }

        public struct Ellipse
        {
            public float cx;
            public float cy;
            public float rx;
            public float ry;
            public SKRect bounds;
            public SKMatrix matrix;

            public Ellipse(SvgEllipse svgEllipse)
            {
                cx = svgEllipse.CenterX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgEllipse);
                cy = svgEllipse.CenterY.ToDeviceValue(null, UnitRenderingType.Vertical, svgEllipse);
                rx = svgEllipse.RadiusX.ToDeviceValue(null, UnitRenderingType.Other, svgEllipse);
                ry = svgEllipse.RadiusY.ToDeviceValue(null, UnitRenderingType.Other, svgEllipse);
                bounds = SKRect.Create(cx - rx, cy - ry, rx + rx, ry + ry);
                matrix = GetSKMatrix(svgEllipse.Transforms);
            }
        }

        private static void DrawSvgEllipse(SKCanvas skCanvas, SKSize skSize, SvgEllipse svgEllipse, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgEllipse, disposable) == null)
            {
                skCanvas.Save();
            }

            var ellipse = new Ellipse(svgEllipse);

            SetTransform(skCanvas, ellipse.matrix);

            if (svgEllipse.Fill != null)
            {
                using (var skPaint = GetFillSKPaint(svgEllipse, skSize, ellipse.bounds, disposable))
                {
                    skCanvas.DrawOval(ellipse.cx, ellipse.cy, ellipse.rx, ellipse.ry, skPaint);
                }
            }

            if (svgEllipse.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgEllipse, skSize, ellipse.bounds, disposable))
                {
                    skCanvas.DrawOval(ellipse.cx, ellipse.cy, ellipse.rx, ellipse.ry, skPaint);
                }
            }

            skCanvas.Restore();
        }

        public struct Rectangle
        {
            public float x;
            public float y;
            public float width;
            public float height;
            public float rx;
            public float ry;
            public bool isRound;
            public SKRect bounds;
            public SKMatrix matrix;

            public Rectangle(SvgRectangle svgRectangle)
            {
                x = svgRectangle.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
                y = svgRectangle.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
                width = svgRectangle.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
                height = svgRectangle.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
                rx = svgRectangle.CornerRadiusX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgRectangle);
                ry = svgRectangle.CornerRadiusY.ToDeviceValue(null, UnitRenderingType.Vertical, svgRectangle);
                isRound = rx > 0f && ry > 0f;
                bounds = SKRect.Create(x, y, width, height);
                matrix = GetSKMatrix(svgRectangle.Transforms);
            }
        }

        private static void DrawSvgRectangle(SKCanvas skCanvas, SKSize skSize, SvgRectangle svgRectangle, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgRectangle, disposable) == null)
            {
                skCanvas.Save();
            }

            var rectangle = new Rectangle(svgRectangle);

            SetTransform(skCanvas, rectangle.matrix);

            if (svgRectangle.Fill != null)
            {
                using (var skPaint = GetFillSKPaint(svgRectangle, skSize, rectangle.bounds, disposable))
                {
                    if (rectangle.isRound)
                    {
                        skCanvas.DrawRoundRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height, rectangle.rx, rectangle.ry, skPaint);
                    }
                    else
                    {
                        skCanvas.DrawRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height, skPaint);
                    }
                }
            }

            if (svgRectangle.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgRectangle, skSize, rectangle.bounds, disposable))
                {
                    if (rectangle.isRound)
                    {
                        skCanvas.DrawRoundRect(rectangle.bounds, rectangle.rx, rectangle.ry, skPaint);
                    }
                    else
                    {
                        skCanvas.DrawRect(rectangle.bounds, skPaint);
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgMarker(SKCanvas skCanvas, SKSize skSize, SvgMarker svgMarker, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgGlyph(SKCanvas skCanvas, SKSize skSize, SvgGlyph svgGlyph, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgGroup(SKCanvas skCanvas, SKSize skSize, SvgGroup svgGroup, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgGroup, disposable) == null)
            {
                skCanvas.Save();
            }
            SetTransform(skCanvas, svgGroup.Transforms);

            DrawSvgElementCollection(skCanvas, skSize, svgGroup.Children, disposable);

            skCanvas.Restore();
        }

        public struct Line
        {
            public float x0;
            public float y0;
            public float x1;
            public float y1;
            public SKRect bounds;
            public SKMatrix matrix;

            public Line(SvgLine svgLine)
            {
                x0 = svgLine.StartX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLine);
                y0 = svgLine.StartY.ToDeviceValue(null, UnitRenderingType.Vertical, svgLine);
                x1 = svgLine.EndX.ToDeviceValue(null, UnitRenderingType.Horizontal, svgLine);
                y1 = svgLine.EndY.ToDeviceValue(null, UnitRenderingType.Vertical, svgLine);
                float x = Math.Min(x0, x1);
                float y = Math.Min(y0, y1);
                float width = Math.Abs(x0 - x1);
                float height = Math.Abs(y0 - y1);
                bounds = SKRect.Create(x, y, width, height);
                matrix = GetSKMatrix(svgLine.Transforms);
            }
        }

        private static void DrawSvgLine(SKCanvas skCanvas, SKSize skSize, SvgLine svgLine, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgLine, disposable) == null)
            {
                skCanvas.Save();
            }

            var line = new Line(svgLine);

            SetTransform(skCanvas, line.matrix);

            if (svgLine.Stroke != null)
            {
                using (var skPaint = GetStrokeSKPaint(svgLine, skSize, line.bounds, disposable))
                {
                    skCanvas.DrawLine(line.x0, line.y0, line.x1, line.y1, skPaint);
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgPath(SKCanvas skCanvas, SKSize skSize, SvgPath svgPath, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgPath, disposable) == null)
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
                        using (var skPaint = GetFillSKPaint(svgPath, skSize, skBounds, disposable))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }

                    if (svgPath.Stroke != null)
                    {
                        using (var skPaint = GetStrokeSKPaint(svgPath, skSize, skBounds, disposable))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgPolyline(SKCanvas skCanvas, SKSize skSize, SvgPolyline svgPolyline, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgPolyline, disposable) == null)
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
                        using (var skPaint = GetFillSKPaint(svgPolyline, skSize, skBounds, disposable))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }

                    if (svgPolyline.Stroke != null)
                    {
                        using (var skPaint = GetStrokeSKPaint(svgPolyline, skSize, skBounds, disposable))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgPolygon(SKCanvas skCanvas, SKSize skSize, SvgPolygon svgPolygon, CompositeDisposable disposable)
        {
            if (SetOpacity(skCanvas, svgPolygon, disposable) == null)
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
                        using (var skPaint = GetFillSKPaint(svgPolygon, skSize, skBounds, disposable))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }

                    if (svgPolygon.Stroke != null)
                    {
                        using (var skPaint = GetStrokeSKPaint(svgPolygon, skSize, skBounds, disposable))
                        {
                            skCanvas.DrawPath(skPath, skPaint);
                        }
                    }
                }
            }

            skCanvas.Restore();
        }

        private static void DrawSvgText(SKCanvas skCanvas, SKSize skSize, SvgText svgText, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgTextPath(SKCanvas skCanvas, SKSize skSize, SvgTextPath svgTextPath, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgTextRef(SKCanvas skCanvas, SKSize skSize, SvgTextRef svgTextRef, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgTextSpan(SKCanvas skCanvas, SKSize skSize, SvgTextSpan svgTextSpan, CompositeDisposable disposable)
        {
            // TODO:
        }

        private static void DrawSvgElement(SKCanvas skCanvas, SKSize skSize, SvgElement svgElement, CompositeDisposable disposable)
        {
            switch (svgElement)
            {
                case SvgFragment svgFragment:
                    {
                        DrawSvgFragment(skCanvas, skSize, svgFragment, disposable);
                    }
                    break;
                case SvgImage svgImage:
                    {
                        DrawSvgImage(skCanvas, skSize, svgImage, disposable);
                    }
                    break;
                case SvgSwitch svgSwitch:
                    {
                        DrawSvgSwitch(skCanvas, skSize, svgSwitch, disposable);
                    }
                    break;
                case SvgUse svgUse:
                    {
                        DrawSvgUse(skCanvas, skSize, svgUse, disposable);
                    }
                    break;
                case SvgForeignObject svgForeignObject:
                    {
                        DrawSvgForeignObject(skCanvas, skSize, svgForeignObject, disposable);
                    }
                    break;
                case SvgCircle svgCircle:
                    {
                        DrawSvgCircle(skCanvas, skSize, svgCircle, disposable);
                    }
                    break;
                case SvgEllipse svgEllipse:
                    {
                        DrawSvgEllipse(skCanvas, skSize, svgEllipse, disposable);
                    }
                    break;
                case SvgRectangle svgRectangle:
                    {
                        DrawSvgRectangle(skCanvas, skSize, svgRectangle, disposable);
                    }
                    break;
                case SvgMarker svgMarker:
                    {
                        DrawSvgMarker(skCanvas, skSize, svgMarker, disposable);
                    }
                    break;
                case SvgGlyph svgGlyph:
                    {
                        DrawSvgGlyph(skCanvas, skSize, svgGlyph, disposable);
                    }
                    break;
                case SvgGroup svgGroup:
                    {
                        DrawSvgGroup(skCanvas, skSize, svgGroup, disposable);
                    }
                    break;
                case SvgLine svgLine:
                    {
                        DrawSvgLine(skCanvas, skSize, svgLine, disposable);
                    }
                    break;
                case SvgPath svgPath:
                    {
                        DrawSvgPath(skCanvas, skSize, svgPath, disposable);
                    }
                    break;
                case SvgPolyline svgPolyline:
                    {
                        DrawSvgPolyline(skCanvas, skSize, svgPolyline, disposable);
                    }
                    break;
                case SvgPolygon svgPolygon:
                    {
                        DrawSvgPolygon(skCanvas, skSize, svgPolygon, disposable);
                    }
                    break;
                case SvgText svgText:
                    {
                        DrawSvgText(skCanvas, skSize, svgText, disposable);
                    }
                    break;
                case SvgTextPath svgTextPath:
                    {
                        DrawSvgTextPath(skCanvas, skSize, svgTextPath, disposable);
                    }
                    break;
                case SvgTextRef svgTextRef:
                    {
                        DrawSvgTextRef(skCanvas, skSize, svgTextRef, disposable);
                    }
                    break;
                case SvgTextSpan svgTextSpan:
                    {
                        DrawSvgTextSpan(skCanvas, skSize, svgTextSpan, disposable);
                    }
                    break;
                default:
                    break;
            }
        }

        private static void DrawSvgElementCollection(SKCanvas canvas, SKSize skSize, SvgElementCollection svgElementCollection, CompositeDisposable disposable)
        {
            foreach (var svgElement in svgElementCollection)
            {
                DrawSvgElement(canvas, skSize, svgElement, disposable);
            }
        }

        private static SKPicture Load(SvgFragment svgFragment)
        {
            using (var disposable = new CompositeDisposable())
            {
                float width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
                float height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
                var skSize = new SKSize(width, height);
                var cullRect = SKRect.Create(skSize);
                using (var skPictureRecorder = new SKPictureRecorder())
                using (var skCanvas = skPictureRecorder.BeginRecording(cullRect))
                {
                    skCanvas.Clear(SKColors.Transparent);
                    DrawSvgElement(skCanvas, skSize, svgFragment, disposable);
                    return skPictureRecorder.EndRecording();
                }
            }
        }

        public SKPicture Load(Stream stream)
        {
            Reset();
            var svgDocument = SvgDocument.Open<SvgDocument>(stream, null);
            if (svgDocument != null)
            {
                svgDocument.FlushStyles(true);
                Picture = Load(svgDocument);
                Document = svgDocument;
                return Picture;
            }
            return null;
        }

        public SKPicture Load(string path)
        {
            Reset();
            var svgDocument = SvgDocument.Open<SvgDocument>(path, null);
            if (svgDocument != null)
            {
                svgDocument.FlushStyles(true);
                Picture = Load(svgDocument);
                Document = svgDocument;
                return Picture;
            }
            return null;
        }

        public bool Save(Stream stream, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100, float scaleX = 1f, float scaleY = 1f)
        {
            if (Picture == null)
            {
                return false;
            }

            float width = Picture.CullRect.Width * scaleX;
            float height = Picture.CullRect.Height * scaleY;

            if (width > 0 && height > 0)
            {
                var skImageInfo = new SKImageInfo((int)width, (int)height);
                using (var skBitmap = new SKBitmap(skImageInfo))
                using (var skCanvas = new SKCanvas(skBitmap))
                {
                    skCanvas.Save();
                    skCanvas.Scale(scaleX, scaleY);
                    skCanvas.DrawPicture(Picture);
                    skCanvas.Restore();
                    using (var skImage = SKImage.FromBitmap(skBitmap))
                    using (var skData = skImage.Encode(format, quality))
                    {
                        if (skData != null)
                        {
                            skData.SaveTo(stream);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool Save(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 100, float scaleX = 1f, float scaleY = 1f)
        {
            using (var stream = File.OpenWrite(path))
            {
                return Save(stream, format, quality, scaleX, scaleY);
            }
        }

        public void Reset()
        {
            if (Picture != null)
            {
                Picture.Dispose();
                Picture = null;
            }

            if (Document != null)
            {
                Document = null;
            }
        }

        public void Dispose()
        {
            Reset();
        }
    }
}
