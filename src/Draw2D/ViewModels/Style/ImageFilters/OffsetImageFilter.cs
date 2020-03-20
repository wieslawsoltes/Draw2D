using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class OffsetImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public OffsetImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new OffsetImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
