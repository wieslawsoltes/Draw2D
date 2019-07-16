// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Style
{
    public interface IPaintEffects : INode, IDirty, ICopyable
    {
        BlendMode BlendMode { get; set; }
        IColorFilter ColorFilter { get; set; }
        IMaskFilter MaskFilter { get; set; }
        IPathEffect PathEffect { get; set; }
        IShader Shader { get; set; }
        void SetColorFilter(IColorFilter colorFilter);
        void SetMaskFilter(IMaskFilter maskFilter);
        void SetPathEffect(IPathEffect pathEffect);
        void SetShader(IShader shader);
    }
}
