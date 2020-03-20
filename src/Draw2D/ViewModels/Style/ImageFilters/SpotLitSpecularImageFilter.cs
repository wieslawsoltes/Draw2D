using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class SpotLitSpecularImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public SpotLitSpecularImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new SpotLitSpecularImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
