using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters
{
    [DataContract(IsReference = true)]
    public class LightingColorFilter : ViewModelBase, IColorFilter
    {
        // TODO:

        public LightingColorFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new LightingColorFilter()
            {
                Title = this.Title
            };
        }
    }
}
