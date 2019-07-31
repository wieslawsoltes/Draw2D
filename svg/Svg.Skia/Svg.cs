// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using SkiaSharp;
using Svg;

namespace Svg.Skia
{
    public class Svg : IDisposable
    {
        public SKPicture Picture { get; set; }

        public SvgDocument Document { get; set; }

        internal SKPicture Load(SvgFragment svgFragment)
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
                    SvgHelper.DrawSvgElement(skCanvas, skSize, svgFragment, disposable);
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
