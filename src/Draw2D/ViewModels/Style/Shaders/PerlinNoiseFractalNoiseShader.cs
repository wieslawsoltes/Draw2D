using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders;

[DataContract(IsReference = true)]
public class PerlinNoiseFractalNoiseShader : ViewModelBase, IShader
{
    // TODO:

    public PerlinNoiseFractalNoiseShader()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new PerlinNoiseFractalNoiseShader()
        {
            Title = this.Title
        };
    }
}