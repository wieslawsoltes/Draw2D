// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class LineToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private bool _splitIntersections;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool SplitIntersections
        {
            get => _splitIntersections;
            set => Update(ref _splitIntersections, value);
        }
    }
}
