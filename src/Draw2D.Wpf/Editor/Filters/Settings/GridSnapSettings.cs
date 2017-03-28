using System;
using Draw2D.Models.Style;

namespace Draw2D.Editor.Filters
{
    public class GridSnapSettings : SettingsBase
    {
        private bool _enableGuides;
        private GridSnapMode _mode;
        private double _gridSizeX;
        private double _gridSizeY;
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

        public GridSnapMode Mode
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

        public double GridSizeX
        {
            get { return _gridSizeX; }
            set
            {
                if (value != _gridSizeX)
                {
                    _gridSizeX = value;
                    Notify("GridSizeX");
                }
            }
        }

        public double GridSizeY
        {
            get { return _gridSizeY; }
            set
            {
                if (value != _gridSizeY)
                {
                    _gridSizeY = value;
                    Notify("GridSizeY");
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
