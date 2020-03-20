
namespace Draw2D.ViewModels.Style
{
    public interface IPaintEffects : INode, IDirty, ICopyable
    {
        BlendMode BlendMode { get; set; }
        IColorFilter ColorFilter { get; set; }
        IImageFilter ImageFilter { get; set; }
        IMaskFilter MaskFilter { get; set; }
        IPathEffect PathEffect { get; set; }
        IShader Shader { get; set; }
        void SetColorFilter(IColorFilter colorFilter);
        void SetImageFilter(IImageFilter imageFilter);
        void SetMaskFilter(IMaskFilter maskFilter);
        void SetPathEffect(IPathEffect pathEffect);
        void SetShader(IShader shader);
    }
}
