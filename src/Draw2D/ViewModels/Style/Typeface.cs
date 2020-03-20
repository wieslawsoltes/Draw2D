using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class Typeface : ViewModelBase, ICopyable
    {
        public static FontStyleSlant[] FontStyleSlantValues { get; } = (FontStyleSlant[])Enum.GetValues(typeof(FontStyleSlant));

        private string _fontFamily;
        private int _fontWeight;
        private int _fontWidth;
        private FontStyleSlant _fontSlant;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string FontFamily
        {
            get => _fontFamily;
            set => Update(ref _fontFamily, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int FontWeight
        {
            get => _fontWeight;
            set => Update(ref _fontWeight, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int FontWidth
        {
            get => _fontWidth;
            set => Update(ref _fontWidth, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FontStyleSlant FontSlant
        {
            get => _fontSlant;
            set => Update(ref _fontSlant, value);
        }

        public Typeface()
        {
        }

        public Typeface(string fontFamily)
        {
            this.FontFamily = fontFamily;
            this.FontWeight = 400;
            this.FontWidth = 5;
            this.FontSlant = FontStyleSlant.Upright;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Typeface()
            {
                Name = this.Name,
                FontFamily = this.FontFamily,
                FontWeight = this.FontWeight,
                FontWidth = this.FontWidth,
                FontSlant = this.FontSlant,
            };
        }
    }
}
