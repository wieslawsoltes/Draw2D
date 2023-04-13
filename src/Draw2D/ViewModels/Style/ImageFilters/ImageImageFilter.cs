using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class ImageImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public ImageImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ImageImageFilter()
        {
            Title = this.Title
        };
    }
}