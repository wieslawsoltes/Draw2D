// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Draw2D.Presenters;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Export
{
    public class SkiaContainerImporter : IContainerImporter
    {
        internal static void ImportSvg(IToolContext context, string path, IContainerView containerView)
        {
            var svg = new SkiaSharp.Extended.Svg.SKSvg();
            using (var stream = File.Open(path, FileMode.Open))
            {
                var picture = svg.Load(stream);
                //var image = SKImage.FromPicture(picture, picture.CullRect.Size.ToSizeI());
            }
        }

        public void Import(IToolContext context, string path, IContainerView containerView)
        {
            var outputExtension = Path.GetExtension(path);

            if (string.Compare(outputExtension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ImportSvg(context, path, containerView);
            }
        }
    }
}
