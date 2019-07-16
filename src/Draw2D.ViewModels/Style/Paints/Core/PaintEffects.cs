// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class PaintEffects : ViewModelBase, IPaintEffects
    {
        public static BlendMode[] BlendModeValues { get; } = (BlendMode[])Enum.GetValues(typeof(BlendMode));

        private BlendMode _blendMode;
        private IColorFilter _colorFilter;
        private IMaskFilter _maskFilter;
        private IPathEffect _pathEffect;
        private IShader _shader;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BlendMode BlendMode
        {
            get => _blendMode;
            set => Update(ref _blendMode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IColorFilter ColorFilter
        {
            get => _colorFilter;
            set => Update(ref _colorFilter, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IMaskFilter MaskFilter
        {
            get => _maskFilter;
            set => Update(ref _maskFilter, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPathEffect PathEffect
        {
            get => _pathEffect;
            set => Update(ref _pathEffect, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IShader Shader
        {
            get => _shader;
            set => Update(ref _shader, value);
        }

        public void SetColorFilter(IColorFilter colorFilter)
        {
            this.ColorFilter = colorFilter;
        }

        public void SetMaskFilter(IMaskFilter maskFilter)
        {
            this.MaskFilter = maskFilter;
        }

        public void SetPathEffect(IPathEffect pathEffect)
        {
            this.PathEffect = pathEffect;
        }

        public void SetShader(IShader shader)
        {
            this.Shader = shader;
        }

        public PaintEffects()
        {
        }

        public PaintEffects(
            BlendMode blendMode = BlendMode.Clear,
            IColorFilter colorFilter = null,
            IMaskFilter maskFilter = null,
            IPathEffect pathEffect = null,
            IShader shader = null)
        {
            this.BlendMode = blendMode;
            this.ColorFilter = colorFilter;
            this.MaskFilter = maskFilter;
            this.PathEffect = pathEffect;
            this.Shader = shader;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PaintEffects()
            {
                Name = this.Name,
                Title = this.Title,
                BlendMode = this.BlendMode,
                ColorFilter = (IColorFilter)this.ColorFilter.Copy(shared),
                MaskFilter = (IMaskFilter)this.MaskFilter.Copy(shared),
                PathEffect = (IPathEffect)this.PathEffect.Copy(shared),
                Shader = (IShader)this.Shader.Copy(shared)
            };
        }
    }
}
