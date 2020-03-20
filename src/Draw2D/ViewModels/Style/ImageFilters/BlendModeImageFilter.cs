using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class BlendModeImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public BlendModeImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new BlendModeImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
