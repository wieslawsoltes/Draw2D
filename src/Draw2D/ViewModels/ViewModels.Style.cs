// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define DEBUG_CONNECTORS
//#define USE_POINT_DECORATOR
//#define USE_GROUP_SHAPES
//#define USE_PATH_FIGURES
//#define USE_CONTAINER_POINTS
//#define USE_CONTAINER_SHAPES
#define USE_SERIALIZE_STYLES
#define USE_SERIALIZE_GROUPS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

namespace Draw2D.ViewModels.Style
{
    public enum HAlign
    {
        Left,
        Center,
        Right
    }

    public enum VAlign
    {
        Top,
        Center,
        Bottom
    }

    [DataContract(IsReference = true)]
    public class ArgbColor : ViewModelBase, ICopyable
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte A
        {
            get => _a;
            set => Update(ref _a, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte R
        {
            get => _r;
            set => Update(ref _r, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte G
        {
            get => _g;
            set => Update(ref _g, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte B
        {
            get => _b;
            set => Update(ref _b, value);
        }

        public ArgbColor()
        {
        }

        public ArgbColor(byte a, byte r, byte g, byte b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ArgbColor()
            {
                A = this.A,
                R = this.R,
                G = this.G,
                B = this.B
            };
        }
    }

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

    [DataContract(IsReference = true)]
    public class ShapeStyle : ViewModelBase, ICopyable
    {
        private string _title;
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private bool _isStroked;
        private bool _isFilled;
        private TextStyle _textStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

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
        public double Thickness
        {
            get => _thickness;
            set => Update(ref _thickness, value);
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
        public TextStyle TextStyle
        {
            get => _textStyle;
            set => Update(ref _textStyle, value);
        }

        public ShapeStyle()
        {
        }

        public ShapeStyle(string title, ArgbColor stroke, ArgbColor fill, double thickness, bool isStroked, bool isFilled, TextStyle textStyle)
        {
            this.Title = title;
            this.Stroke = stroke;
            this.Fill = fill;
            this.Thickness = thickness;
            this.IsStroked = isStroked;
            this.IsFilled = isFilled;
            this.TextStyle = textStyle;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Title = this.Title + "_copy",
                Stroke = (ArgbColor)(this.Stroke.Copy(shared)),
                Fill = (ArgbColor)(this.Fill.Copy(shared)),
                Thickness = this.Thickness,
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
                TextStyle = (TextStyle)(this.TextStyle.Copy(shared))
            };
        }
    }
}
