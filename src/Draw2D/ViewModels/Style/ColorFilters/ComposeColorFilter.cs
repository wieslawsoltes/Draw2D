using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters
{
    [DataContract(IsReference = true)]
    public class ComposeColorFilter : ViewModelBase, IColorFilter
    {
        // TODO:

        public ComposeColorFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ComposeColorFilter()
            {
                Title = this.Title
            };
        }
    }
}
