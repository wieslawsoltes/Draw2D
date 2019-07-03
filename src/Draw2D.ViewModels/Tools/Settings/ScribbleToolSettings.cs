// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class ScribbleToolSettings : Settings
    {
        public static PathFillType[] PathFillTypeValues { get; } = (PathFillType[])Enum.GetValues(typeof(PathFillType));

        private bool _simplify;
        private double _epsilon;
        private PathFillType _fillType;
        private bool _isFilled;
        private bool _isClosed;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillType FillType
        {
            get => _fillType;
            set => Update(ref _fillType, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }
    }
}
