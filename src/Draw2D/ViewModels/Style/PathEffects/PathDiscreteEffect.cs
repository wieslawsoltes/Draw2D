// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathDiscreteEffect : ViewModelBase, IPathEffect
    {
        private double _segLength;
        private double _deviation;
        private uint _seedAssist;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double SegLength
        {
            get => _segLength;
            set => Update(ref _segLength, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Deviation
        {
            get => _deviation;
            set => Update(ref _deviation, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public uint SeedAssist
        {
            get => _seedAssist;
            set => Update(ref _seedAssist, value);
        }

        public PathDiscreteEffect()
        {
        }

        public PathDiscreteEffect(double segLength, double deviation, uint seedAssist = 0)
        {
            this.SegLength = segLength;
            this.Deviation = deviation;
            this.SeedAssist = seedAssist;
        }

        public static IPathEffect MakeDiscrete()
        {
            return new PathDiscreteEffect(3, 10) { Title = "Discrete" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathDiscreteEffect()
            {
                Title = this.Title,
                SegLength = this.SegLength,
                Deviation = this.Deviation,
                SeedAssist = this.SeedAssist
            };
        }
    }
}
