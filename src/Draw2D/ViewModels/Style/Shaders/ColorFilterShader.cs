using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders
{
    [DataContract(IsReference = true)]
    public class ColorFilterShader : ViewModelBase, IShader
    {
        // TODO:

        public ColorFilterShader()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ColorFilterShader()
            {
                Title = this.Title
            };
        }
    }
}
