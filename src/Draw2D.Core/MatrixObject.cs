using System;

namespace Draw2D.Models
{
    public class MatrixObject : ObservableObject
    {
        private double _m11;
        private double _m12;
        private double _m21;
        private double _m22;
        private double _offsetX;
        private double _offsetY;

        public double M11
        {
            get => _m11;
            set => Update(ref _m11, value);
        }

        public double M12
        {
            get => _m12;
            set => Update(ref _m12, value);
        }

        public double M21
        {
            get => _m21;
            set => Update(ref _m21, value);
        }

        public double M22
        {
            get => _m22;
            set => Update(ref _m22, value);
        }

        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        public static MatrixObject Identity => new MatrixObject(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

        public MatrixObject()
            : base()
        {
        }

        public MatrixObject(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
            : base()
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M21 = m21;
            this.M22 = m22;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }
    }
}
