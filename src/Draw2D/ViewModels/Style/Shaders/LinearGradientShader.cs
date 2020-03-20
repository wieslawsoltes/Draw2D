using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders
{
    [DataContract(IsReference = true)]
    public class LinearGradientShader : ViewModelBase, IShader
    {
        // TODO:

        public LinearGradientShader()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new LinearGradientShader()
            {
                Title = this.Title
            };
        }
    }
}
