// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class TextStyle : ViewModelBase, ICopyable
    {
        public static HAlign[] HAlignValues { get; } = (HAlign[])Enum.GetValues(typeof(HAlign));
        public static VAlign[] VAlignValues { get; } = (VAlign[])Enum.GetValues(typeof(VAlign));

        private Typeface _typeface;
        private double _fontSize;
        private HAlign _hAlign;
        private VAlign _vAlign;
        private ArgbColor _stroke;
        private bool _isStroked;
        private bool _isAntialias;
        private bool _lcdRenderText;
        private bool _subpixelText;

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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAntialias
        {
            get => _isAntialias;
            set => Update(ref _isAntialias, value);
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

        public TextStyle()
        {
        }

        public TextStyle(string fontFamily, double fontSize, HAlign hAlign, VAlign vAlign, ArgbColor stroke, bool isStroked)
        {
            this.Typeface = new Typeface(fontFamily);
            this.FontSize = fontSize;
            this.HAlign = hAlign;
            this.VAlign = vAlign;
            this.Stroke = stroke;
            this.IsStroked = isStroked;
            this.IsAntialias = true;
            this.LcdRenderText = true;
            this.SubpixelText = true;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new TextStyle()
            {
                Name = this.Name,
                Typeface = (Typeface)(this.Typeface.Copy(shared)),
                FontSize = this.FontSize,
                HAlign = this.HAlign,
                VAlign = this.VAlign,
                Stroke = (ArgbColor)(this.Stroke.Copy(shared)),
                IsStroked = this.IsStroked,
                IsAntialias = this.IsAntialias,
                LcdRenderText = this.LcdRenderText,
                SubpixelText = this.SubpixelText
            };
        }
    }
}
