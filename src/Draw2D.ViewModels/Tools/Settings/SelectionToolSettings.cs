// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.Input;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class SelectionToolSettings : SettingsBase
    {
        private SelectionMode _mode;
        private SelectionTargets _targets;
        private Modifier _selectionModifier;
        private Modifier _connectionModifier;
        private string _selectionStyle;
        private bool _clearSelectionOnClean;
        private double _hitTestRadius;
        private bool _connectPoints;
        private double _connectTestRadius;
        private bool _disconnectPoints;
        private double _disconnectTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionTargets Targets
        {
            get => _targets;
            set => Update(ref _targets, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Modifier SelectionModifier
        {
            get => _selectionModifier;
            set => Update(ref _selectionModifier, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Modifier ConnectionModifier
        {
            get => _connectionModifier;
            set => Update(ref _connectionModifier, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string SelectionStyle
        {
            get => _selectionStyle;
            set => Update(ref _selectionStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ClearSelectionOnClean
        {
            get => _clearSelectionOnClean;
            set => Update(ref _clearSelectionOnClean, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ConnectTestRadius
        {
            get => _connectTestRadius;
            set => Update(ref _connectTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool DisconnectPoints
        {
            get => _disconnectPoints;
            set => Update(ref _disconnectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double DisconnectTestRadius
        {
            get => _disconnectTestRadius;
            set => Update(ref _disconnectTestRadius, value);
        }
    }
}
