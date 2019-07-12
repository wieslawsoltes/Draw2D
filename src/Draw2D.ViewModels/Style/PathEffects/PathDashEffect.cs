// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathDashEffect : ViewModelBase, IPathEffect
    {
        private string _intervals;
        private double _phase;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Intervals
        {
            get => _intervals;
            set => Update(ref _intervals, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Phase
        {
            get => _phase;
            set => Update(ref _phase, value);
        }

        public PathDashEffect()
        {
        }

        public PathDashEffect(string intervals, double phase)
        {
            this.Intervals = intervals;
            this.Phase = phase;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathDashEffect()
            {
                Intervals = this.Intervals,
                Phase = this.Phase
            };
        }
    }
}
