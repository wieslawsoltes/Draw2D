﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style;

[DataContract(IsReference = true)]
public class ArgbColor : ViewModelBase, ICopyable
{
    private byte _a;
    private byte _r;
    private byte _g;
    private byte _b;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte A
    {
        get => _a;
        set => Update(ref _a, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte R
    {
        get => _r;
        set => Update(ref _r, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte G
    {
        get => _g;
        set => Update(ref _g, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte B
    {
        get => _b;
        set => Update(ref _b, value);
    }

    public ArgbColor()
    {
    }

    public ArgbColor(byte a, byte r, byte g, byte b)
    {
        this.A = a;
        this.R = r;
        this.G = g;
        this.B = b;
    }

    public uint ToUint32()
    {
        return ((uint)_a << 24) | ((uint)_r << 16) | ((uint)_g << 8) | (uint)_b;
    }

    public string ToHex()
    {
        return $"#{this.ToUint32():X8}";
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ArgbColor()
        {
            A = this.A,
            R = this.R,
            G = this.G,
            B = this.B
        };
    }
}