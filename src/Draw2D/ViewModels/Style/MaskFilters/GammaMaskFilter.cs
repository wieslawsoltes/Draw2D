using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.MaskFilters
{
    [DataContract(IsReference = true)]
    public class GammaMaskFilter : ViewModelBase, IMaskFilter
    {
        // TODO:

        public GammaMaskFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new GammaMaskFilter()
            {
                Title = this.Title
            };
        }
    }
}
