using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public class TextToolSettings : SettingsBase
{
    private bool _connectPoints;
    private double _hitTestRadius;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool ConnectPoints
    {
        get => _connectPoints;
        set => Update(ref _connectPoints, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double HitTestRadius
    {
        get => _hitTestRadius;
        set => Update(ref _hitTestRadius, value);
    }
}