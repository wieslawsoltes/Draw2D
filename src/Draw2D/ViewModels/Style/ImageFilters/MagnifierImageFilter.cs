using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class MagnifierImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public MagnifierImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new MagnifierImageFilter()
        {
            Title = this.Title
        };
    }
}