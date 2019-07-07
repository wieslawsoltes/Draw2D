// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.ObjectModel;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Export
{
    public class SkiaContainerImporter : IContainerImporter
    {
        public void Import(IToolContext context, string path, IContainerView containerView)
        {
            try
            {
                var image = SkiaHelper.ToSKImage(path);
                if (image != null)
                {
                    var imageShape = new ImageShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        StartPoint = new PointShape(0.0, 0.0, context.PointTemplate),
                        Point = new PointShape(image.Width, image.Height, context.PointTemplate),
                        Path = path,
                        Text = new Text(),
                        StyleId = context.StyleLibrary?.CurrentItem?.Title,
                    };

                    imageShape.StartPoint.Owner = imageShape;
                    imageShape.Point.Owner = imageShape;

                    image.Dispose();

                    context.ContainerView?.CurrentContainer.Shapes.Add(imageShape);
                    context.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }
    }
}
