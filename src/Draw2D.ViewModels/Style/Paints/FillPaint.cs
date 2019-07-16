// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class FillPaint : BasePaint, IFillPaint
    {
        public static IPathEffectFactory PathEffectFactory { get; } = Style.PathEffectFactory.Instance;

        public FillPaint()
        {
        }

        public FillPaint(
            ArgbColor color,
            bool isAntialias = true,
            BlendMode blendMode = BlendMode.Clear,
            IColorFilter colorFilter = null,
            IMaskFilter maskFilter = null,
            IPathEffect pathEffect = null,
            IShader shader = null)
        {
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.BlendMode = blendMode;
            this.ColorFilter = colorFilter;
            this.MaskFilter = maskFilter;
            this.PathEffect = pathEffect;
            this.Shader = shader;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new FillPaint()
            {
                Name = this.Name,
                Title = this.Title,
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                BlendMode = this.BlendMode,
                ColorFilter = (IColorFilter)this.ColorFilter.Copy(shared),
                MaskFilter = (IMaskFilter)this.MaskFilter.Copy(shared),
                PathEffect = (IPathEffect)this.PathEffect.Copy(shared),
                Shader = (IShader)this.Shader.Copy(shared)
            };
        }
    }
}
