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
            get { return _connectPoints; }
            set
            {
                if (value != _connectPoints)
                {
                    _connectPoints = value;
                    Notify("ConnectPoints");
                }
            }
        }

        public double HitTestRadius
        {
            get { return _hitTestRadius; }
            set
            {
                if (value != _hitTestRadius)
                {
                    _hitTestRadius = value;
                    Notify("HitTestRadius");
                }
            }
        }

        public bool SplitIntersections
        {
            get { return _splitIntersections; }
            set
            {
                if (value != _splitIntersections)
                {
                    _splitIntersections = value;
                    Notify("SplitIntersections");
                }
            }
        }
    }
}
