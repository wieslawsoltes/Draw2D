﻿using System;
using System.IO;
using Draw2D.Presenters;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Export;

public class SkiaContainerExporter : IContainerExporter
{
    internal static void ExportSvg(IToolContext context, string path, IContainerView containerView)
    {
        using var wstream = new SKFileWStream(path);
        using var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)containerView.Width, (int)containerView.Height), wstream);
        using var skiaContainerPresenter = new SkiaExportContainerPresenter(context, containerView);
        skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0, 1.0);
    }

    internal static void ExportPdf(IToolContext context, string path, IContainerView containerView)
    {
        using var stream = new SKFileWStream(path);
        using var pdf = SKDocument.CreatePdf(stream, SKDocument.DefaultRasterDpi);
        using var canvas = pdf.BeginPage((float)containerView.Width, (float)containerView.Height);
        using var skiaContainerPresenter = new SkiaExportContainerPresenter(context, containerView);
        skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0, 1.0);
        pdf.Close();
    }

    internal static void ExportXps(IToolContext context, string path, IContainerView containerView)
    {
        using var stream = new SKFileWStream(path);
        using var xps = SKDocument.CreateXps(stream, SKDocument.DefaultRasterDpi);
        using var canvas = xps.BeginPage((float)containerView.Width, (float)containerView.Height);
        using var skiaContainerPresenter = new SkiaExportContainerPresenter(context, containerView);
        skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0, 1.0);
        xps.Close();
    }

    internal static void ExportSkp(IToolContext context, string path, IContainerView containerView)
    {
        var recorder = new SKPictureRecorder();
        var rect = new SKRect(0f, 0f, (float)containerView.Width, (float)containerView.Height);
        var canvas = recorder.BeginRecording(rect);
        using (var skiaContainerPresenter = new SkiaExportContainerPresenter(context, containerView))
        {
            skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0, 1.0);
        }
        var picture = recorder.EndRecording();
        var dimensions = new SKSizeI((int)containerView.Width, (int)containerView.Height);
        using (var image = SKImage.FromPicture(picture, dimensions))
        {
            var data = image.EncodedData;
            if (data != null)
            {
                using var stream = File.OpenWrite(path);
                data.SaveTo(stream);
            }
        }
        picture.Dispose();
    }

    internal static void ExportImage(IToolContext context, string path, IContainerView containerView, SKEncodedImageFormat format, int quality)
    {
        var info = new SKImageInfo((int)containerView.Width, (int)containerView.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
        using var bitmap = new SKBitmap(info);
        using (var canvas = new SKCanvas(bitmap))
        using (var skiaContainerPresenter = new SkiaExportContainerPresenter(context, containerView))
        {
            skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0, 1.0);
        }
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality);
        if (data != null)
        {
            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
        }
    }

    public void Export(IToolContext context, string path, IContainerView containerView)
    {
        try
        {
            var outputExtension = Path.GetExtension(path);

            if (string.Compare(outputExtension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportSvg(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportPdf(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".xps", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportXps(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".skp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportSkp(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".bmp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Bmp, 100);
            }
            else if (string.Compare(outputExtension, ".gif", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Gif, 100);
            }
            else if (string.Compare(outputExtension, ".ico", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Ico, 100);
            }
            else if (string.Compare(outputExtension, ".jpeg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Jpeg, 100);
            }
            else if (string.Compare(outputExtension, ".jpg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Jpeg, 100);
            }
            else if (string.Compare(outputExtension, ".png", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Png, 100);
            }
            else if (string.Compare(outputExtension, ".wbmp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Wbmp, 100);
            }
            else if (string.Compare(outputExtension, ".webp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Webp, 100);
            }
            else if (string.Compare(outputExtension, ".pkm", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Pkm, 100);
            }
            else if (string.Compare(outputExtension, ".ktx", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Ktx, 100);
            }
            else if (string.Compare(outputExtension, ".astc", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Astc, 100);
            }
            else if (string.Compare(outputExtension, ".dng", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Dng, 100);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }
}