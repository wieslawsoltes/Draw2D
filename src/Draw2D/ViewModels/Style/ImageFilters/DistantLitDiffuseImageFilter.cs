using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class DistantLitDiffuseImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public DistantLitDiffuseImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new DistantLitDiffuseImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
