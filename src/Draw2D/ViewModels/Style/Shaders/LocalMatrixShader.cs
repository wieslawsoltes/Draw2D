using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders;

[DataContract(IsReference = true)]
public class LocalMatrixShader : ViewModelBase, IShader
{
    // TODO:

    public LocalMatrixShader()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new LocalMatrixShader()
        {
            Title = this.Title
        };
    }
}