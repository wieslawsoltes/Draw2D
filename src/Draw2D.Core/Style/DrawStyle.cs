namespace Draw2D.Core.Style
{
    public class DrawStyle : IdObject
    {
        private DrawColor _stroke;
        private DrawColor _fill;
        private double _thickness;
        private bool _isStroked;
        private bool _isFilled;

        public DrawColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        public DrawColor Fill
        {
            get => _fill;
            set => Update(ref _fill, value);
        }

        public double Thickness
        {
            get => _thickness;
            set => Update(ref _thickness, value);
        }

        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        public DrawStyle()
        {
        }

        public DrawStyle(DrawColor stroke, DrawColor fill, double thickness, bool isStroked, bool isFilled)
        {
            this.Stroke = stroke;
            this.Fill = fill;
            this.Thickness = thickness;
            this.IsStroked = isStroked;
            this.IsFilled = isFilled;
        }
    }
}
