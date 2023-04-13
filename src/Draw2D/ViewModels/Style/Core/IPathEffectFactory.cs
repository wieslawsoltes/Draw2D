using System;

namespace Draw2D.ViewModels.Style;

public interface IPathEffectFactory
{
    void Create1DPathTranslateEffect(Action<IPathEffect> setter);
    void Create1DPathRotateEffect(Action<IPathEffect> setter);
    void Create1DPathMorphEffect(Action<IPathEffect> setter);
    void Create2DLineHatchHorizontalLinesEffect(Action<IPathEffect> setter);
    void Create2DLineHatchVerticalLinesEffect(Action<IPathEffect> setter);
    void Create2DLineHatchDiagonalLinesEffect(Action<IPathEffect> setter);
    void Create2DPathTileEffect(Action<IPathEffect> setter);
    void CreateCornerEffect(Action<IPathEffect> setter);
    void CreateDashEffect(Action<IPathEffect> setter);
    void CreateDotEffect(Action<IPathEffect> setter);
    void CreateDashDotEffect(Action<IPathEffect> setter);
    void CreateDashDotDotEffect(Action<IPathEffect> setter);
    void CreateDiscreteEffect(Action<IPathEffect> setter);
    void CreateTrimEffect(Action<IPathEffect> setter);
    void CreateComposeEffect(Action<IPathEffect> setter);
    void CreateSumEffect(Action<IPathEffect> setter);
    void Copy(IPathEffect pathEffect);
    void Paste(Action<IPathEffect> setter);
    void Delete(Action<IPathEffect> setter);
}