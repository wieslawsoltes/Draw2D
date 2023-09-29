using System;
using CommunityToolkit.Mvvm.Input;
using Draw2D.ViewModels.Style.PathEffects;

namespace Draw2D.ViewModels.Style;

public partial class PathEffectFactory : IPathEffectFactory
{
    public static IPathEffectFactory Instance { get; } = new PathEffectFactory();

    private IPathEffect _copy = null;

    [RelayCommand]
    public void Create1DPathTranslateEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path1DPathEffect.MakeTranslate());
    }

    [RelayCommand]
    public void Create1DPathRotateEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path1DPathEffect.MakeRotate());
    }

    [RelayCommand]
    public void Create1DPathMorphEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path1DPathEffect.MakeMorph());
    }

    [RelayCommand]
    public void Create2DLineHatchHorizontalLinesEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DLineEffect.MakeHatchHorizontalLines());
    }

    [RelayCommand]
    public void Create2DLineHatchVerticalLinesEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DLineEffect.MakeHatchVerticalLines());
    }

    [RelayCommand]
    public void Create2DLineHatchDiagonalLinesEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DLineEffect.MakeHatchDiagonalLines());
    }

    [RelayCommand]
    public void Create2DPathTileEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DPathEffect.MakeTile());
    }

    [RelayCommand]
    public void CreateCornerEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathCornerEffect.MakeCorner());
    }

    [RelayCommand]
    public void CreateDashEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDash());
    }

    [RelayCommand]
    public void CreateDotEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDot());
    }

    [RelayCommand]
    public void CreateDashDotEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDashDot());
    }

    [RelayCommand]
    public void CreateDashDotDotEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDashDotDot());
    }

    [RelayCommand]
    public void CreateDiscreteEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDiscreteEffect.MakeDiscrete());
    }

    [RelayCommand]
    public void CreateTrimEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathTrimEffect.MakeTrim());
    }

    [RelayCommand]
    public void CreateComposeEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathComposeEffect.MakeCompose());
    }

    [RelayCommand]
    public void CreateSumEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathSumEffect.MakeSum());
    }

    [RelayCommand]
    public void Copy(IPathEffect pathEffect)
    {
        if (pathEffect != null)
        {
            _copy = pathEffect;
        }
    }

    [RelayCommand]
    public void Paste(Action<IPathEffect> setter)
    {
        if (_copy != null)
        {
            setter?.Invoke((IPathEffect)_copy.Copy(null));
        }
    }

    [RelayCommand]
    public void Delete(Action<IPathEffect> setter)
    {
        setter?.Invoke(null);
    }
}
