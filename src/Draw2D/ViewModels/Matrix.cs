using System.Collections.Generic;
using System.Runtime.Serialization;
using Spatial;

namespace Draw2D.ViewModels;

[DataContract(IsReference = true)]
public class Matrix : ViewModelBase, ICopyable
{
    private double _scaleX;
    private double _skewX;
    private double _transX;
    private double _skewY;
    private double _scaleY;
    private double _transY;
    private double _persp0;
    private double _persp1;
    private double _persp2;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double ScaleX
    {
        get => _scaleX;
        set => Update(ref _scaleX, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double SkewX
    {
        get => _skewX;
        set => Update(ref _skewX, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double TransX
    {
        get => _transX;
        set => Update(ref _transX, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double SkewY
    {
        get => _skewY;
        set => Update(ref _skewY, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double ScaleY
    {
        get => _scaleY;
        set => Update(ref _scaleY, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double TransY
    {
        get => _transY;
        set => Update(ref _transY, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double Persp0
    {
        get => _persp0;
        set => Update(ref _persp0, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double Persp1
    {
        get => _persp1;
        set => Update(ref _persp1, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double Persp2
    {
        get => _persp2;
        set => Update(ref _persp2, value);
    }

    public Matrix()
    {
    }

    public Matrix(
        double scaleX, double skewX, double transX,
        double skewY, double scaleY, double transY,
        double persp0, double persp1, double persp2)
    {
        this.ScaleX = scaleX;
        this.SkewX = skewX;
        this.TransX = transX;
        this.SkewY = skewY;
        this.ScaleY = scaleY;
        this.TransY = transY;
        this.Persp0 = persp0;
        this.Persp1 = persp1;
        this.Persp2 = persp2;
    }

    public static Matrix MakeIdentity()
    {
        return new Matrix()
        {
            ScaleX = 1,
            ScaleY = 1,
            Persp2 = 1
        };
    }

    public static Matrix MakeFrom(Matrix2 matrix)
    {
        return new Matrix()
        {
            ScaleX = matrix.M11,
            SkewX = matrix.M12,
            TransX = matrix.OffsetX,
            SkewY = matrix.M21,
            ScaleY = matrix.M22,
            TransY = matrix.OffsetY,
            Persp0 = 0,
            Persp1 = 0,
            Persp2 = 1
        };
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new Matrix()
        {
            ScaleX = this.ScaleX,
            SkewX = this.SkewX,
            TransX = this.TransX,
            SkewY = this.SkewY,
            ScaleY = this.ScaleY,
            TransY = this.TransY,
            Persp0 = this.Persp0,
            Persp1 = this.Persp1,
            Persp2 = this.Persp2
        };
    }
}