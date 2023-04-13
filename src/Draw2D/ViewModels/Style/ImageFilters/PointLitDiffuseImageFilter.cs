using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters;

[DataContract(IsReference = true)]
public class PointLitDiffuseImageFilter : ViewModelBase, IImageFilter
{
    // TODO:

    public PointLitDiffuseImageFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new PointLitDiffuseImageFilter()
        {
            Title = this.Title
        };
    }
}