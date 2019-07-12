// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathTrimEffect : ViewModelBase, IPathEffect
    {
        public static TrimPathEffectMode[] TrimPathEffectModeValues { get; } = (TrimPathEffectMode[])Enum.GetValues(typeof(TrimPathEffectMode));

        private double _start;
        private double _stop;
        private TrimPathEffectMode _mode;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Start
        {
            get => _start;
            set => Update(ref _start, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Stop
        {
            get => _stop;
            set => Update(ref _stop, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TrimPathEffectMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public PathTrimEffect()
        {
        }

        public PathTrimEffect(double start, double stop, TrimPathEffectMode mode = TrimPathEffectMode.Normal)
        {
            this.Start = start;
            this.Stop = stop;
            this.Mode = mode;
        }

        public static IPathEffect MakeTrim()
        {
            return new PathTrimEffect(0, 1) { Title = "Trim" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathTrimEffect()
            {
                Title = this.Title,
                Start = this.Start,
                Stop = this.Stop,
                Mode = this.Mode
            };
        }
    }
}
