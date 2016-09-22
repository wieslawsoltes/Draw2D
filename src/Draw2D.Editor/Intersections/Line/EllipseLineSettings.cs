using System;

namespace Draw2D.Editor.Intersections.Line
{
    public class EllipseLineSettings : SettingsBase
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    Notify("IsEnabled");
                }
            }
        }
    }
}
