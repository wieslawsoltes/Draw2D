using System;

namespace Draw2D.Editor.Intersections.Line
{
    public class RectangleLineSettings : SettingsBase
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }
}
