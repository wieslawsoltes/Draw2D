using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class DilateImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public DilateImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new DilateImageFilter()
        {
            Title = this.Title
        };
    }
}