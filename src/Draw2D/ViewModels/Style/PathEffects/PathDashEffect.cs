using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathDashEffect : ViewModelBase, IPathEffect
    {
        private string _intervals;
        private double _phase;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Intervals
        {
            get => _intervals;
            set => Update(ref _intervals, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Phase
        {
            get => _phase;
            set => Update(ref _phase, value);
        }

        public PathDashEffect()
        {
        }

        public PathDashEffect(string intervals, double phase)
        {
            this.Intervals = intervals;
            this.Phase = phase;
        }

        public static IPathEffect MakeDash()
        {
            return new PathDashEffect("2 2", 1.0) { Title = "Dash" };
        }

        public static IPathEffect MakeDot()
        {
            return new PathDashEffect("0 2", 0.0) { Title = "Dot" };
        }

        public static IPathEffect MakeDashDot()
        {
            return new PathDashEffect("2 2 0 2", 1.0) { Title = "DashDot" };
        }

        public static IPathEffect MakeDashDotDot()
        {
            return new PathDashEffect("2 2 0 2 0 2", 1.0) { Title = "DashDotDot" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathDashEffect()
            {
                Title = this.Title,
                Intervals = this.Intervals,
                Phase = this.Phase
            };
        }
    }
}
