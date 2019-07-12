// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class Path1DPathEffect : ViewModelBase, IPathEffect
    {
        public static Path1DPathEffectStyle[] Path1DPathEffectStyleValues { get; } = (Path1DPathEffectStyle[])Enum.GetValues(typeof(Path1DPathEffectStyle));

        private PathShape _path;
        private double _advance;
        private double _phase;
        private Path1DPathEffectStyle _style;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathShape Path
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

        public Path1DPathEffect(PathShape path, double advance, double phase, Path1DPathEffectStyle style)
        {
            this.Path = path;
            this.Advance = advance;
            this.Phase = phase;
            this.Style = style;
        }

        public void SetPath(PathShape path)
        {
            this.Path = path;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Path1DPathEffect()
            {
                Path = this.Path,
                Advance = this.Advance,
                Phase = this.Phase,
                Style = this.Style
            };
        }
    }
}
