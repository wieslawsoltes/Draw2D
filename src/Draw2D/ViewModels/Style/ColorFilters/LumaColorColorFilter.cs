using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters;

[DataContract(IsReference = true)]
public class LumaColorColorFilter : ViewModelBase, IColorFilter
{
    // TODO:

    public LumaColorColorFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new LumaColorColorFilter()
        {
            Title = this.Title
        };
    }
}