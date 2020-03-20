using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class DisplacementMapEffectImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public DisplacementMapEffectImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new DisplacementMapEffectImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
