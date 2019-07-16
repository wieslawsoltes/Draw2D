// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class TextPaint : BasePaint, ITextPaint
    {
        public static HAlign[] HAlignValues { get; } = (HAlign[])Enum.GetValues(typeof(HAlign));
        public static VAlign[] VAlignValues { get; } = (VAlign[])Enum.GetValues(typeof(VAlign));

        private Typeface _typeface;
        private double _fontSize;
        private bool _lcdRenderText;
        private bool _subpixelText;
        private HAlign _hAlign;
        private VAlign _vAlign;

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

        public TextPaint()
        {
        }

        public TextPaint(
            ArgbColor color,
            string fontFamily = "Calibri",
            double fontSize = 12.0,
            HAlign hAlign = HAlign.Center,
            VAlign vAlign = VAlign.Center,
            bool lcdRenderText = true,
            bool subpixelText = true,
            bool isAntialias = true,
            IPaintEffects effects = null)
        {
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.Typeface = new Typeface(fontFamily);
            this.FontSize = fontSize;
            this.LcdRenderText = lcdRenderText;
            this.SubpixelText = subpixelText;
            this.Effects = effects;
            this.HAlign = hAlign;
            this.VAlign = vAlign;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new TextPaint()
            {
                Name = this.Name,
                Title = this.Title,
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                Typeface = (Typeface)(this.Typeface.Copy(shared)),
                FontSize = this.FontSize,
                LcdRenderText = this.LcdRenderText,
                SubpixelText = this.SubpixelText,
                Effects = (IPaintEffects)this.Effects.Copy(shared),
                HAlign = this.HAlign,
                VAlign = this.VAlign,
            };
        }
    }
}
