// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class StrokePaint : ViewModelBase, IStrokePaint
    {
        public static IPathEffectFactory PathEffectFactory { get; } = Style.PathEffectFactory.Instance;
        public static StrokeCap[] StrokeCapValues { get; } = (StrokeCap[])Enum.GetValues(typeof(StrokeCap));
        public static StrokeJoin[] StrokeJoinValues { get; } = (StrokeJoin[])Enum.GetValues(typeof(StrokeJoin));

        private ArgbColor _color;
        private bool _isAntialias;
        private double _strokeWidth;
        private StrokeCap _strokeCap;
        private StrokeJoin _strokeJoin;
        private double _strokeMiter;
        private IPathEffect _pathEffect;
        private IShader _shader;
        private bool _isScaled;

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
        public double StrokeWidth
        {
            get => _strokeWidth;
            set => Update(ref _strokeWidth, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public StrokeCap StrokeCap
        {
            get => _strokeCap;
            set => Update(ref _strokeCap, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public StrokeJoin StrokeJoin
        {
            get => _strokeJoin;
            set => Update(ref _strokeJoin, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double StrokeMiter
        {
            get => _strokeMiter;
            set => Update(ref _strokeMiter, value);
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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsScaled
        {
            get => _isScaled;
            set => Update(ref _isScaled, value);
        }

        public StrokePaint()
        {
        }

        public StrokePaint(
            ArgbColor color,
            double strokeWidth = 2.0,
            StrokeCap strokeCap = StrokeCap.Butt,
            StrokeJoin strokeJoin = StrokeJoin.Miter,
            double strokeMiter = 4.0,
            bool isScaled = false,
            bool isAntialias = true,
            IPathEffect pathEffect = null,
            IShader shader = null)
        {
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.StrokeWidth = strokeWidth;
            this.StrokeCap = strokeCap;
            this.StrokeJoin = strokeJoin;
            this.StrokeMiter = strokeMiter;
            this.PathEffect = pathEffect;
            this.Shader = shader;
            this.IsScaled = isScaled;
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
            return new StrokePaint()
            {
                Name = this.Name,
                Title = this.Title,
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                StrokeWidth = this.StrokeWidth,
                StrokeCap = this.StrokeCap,
                StrokeJoin = this.StrokeJoin,
                StrokeMiter = this.StrokeMiter,
                PathEffect = (IPathEffect)this.PathEffect.Copy(shared),
                Shader = (IShader)this.Shader.Copy(shared),
                IsScaled = this.IsScaled
            };
        }
    }
}
