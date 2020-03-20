using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class BlurImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public BlurImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new BlurImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
