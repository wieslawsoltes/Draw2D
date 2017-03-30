using System;

namespace Draw2D.Editor.Tools
{
    public class LineToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private bool _splitIntersections;

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public bool SplitIntersections
        {
            get => _splitIntersections;
            set => Update(ref _splitIntersections, value);
        }
    }
}
