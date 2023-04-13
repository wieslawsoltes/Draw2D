using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style;

[DataContract(IsReference = true)]
public class ShapeStyle : ViewModelBase, IShapeStyle
{
    private bool _isStroked;
    private bool _isFilled;
    private bool _isText;
    private IPaint _strokePaint;
    private IPaint _fillPaint;
    private IPaint _textPaint;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsStroked
    {
        get => _isStroked;
        set => Update(ref _isStroked, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsFilled
    {
        get => _isFilled;
        set => Update(ref _isFilled, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsText
    {
        get => _isText;
        set => Update(ref _isText, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IPaint StrokePaint
    {
        get => _strokePaint;
        set => Update(ref _strokePaint, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IPaint FillPaint
    {
        get => _fillPaint;
        set => Update(ref _fillPaint, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IPaint TextPaint
    {
        get => _textPaint;
        set => Update(ref _textPaint, value);
    }

    public ShapeStyle()
    {
    }

    public ShapeStyle(
        string title,
        IPaint strokePaint = null,
        IPaint fillPaint = null,
        IPaint textPaint = null,
        bool isStroked = true,
        bool isFilled = false,
        bool isText = true)
    {
        this.Title = title;
        this.IsStroked = isStroked;
        this.IsFilled = isFilled;
        this.IsText = isText;
        this.StrokePaint = strokePaint;
        this.FillPaint = fillPaint;
        this.TextPaint = textPaint;
    }

    public override bool IsTreeDirty()
    {
        if (base.IsTreeDirty())
        {
            return true;
        }

        if (_strokePaint?.IsTreeDirty() ?? false)
        {
            return true;
        }

        if (_fillPaint?.IsTreeDirty() ?? false)
        {
            return true;
        }

        if (_textPaint?.IsTreeDirty() ?? false)
        {
            return true;
        }

        return false;
    }

    public override void Invalidate()
    {
        _strokePaint?.Invalidate();

        _fillPaint?.Invalidate();

        _textPaint?.Invalidate();

        base.Invalidate();
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new ShapeStyle()
        {
            Name = this.Name,
            Title = this.Title + "_copy",
            IsStroked = this.IsStroked,
            IsFilled = this.IsFilled,
            IsText = this.IsText,
            StrokePaint = (IPaint)this.StrokePaint.Copy(shared),
            FillPaint = (IPaint)this.FillPaint.Copy(shared),
            TextPaint = (IPaint)this.TextPaint.Copy(shared)
        };
    }
}