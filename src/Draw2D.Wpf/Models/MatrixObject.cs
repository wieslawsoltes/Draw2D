using System;

namespace Draw2D.Models
{
    public class MatrixObject : BaseObject
    {
        private double _m11;
        private double _m12;
        private double _m21;
        private double _m22;
        private double _offsetX;
        private double _offsetY;

        public double M11
        {
            get { return _m11; }
            set
            {
                if (value != _m11)
                {
                    _m11 = value;
                    Notify("M11");
                }
            }
        }

        public double M12
        {
            get { return _m12; }
            set
            {
                if (value != _m12)
                {
                    _m12 = value;
                    Notify("M12");
                }
            }
        }

        public double M21
        {
            get { return _m21; }
            set
            {
                if (value != _m21)
                {
                    _m21 = value;
                    Notify("M21");
                }
            }
        }

        public double M22
        {
            get { return _m22; }
            set
            {
                if (value != _m22)
                {
                    _m22 = value;
                    Notify("M22");
                }
            }
        }

        public double OffsetX
        {
            get { return _offsetX; }
            set
            {
                if (value != _offsetX)
                {
                    _offsetX = value;
                    Notify("OffsetX");
                }
            }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            set
            {
                if (value != _offsetY)
                {
                    _offsetY = value;
                    Notify("OffsetY");
                }
            }
        }

        public static readonly MatrixObject Identity = new MatrixObject(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

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
