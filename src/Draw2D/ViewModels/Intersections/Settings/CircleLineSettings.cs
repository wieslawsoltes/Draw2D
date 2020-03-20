using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Intersections
{
    [DataContract(IsReference = true)]
    public class CircleLineSettings : SettingsBase
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }
}
