// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Export
{
    public class SkiaContainerImporter : IContainerImporter
    {
        internal static void ImportSvg(IToolContext context, string path, IContainerView containerView)
        {
            var xml = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(xml))
            {
                var picture = SkiaHelper.ToSKPicture(xml);
                var image = SkiaHelper.ToSKImage(picture);
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
