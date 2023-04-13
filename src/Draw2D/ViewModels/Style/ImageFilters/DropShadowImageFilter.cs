using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class DropShadowImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public DropShadowImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new DropShadowImageFilter()
        {
            Title = this.Title
        };
    }
}