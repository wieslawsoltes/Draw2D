// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class TextStyle : ViewModelBase, ICopyable
    {
        private string _fontFamily;
        private double _fontSize;
        private HAlign _hAlign;
        private VAlign _vAlign;
        private ArgbColor _stroke;
        private bool _isStroked;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string FontFamily
        {
            get => _fontFamily;
            set => Update(ref _fontFamily, value);
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

        public TextStyle()
        {
        }

        public TextStyle(string fontFamily, double fontSize, HAlign hAlign, VAlign vAlign, ArgbColor stroke, bool isStroked)
        {
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.HAlign = hAlign;
            this.VAlign = vAlign;
            this.Stroke = stroke;
            this.IsStroked = isStroked;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new TextStyle()
            {
                Name = this.Name,
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                HAlign = this.HAlign,
                VAlign = this.VAlign,
                Stroke = (ArgbColor)(this.Stroke.Copy(shared)),
                IsStroked = this.IsStroked
            };
        }
    }
}
