using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders;

[DataContract(IsReference = true)]
public class ColorShader : ViewModelBase, IShader
{
    private ArgbColor _color;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ArgbColor Color
    {
        get => _color;
        set => Update(ref _color, value);
    }

    public ColorShader()
    {
    }

    public ColorShader(ArgbColor color)
    {
        this.Color = color;
    }

    public ColorShader(byte a, byte r, byte g, byte b)
    {
        this.Color = new ArgbColor(a, r, b, b);
    }

    public static IShader MakeColor()
    {
        return new ColorShader(new ArgbColor(255, 0, 0, 0));
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ColorShader()
        {
            Title = this.Title,
            Color = (ArgbColor)this.Color.Copy(shared)
        };
    }
}