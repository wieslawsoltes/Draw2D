using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.MaskFilters;

[DataContract(IsReference = true)]
public class ClipMaskFilter : ViewModelBase, IMaskFilter
{
    // TODO:

    public ClipMaskFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ClipMaskFilter()
        {
            Title = this.Title
        };
    }
}