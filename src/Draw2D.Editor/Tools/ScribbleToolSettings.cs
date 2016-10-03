using System;

namespace Draw2D.Editor.Tools
{
    public class ScribbleToolSettings : SettingsBase
    {
        private bool _simplify;
        private double _epsilon;

        public bool Simplify
        {
            get { return _simplify; }
            set
            {
                if (value != _simplify)
                {
                    _simplify = value;
                    Notify("Simplify");
                }
            }
        }

        public double Epsilon
        {
            get { return _epsilon; }
            set
            {
                if (value != _epsilon)
                {
                    _epsilon = value;
                    Notify("Epsilon");
                }
            }
        }
    }
}
