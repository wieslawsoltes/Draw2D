// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// Parts of this source file are adapted from the https://github.com/vvvv/SVG
using System;
using SkiaSharp;
using Svg;
using Svg.Document_Structure;

namespace Svg.Skia
{
    internal struct Fragment
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public SKRect bounds;
        public SKMatrix matrix;

        public Fragment(SvgFragment svgFragment)
        {
            x = svgFragment.X.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
            y = svgFragment.Y.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
            width = svgFragment.Width.ToDeviceValue(null, UnitRenderingType.Horizontal, svgFragment);
            height = svgFragment.Height.ToDeviceValue(null, UnitRenderingType.Vertical, svgFragment);
            bounds = SKRect.Create(x, y, width, height);

            matrix = Svg.GetSKMatrix(svgFragment.Transforms);
            var viewBoxMatrix = Svg.GetSvgViewBoxTransform(svgFragment.ViewBox, svgFragment.AspectRatio, x, y, width, height);
            SKMatrix.Concat(ref matrix, ref matrix, ref viewBoxMatrix);
        }
    }

    internal struct Symbol
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public SKRect bounds;
        public SKMatrix matrix;

        public Symbol(SvgSymbol svgSymbol)
        {
            x = 0f;
            y = 0f;
            width = svgSymbol.ViewBox.Width;
            height = svgSymbol.ViewBox.Height;

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

            bounds = SKRect.Create(x, y, width, height);

            matrix = Svg.GetSKMatrix(svgSymbol.Transforms);
            var viewBoxMatrix = Svg.GetSvgViewBoxTransform(svgSymbol.ViewBox, svgSymbol.AspectRatio, x, y, width, height);
            SKMatrix.Concat(ref matrix, ref matrix, ref viewBoxMatrix);
        }
    }

    internal struct Image
    {
        public SKMatrix matrix;

        public Image(SvgImage svgImage)
        {
            matrix = Svg.GetSKMatrix(svgImage.Transforms);
        }
    }

    internal struct Switch
    {
        public SKMatrix matrix;

        public Switch(SvgSwitch svgSwitch)
        {
            matrix = Svg.GetSKMatrix(svgSwitch.Transforms);
        }
    }

    internal struct Use
    {
        public SKMatrix matrix;

        public Use(SvgUse svgUse)
        {
            matrix = Svg.GetSKMatrix(svgUse.Transforms);
        }
    }

    internal struct ForeignObject
    {
        public SKMatrix matrix;

        public ForeignObject(SvgForeignObject svgForeignObject)
        {
            matrix = Svg.GetSKMatrix(svgForeignObject.Transforms);
        }
    }

    internal struct Circle
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
            matrix = Svg.GetSKMatrix(svgCircle.Transforms);
        }
    }

    internal struct Ellipse
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
            matrix = Svg.GetSKMatrix(svgEllipse.Transforms);
        }
    }

    internal struct Rectangle
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
            matrix = Svg.GetSKMatrix(svgRectangle.Transforms);
        }
    }

    internal struct Marker
    {
        public SKMatrix matrix;

        public Marker(SvgMarker svgMarker)
        {
            matrix = Svg.GetSKMatrix(svgMarker.Transforms);
        }
    }

    internal struct Glyph
    {
        public SKMatrix matrix;

        public Glyph(SvgGlyph svgGlyph)
        {
            matrix = Svg.GetSKMatrix(svgGlyph.Transforms);
        }
    }

    internal struct Group
    {
        public SKMatrix matrix;

        public Group(SvgGroup svgGroup)
        {
            matrix = Svg.GetSKMatrix(svgGroup.Transforms);
        }
    }

    internal struct Line
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
            matrix = Svg.GetSKMatrix(svgLine.Transforms);
        }
    }

    internal struct Path
    {
        public SKMatrix matrix;

        public Path(SvgPath svgPath)
        {
            matrix = Svg.GetSKMatrix(svgPath.Transforms);
        }
    }

    internal struct Polyline
    {
        public SKMatrix matrix;

        public Polyline(SvgPolyline svgPolyline)
        {
            matrix = Svg.GetSKMatrix(svgPolyline.Transforms);
        }
    }

    internal struct Polygon
    {
        public SKMatrix matrix;

        public Polygon(SvgPolygon svgPolygon)
        {
            matrix = Svg.GetSKMatrix(svgPolygon.Transforms);
        }
    }

    internal struct Text
    {
        public SKMatrix matrix;

        public Text(SvgText svgText)
        {
            matrix = Svg.GetSKMatrix(svgText.Transforms);
        }
    }

    internal struct TextPath
    {
        public SKMatrix matrix;

        public TextPath(SvgTextPath svgTextPath)
        {
            matrix = Svg.GetSKMatrix(svgTextPath.Transforms);
        }
    }

    internal struct TextRef
    {
        public SKMatrix matrix;

        public TextRef(SvgTextRef svgTextRef)
        {
            matrix = Svg.GetSKMatrix(svgTextRef.Transforms);
        }
    }

    internal struct TextSpan
    {
        public SKMatrix matrix;

        public TextSpan(SvgTextSpan svgTextSpan)
        {
            matrix = Svg.GetSKMatrix(svgTextSpan.Transforms);
        }
    }
}
