using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class ArithmeticImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public ArithmeticImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ArithmeticImageFilter()
        {
            Title = this.Title
        };
    }
}