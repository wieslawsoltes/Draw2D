// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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
        private IPathEffect _pathEffect;

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
        public IPathEffect PathEffect
        {
            get => _pathEffect;
            set => Update(ref _pathEffect, value);
        }

        public FillPaint()
        {
        }

        public FillPaint(
            ArgbColor color,
            bool isAntialias = true,
            IPathEffect pathEffect = null)
        {
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.PathEffect = pathEffect;
        }

        public void SetPathEffect(IPathEffect pathEffect)
        {
            this.PathEffect = pathEffect;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new FillPaint()
            {
                Name = this.Name,
                Title = this.Title,
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                PathEffect = (IPathEffect)this.PathEffect.Copy(shared)
            };
        }
    }
}
