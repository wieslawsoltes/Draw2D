using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters
{
    [DataContract(IsReference = true)]
    public class HighContrastColorFilter : ViewModelBase, IColorFilter
    {
        // TODO:

        public HighContrastColorFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new HighContrastColorFilter()
            {
                Title = this.Title
            };
        }
    }
}
