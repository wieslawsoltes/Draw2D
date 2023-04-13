using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class PaintImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public PaintImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new PaintImageFilter()
        {
            Title = this.Title
        };
    }
}