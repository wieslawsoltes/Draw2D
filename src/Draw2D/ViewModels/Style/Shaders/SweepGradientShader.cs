using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders;

[DataContract(IsReference = true)]
public class SweepGradientShader : ViewModelBase, IShader
{
    // TODO:

    public SweepGradientShader()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new SweepGradientShader()
        {
            Title = this.Title
        };
    }
}