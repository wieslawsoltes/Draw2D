using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders
{
    [DataContract(IsReference = true)]
    public class EmptyShader : ViewModelBase, IShader
    {
        // TODO:

        public EmptyShader()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new EmptyShader()
            {
                Title = this.Title
            };
        }
    }
}
