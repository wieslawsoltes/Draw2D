// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Style.PathEffects;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class ShapeStyle : ViewModelBase, ICopyable
    {
        public static StrokeCap[] StrokeCapValues { get; } = (StrokeCap[])Enum.GetValues(typeof(StrokeCap));
        public static StrokeJoin[] StrokeJoinValues { get; } = (StrokeJoin[])Enum.GetValues(typeof(StrokeJoin));

        private ArgbColor _stroke;
        private ArgbColor _fill;
        private bool _isStroked;
        private bool _isFilled;
        private bool _isAntialias;
        private bool _isScaled;
        private double _strokeWidth;
        private StrokeCap _strokeCap;
        private StrokeJoin _strokeJoin;
        private double _strokeMiter;
        private TextStyle _textStyle;
        private IList<IPathEffect> _pathEffects;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Fill
        {
            get => _fill;
            set => Update(ref _fill, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAntialias
        {
            get => _isAntialias;
            set => Update(ref _isAntialias, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsScaled
        {
            get => _isScaled;
            set => Update(ref _isScaled, value);
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
        public TextStyle TextStyle
        {
            get => _textStyle;
            set => Update(ref _textStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPathEffect> PathEffects
        {
            get => _pathEffects;
            set => Update(ref _pathEffects, value);
        }

        public ShapeStyle()
        {
        }

        public ShapeStyle(string title, ArgbColor stroke, ArgbColor fill, bool isStroked, bool isFilled, bool isScaled, double strokeWidth, TextStyle textStyle, IList<IPathEffect> pathEffects = null)
        {
            this.Title = title;
            this.Stroke = stroke;
            this.Fill = fill;
            this.IsStroked = isStroked;
            this.IsFilled = isFilled;
            this.IsAntialias = true;
            this.IsScaled = isScaled;
            this.StrokeWidth = strokeWidth;
            this.StrokeCap = StrokeCap.Butt;
            this.StrokeJoin = StrokeJoin.Miter;
            this.StrokeMiter = 4.0;
            this.TextStyle = textStyle;
            this.PathEffects = pathEffects != null ?
                new ObservableCollection<IPathEffect>(pathEffects) : 
                new ObservableCollection<IPathEffect>();
        }

        public void Add1DPathTranslateEffect()
        {
            _pathEffects.Add(Path1DPathEffect.MakeTranslate());
        }

        public void Add1DPathRotateEffect()
        {
            _pathEffects.Add(Path1DPathEffect.MakeRotate());
        }

        public void Add1DPathMorphEffect()
        {
            _pathEffects.Add(Path1DPathEffect.MakeMorph());
        }

        public void AddDashEffect()
        {
            _pathEffects.Add(PathDashEffect.MakeDash());
        }

        public void AddDotEffect()
        {
            _pathEffects.Add(PathDashEffect.MakeDot());
        }

        public void AddDashDotEffect()
        {
            _pathEffects.Add(PathDashEffect.MakeDashDot());
        }

        public void AddDashDotDotEffect()
        {
            _pathEffects.Add(PathDashEffect.MakeDashDotDot());
        }

        public void RemovePathEffect(IPathEffect pathEffect)
        {
            if (pathEffect != null)
            {
                _pathEffects.Remove(pathEffect);
            }
        }

        public override void Invalidate()
        {
            _stroke?.Invalidate();
            _fill?.Invalidate();
            _textStyle?.Invalidate();
            _textStyle.Typeface?.Invalidate();
            _textStyle.Stroke?.Invalidate();

            if (_pathEffects != null)
            {
                foreach (var pathEffect in _pathEffects)
                {
                    pathEffect.Invalidate();
                } 
            }

            base.Invalidate();
        }

        public object Copy(Dictionary<object, object> shared)
        {
            var copy = new ShapeStyle()
            {
                Name = this.Name,
                Title = this.Title + "_copy",
                Stroke = (ArgbColor)(this.Stroke.Copy(shared)),
                Fill = (ArgbColor)(this.Fill.Copy(shared)),
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
                IsAntialias = this.IsAntialias,
                IsScaled = this.IsScaled,
                StrokeWidth = this.StrokeWidth,
                StrokeCap = this.StrokeCap,
                StrokeJoin = this.StrokeJoin,
                StrokeMiter = this.StrokeMiter,
                TextStyle = (TextStyle)(this.TextStyle.Copy(shared)),
                PathEffects = new ObservableCollection<IPathEffect>()
            };

            foreach (var pathEffect in this.PathEffects)
            {
                var pathEffectCopy = (IPathEffect)pathEffect.Copy(shared);
                copy.PathEffects.Add(pathEffectCopy);
            }

            return copy;
        }
    }
}
