using System;

namespace Draw2D.Editor.Tools
{
    public class PolyLineToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;

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
    }
}
