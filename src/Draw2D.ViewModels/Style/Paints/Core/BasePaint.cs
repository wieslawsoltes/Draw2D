// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public abstract class BasePaint : ViewModelBase, IPaint
    {
        private ArgbColor _color;
        private bool _isAntialias;
        private BlendMode _blendMode;
        private IColorFilter _colorFilter;
        private IMaskFilter _maskFilter;
        private IPathEffect _pathEffect;
        private IShader _shader;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Color
        {
            get => _color;
            set => Update(ref _color, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAntialias
        {
            get => _isAntialias;
            set => Update(ref _isAntialias, value);
        }

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

        public abstract object Copy(Dictionary<object, object> shared);
    }
}
