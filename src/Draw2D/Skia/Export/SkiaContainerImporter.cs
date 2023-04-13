using System;
using System.Collections.ObjectModel;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Export;

public class SkiaContainerImporter : IContainerImporter
{
    public void Import(IToolContext context, string path, IContainerView containerView)
    {
        try
        {
            var picture = SkiaUtil.ToSKPicture(path);
            if (picture != null)
            {
                var image = new ImageShape()
                {
                    Points = new ObservableCollection<IPointShape>(),
                    StartPoint = new PointShape(0.0, 0.0, context?.DocumentContainer?.PointTemplate),
                    Point = new PointShape(picture.CullRect.Width, picture.CullRect.Height, context?.DocumentContainer?.PointTemplate),
                    Path = path,
                    StretchMode = StretchMode.Center,
                    Text = new Text(),
                    StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title,
                };

                image.StartPoint.Owner = image;
                image.Point.Owner = image;

                picture.Dispose();

                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(image);
                context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }
}