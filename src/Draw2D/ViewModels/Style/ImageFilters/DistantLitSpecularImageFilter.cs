using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class DistantLitSpecularImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public DistantLitSpecularImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new DistantLitSpecularImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
