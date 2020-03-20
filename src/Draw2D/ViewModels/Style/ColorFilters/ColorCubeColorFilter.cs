using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters
{
    [DataContract(IsReference = true)]
    public class ColorCubeColorFilter : ViewModelBase, IColorFilter
    {
        // TODO:

        public ColorCubeColorFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ColorCubeColorFilter()
            {
                Title = this.Title
            };
        }
    }
}
