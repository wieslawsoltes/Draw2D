using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels;

[DataContract(IsReference = true)]
public class Text : ViewModelBase, ICopyable
{
    private string _value;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Value
    {
        get => _value;
        set => Update(ref _value, value);
    }

    public Text()
    {
    }

    public Text(string value)
    {
        _value = value;
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new Text()
        {
            Value = this.Value
        };
    }
}