using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class Paint : ViewModelBase, IPaint
    {
        public static PaintStyle[] PaintStyleValues { get; } = (PaintStyle[])Enum.GetValues(typeof(PaintStyle));
        public static StrokeCap[] StrokeCapValues { get; } = (StrokeCap[])Enum.GetValues(typeof(StrokeCap));
        public static StrokeJoin[] StrokeJoinValues { get; } = (StrokeJoin[])Enum.GetValues(typeof(StrokeJoin));
        public static HAlign[] HAlignValues { get; } = (HAlign[])Enum.GetValues(typeof(HAlign));
        public static VAlign[] VAlignValues { get; } = (VAlign[])Enum.GetValues(typeof(VAlign));

        private PaintStyle _paintStyle;
        private ArgbColor _color;
        private bool _isAntialias;
        private IPaintEffects _effects;
        private double _strokeWidth;
        private StrokeCap _strokeCap;
        private StrokeJoin _strokeJoin;
        private double _strokeMiter;
        private bool _isScaled;
        private Typeface _typeface;
        private double _fontSize;
        private bool _lcdRenderText;
        private bool _subpixelText;
        private HAlign _hAlign;
        private VAlign _vAlign;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PaintStyle PaintStyle
        {
            get => _paintStyle;
            set => Update(ref _paintStyle, value);
        }

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
        public IPaintEffects Effects
        {
            get => _effects;
            set => Update(ref _effects, value);
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
        public bool IsScaled
        {
            get => _isScaled;
            set => Update(ref _isScaled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Typeface Typeface
        {
            get => _typeface;
            set => Update(ref _typeface, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double FontSize
        {
            get => _fontSize;
            set => Update(ref _fontSize, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool LcdRenderText
        {
            get => _lcdRenderText;
            set => Update(ref _lcdRenderText, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool SubpixelText
        {
            get => _subpixelText;
            set => Update(ref _subpixelText, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HAlign HAlign
        {
            get => _hAlign;
            set => Update(ref _hAlign, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public VAlign VAlign
        {
            get => _vAlign;
            set => Update(ref _vAlign, value);
        }

        public Paint()
        {
        }

        public Paint(
            PaintStyle paintStyle,
            ArgbColor color,
            bool isAntialias = true,
            IPaintEffects effects = null,
            double strokeWidth = 2.0,
            StrokeCap strokeCap = StrokeCap.Butt,
            StrokeJoin strokeJoin = StrokeJoin.Miter,
            double strokeMiter = 4.0,
            bool isScaled = false,
            string fontFamily = "System", // Calibri
            double fontSize = 12.0,
            HAlign hAlign = HAlign.Center,
            VAlign vAlign = VAlign.Center,
            bool lcdRenderText = true,
            bool subpixelText = true)
        {
            this.PaintStyle = paintStyle;
            // Common
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.Effects = effects;
            // Stroke
            this.StrokeWidth = strokeWidth;
            this.StrokeCap = strokeCap;
            this.StrokeJoin = strokeJoin;
            this.StrokeMiter = strokeMiter;
            this.IsScaled = isScaled;
            // Text
            this.Typeface = new Typeface(fontFamily);
            this.FontSize = fontSize;
            this.LcdRenderText = lcdRenderText;
            this.SubpixelText = subpixelText;
            this.HAlign = hAlign;
            this.VAlign = vAlign;
        }

        public override bool IsTreeDirty()
        {
            if (base.IsTreeDirty())
            {
                return true;
            }

            if (_color?.IsTreeDirty() ?? false)
            {
                return true;
            }

            if (_typeface?.IsTreeDirty() ?? false)
            {
                return true;
            }

            if (_effects?.IsTreeDirty() ?? false)
            {
                return true;
            }

            return false;
        }

        public override void Invalidate()
        {
            _color?.Invalidate();

            _typeface?.Invalidate();

            _effects?.Invalidate();

            base.Invalidate();
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Paint()
            {
                Name = this.Name,
                Title = this.Title,
                PaintStyle = this.PaintStyle,
                // Common
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                Effects = (IPaintEffects)this.Effects.Copy(shared),
                // Stroke
                StrokeWidth = this.StrokeWidth,
                StrokeCap = this.StrokeCap,
                StrokeJoin = this.StrokeJoin,
                StrokeMiter = this.StrokeMiter,
                IsScaled = this.IsScaled,
                // Text
                Typeface = (Typeface)(this.Typeface.Copy(shared)),
                FontSize = this.FontSize,
                LcdRenderText = this.LcdRenderText,
                SubpixelText = this.SubpixelText,
                HAlign = this.HAlign,
                VAlign = this.VAlign,
            };
        }
    }
}
