// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class Path1DPathEffect : ViewModelBase, IPathEffect
    {
        public static Path1DPathEffectStyle[] Path1DPathEffectStyleValues { get; } = (Path1DPathEffectStyle[])Enum.GetValues(typeof(Path1DPathEffectStyle));

        private string _path;
        private double _advance;
        private double _phase;
        private Path1DPathEffectStyle _style;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Path
        {
            get => _path;
            set => Update(ref _path, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Advance
        {
            get => _advance;
            set => Update(ref _advance, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Phase
        {
            get => _phase;
            set => Update(ref _phase, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Path1DPathEffectStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        public Path1DPathEffect()
        {
        }

        public Path1DPathEffect(string path, double advance, double phase, Path1DPathEffectStyle style)
        {
            this.Path = path;
            this.Advance = advance;
            this.Phase = phase;
            this.Style = style;
        }

        public static IPathEffect MakeTranslate()
        {
            return new Path1DPathEffect("M -10 -10 L 10 -10, 10 10, -10 10 Z", 24, 0, Path1DPathEffectStyle.Translate) { Title = "1DPathTranslate" };
        }

        public static IPathEffect MakeRotate()
        {
            return new Path1DPathEffect("M -10 0 L 0 -10, 10 0, 0 10 Z", 20, 0, Path1DPathEffectStyle.Rotate) { Title = "1DPathRotate" };
        }

        public static IPathEffect MakeMorph()
        {
            return new Path1DPathEffect("M -25 -10 L 25 -10, 25 10, -25 10 Z", 55, 0, Path1DPathEffectStyle.Morph) { Title = "1DPathMorph" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Path1DPathEffect()
            {
                Title = this.Title,
                Path = this.Path,
                Advance = this.Advance,
                Phase = this.Phase,
                Style = this.Style
            };
        }
    }
}
