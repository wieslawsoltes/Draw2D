using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class ArcToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private double _startAngle;
        private double _sweepAngle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double StartAngle
        {
            get => _startAngle;
            set => Update(ref _startAngle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double SweepAngle
        {
            get => _sweepAngle;
            set => Update(ref _sweepAngle, value);
        }
    }
}
