using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders;

[DataContract(IsReference = true)]
public class TwoPointConicalGradientShader : ViewModelBase, IShader
{
    // TODO:

    public TwoPointConicalGradientShader()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new TwoPointConicalGradientShader()
        {
            Title = this.Title
        };
    }
}