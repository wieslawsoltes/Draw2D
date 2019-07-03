// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class PathToolSettings : Settings
    {
        public static PathFillType[] PathFillTypeValues { get; } = (PathFillType[])Enum.GetValues(typeof(PathFillType));

        private bool _connectPoints;
        private double _hitTestRadius;
        private PathFillType _fillType;
        private bool _isFilled;
        private bool _isClosed;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private ITool _previousTool;

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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                PreviousTool = _currentTool;
                Update(ref _currentTool, value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool PreviousTool
        {
            get => _previousTool;
            set => Update(ref _previousTool, value);
        }
    }
}
