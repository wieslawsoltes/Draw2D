using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class PictureImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public PictureImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new PictureImageFilter()
        {
            Title = this.Title
        };
    }
}