// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Editor.Selection;
using Draw2D.Core.Style;

namespace Draw2D.Core.Editor.Tools
{
    public class SelectionToolSettings : SettingsBase
    {
        private SelectionMode _mode;
        private SelectionTargets _targets;
        private DrawStyle _selectionStyle;
        private bool _clearSelectionOnClean;
        private double _hitTestRadius;
        private bool _connectPoints;
        private double _connectTestRadius;
        private bool _disconnectPoints;
        private double _disconnectTestRadius;

        public SelectionMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public SelectionTargets Targets
        {
            get => _targets;
            set => Update(ref _targets, value);
        }

        public DrawStyle SelectionStyle
        {
            get => _selectionStyle;
            set => Update(ref _selectionStyle, value);
        }

        public bool ClearSelectionOnClean
        {
            get => _clearSelectionOnClean;
            set => Update(ref _clearSelectionOnClean, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double ConnectTestRadius
        {
            get => _connectTestRadius;
            set => Update(ref _connectTestRadius, value);
        }

        public bool DisconnectPoints
        {
            get => _disconnectPoints;
            set => Update(ref _disconnectPoints, value);
        }

        public double DisconnectTestRadius
        {
            get => _disconnectTestRadius;
            set => Update(ref _disconnectTestRadius, value);
        }
    }
}
