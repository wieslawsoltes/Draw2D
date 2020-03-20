using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters
{
    [DataContract(IsReference = true)]
    public class ColorMatrixColorFilter : ViewModelBase, IColorFilter
    {
        // TODO:

        public ColorMatrixColorFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ColorMatrixColorFilter()
            {
                Title = this.Title
            };
        }
    }
}
