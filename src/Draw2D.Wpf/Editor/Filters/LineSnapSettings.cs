using System;
using Draw2D.Models.Style;

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
            get { return _enableGuides; }
            set
            {
                if (value != _enableGuides)
                {
                    _enableGuides = value;
                    Notify("EnableGuides");
                }
            }
        }

        public LineSnapMode Mode
        {
            get { return _mode; }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                    Notify("Mode");
                }
            }
        }

        public LineSnapTarget Target
        {
            get { return _target; }
            set
            {
                if (value != _target)
                {
                    _target = value;
                    Notify("Target");
                }
            }
        }

        public double Threshold
        {
            get { return _threshold; }
            set
            {
                if (value != _threshold)
                {
                    _threshold = value;
                    Notify("Threshold");
                }
            }
        }

        public DrawStyle GuideStyle
        {
            get { return _guideStyle; }
            set
            {
                if (value != _guideStyle)
                {
                    _guideStyle = value;
                    Notify("GuideStyle");
                }
            }
        }
    }
}
