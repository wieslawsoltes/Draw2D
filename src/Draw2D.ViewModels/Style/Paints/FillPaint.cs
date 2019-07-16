// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class FillPaint : ViewModelBase, IFillPaint
    {
        public static IPathEffectFactory PathEffectFactory { get; } = Style.PathEffectFactory.Instance;

        private ArgbColor _color;
        private bool _isAntialias;
        private IColorFilter _colorFilter;
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
        public IColorFilter ColorFilter
        {
            get => _colorFilter;
            set => Update(ref _colorFilter, value);
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

        public FillPaint()
        {
        }

        public FillPaint(
            ArgbColor color,
            bool isAntialias = true,
            IColorFilter colorFilter = null,
            IPathEffect pathEffect = null,
            IShader shader = null)
        {
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.ColorFilter = colorFilter;
            this.PathEffect = pathEffect;
            this.Shader = shader;
        }

        public void SetColorFilter(IColorFilter colorFilter)
        {
            this.ColorFilter = colorFilter;
        }

        public void SetPathEffect(IPathEffect pathEffect)
        {
            this.PathEffect = pathEffect;
        }

        public void SetShader(IShader shader)
        {
            this.Shader = shader;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new FillPaint()
            {
                Name = this.Name,
                Title = this.Title,
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                ColorFilter = (IColorFilter)this.ColorFilter.Copy(shared),
                PathEffect = (IPathEffect)this.PathEffect.Copy(shared),
                Shader = (IShader)this.Shader.Copy(shared)
            };
        }
    }
}
