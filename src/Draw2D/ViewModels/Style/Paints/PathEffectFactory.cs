﻿using System;
using Draw2D.ViewModels.Style.PathEffects;

namespace Draw2D.ViewModels.Style;

public class PathEffectFactory : IPathEffectFactory
{
    public static IPathEffectFactory Instance { get; } = new PathEffectFactory();

    private IPathEffect _copy = null;

    public void Create1DPathTranslateEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path1DPathEffect.MakeTranslate());
    }

    public void Create1DPathRotateEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path1DPathEffect.MakeRotate());
    }

    public void Create1DPathMorphEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path1DPathEffect.MakeMorph());
    }

    public void Create2DLineHatchHorizontalLinesEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DLineEffect.MakeHatchHorizontalLines());
    }

    public void Create2DLineHatchVerticalLinesEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DLineEffect.MakeHatchVerticalLines());
    }

    public void Create2DLineHatchDiagonalLinesEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DLineEffect.MakeHatchDiagonalLines());
    }

    public void Create2DPathTileEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(Path2DPathEffect.MakeTile());
    }

    public void CreateCornerEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathCornerEffect.MakeCorner());
    }

    public void CreateDashEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDash());
    }

    public void CreateDotEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDot());
    }

    public void CreateDashDotEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDashDot());
    }

    public void CreateDashDotDotEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDashEffect.MakeDashDotDot());
    }

    public void CreateDiscreteEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathDiscreteEffect.MakeDiscrete());
    }

    public void CreateTrimEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathTrimEffect.MakeTrim());
    }

    public void CreateComposeEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathComposeEffect.MakeCompose());
    }

    public void CreateSumEffect(Action<IPathEffect> setter)
    {
        setter?.Invoke(PathSumEffect.MakeSum());
    }

    public void Copy(IPathEffect pathEffect)
    {
        if (pathEffect != null)
        {
            _copy = pathEffect;
        }
    }

    public void Paste(Action<IPathEffect> setter)
    {
        if (_copy != null)
        {
            setter?.Invoke((IPathEffect)_copy.Copy(null));
        }
    }

    public void Delete(Action<IPathEffect> setter)
    {
        setter?.Invoke(null);
    }
}