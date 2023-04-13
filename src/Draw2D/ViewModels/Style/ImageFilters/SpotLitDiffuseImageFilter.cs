using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class SpotLitDiffuseImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public SpotLitDiffuseImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new SpotLitDiffuseImageFilter()
        {
            Title = this.Title
        };
    }
}