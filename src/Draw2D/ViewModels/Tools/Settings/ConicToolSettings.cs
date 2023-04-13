﻿using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public class ConicToolSettings : SettingsBase
{
    private bool _connectPoints;
    private double _hitTestRadius;
    private double _weight;

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
    public double Weight
    {
        get => _weight;
        set => Update(ref _weight, value);
    }
}