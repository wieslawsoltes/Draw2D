using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class MergeImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public MergeImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new MergeImageFilter()
        {
            Title = this.Title
        };
    }
}