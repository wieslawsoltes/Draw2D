using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders;

[DataContract(IsReference = true)]
public class PerlinNoiseTurbulenceShader : ViewModelBase, IShader
{
    // TODO:

    public PerlinNoiseTurbulenceShader()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new PerlinNoiseTurbulenceShader()
        {
            Title = this.Title
        };
    }
}