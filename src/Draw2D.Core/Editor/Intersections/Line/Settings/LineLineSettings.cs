namespace Draw2D.Editor.Intersections.Line
{
    public class LineLineSettings : SettingsBase
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }
}
