using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders
{
    [DataContract(IsReference = true)]
    public class ComposeShader : ViewModelBase, IShader
    {
        // TODO:

        public ComposeShader()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ComposeShader()
            {
                Title = this.Title
            };
        }
    }
}
