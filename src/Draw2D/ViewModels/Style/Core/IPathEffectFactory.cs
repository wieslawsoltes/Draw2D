using System;
using CommunityToolkit.Mvvm.Input;

namespace Draw2D.ViewModels.Style;

public interface IPathEffectFactory
{
    IRelayCommand<Action<IPathEffect>> Create1DPathTranslateEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> Create1DPathRotateEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> Create1DPathMorphEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> Create2DLineHatchHorizontalLinesEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> Create2DLineHatchVerticalLinesEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> Create2DLineHatchDiagonalLinesEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> Create2DPathTileEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateCornerEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateDashEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateDotEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateDashDotEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateDashDotDotEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateDiscreteEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateTrimEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateComposeEffectCommand { get; }
    IRelayCommand<Action<IPathEffect>> CreateSumEffectCommand { get; }
    IRelayCommand<IPathEffect> CopyCommand { get; }
    IRelayCommand<Action<IPathEffect>> PasteCommand { get; }
    IRelayCommand<Action<IPathEffect>> DeleteCommand { get; }
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
