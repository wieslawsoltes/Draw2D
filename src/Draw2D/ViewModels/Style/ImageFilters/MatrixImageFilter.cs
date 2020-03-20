using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class MatrixImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public MatrixImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new MatrixImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
