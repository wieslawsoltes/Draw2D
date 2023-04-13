using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class ColorFilterImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public ColorFilterImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ColorFilterImageFilter()
        {
            Title = this.Title
        };
    }
}