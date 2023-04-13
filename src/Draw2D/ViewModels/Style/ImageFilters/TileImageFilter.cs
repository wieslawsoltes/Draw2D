using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class TileImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public TileImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new TileImageFilter()
        {
            Title = this.Title
        };
    }
}