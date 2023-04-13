using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public class RectangleToolSettings : SettingsBase
{
    private bool _connectPoints;
    private double _hitTestRadius;
    private double _radiusX;
    private double _radiusY;

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

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double RadiusX
    {
        get => _radiusX;
        set => Update(ref _radiusX, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double RadiusY
    {
        get => _radiusY;
        set => Update(ref _radiusY, value);
    }
}