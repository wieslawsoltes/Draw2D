// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Style;

namespace Draw2D.Editor.Filters
{
    public class LineSnapSettings : SettingsBase
    {
        private bool _enableGuides;
        private LineSnapMode _mode;
        private LineSnapTarget _target;
        private double _threshold;
        private DrawStyle _guideStyle;

        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        public LineSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public LineSnapTarget Target
        {
            get => _target;
            set => Update(ref _target, value);
        }

        public double Threshold
        {
            get => _threshold;
            set => Update(ref _threshold, value);
        }

        public DrawStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
