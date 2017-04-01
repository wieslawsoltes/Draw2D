using Draw2D.Core.Style;

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
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        public GridSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public double GridSizeX
        {
            get => _gridSizeX;
            set => Update(ref _gridSizeX, value);
        }

        public double GridSizeY
        {
            get => _gridSizeY;
            set => Update(ref _gridSizeY, value);
        }

        public DrawStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
