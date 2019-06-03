// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Filters
{
    [DataContract(IsReference = true)]
    public class GridSnapSettings : Settings
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private GridSnapMode _mode;
        private double _gridSizeX;
        private double _gridSizeY;
        private string _guideStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GridSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double GridSizeX
        {
            get => _gridSizeX;
            set => Update(ref _gridSizeX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double GridSizeY
        {
            get => _gridSizeY;
            set => Update(ref _gridSizeY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
