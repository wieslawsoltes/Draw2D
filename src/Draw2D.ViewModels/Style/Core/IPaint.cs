// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Style
{
    public interface IPaint : INode, IDirty, ICopyable
    {
        ArgbColor Color { get; set; }
        bool IsAntialias { get; set; }
        IPathEffect PathEffect { get; set; }
        IShader Shader { get; set; }
        void SetPathEffect(IPathEffect pathEffect);
        void SetShader(IShader shader);
    }
}
