namespace Draw2D.Editor.Tools
{
    public class ScribbleToolSettings : SettingsBase
    {
        private bool _simplify;
        private double _epsilon;

        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }
    }
}
