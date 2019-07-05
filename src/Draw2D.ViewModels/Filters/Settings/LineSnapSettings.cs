// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Filters
{
    [DataContract(IsReference = true)]
    public class LineSnapSettings : SettingsBase
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private LineSnapMode _mode;
        private LineSnapTarget _target;
        private double _threshold;
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
        public LineSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapTarget Target
        {
            get => _target;
            set => Update(ref _target, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Threshold
        {
            get => _threshold;
            set => Update(ref _threshold, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
