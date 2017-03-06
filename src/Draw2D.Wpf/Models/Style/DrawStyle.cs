using System;

namespace Draw2D.Models.Style
{
    public class DrawStyle : BaseObject
    {
        private DrawColor _stroke;
        private DrawColor _fill;
        private double _thickness;
        private bool _isStroked;
        private bool _isFilled;

        public DrawColor Stroke
        {
            get { return _stroke; }
            set
            {
                if (value != _stroke)
                {
                    _stroke = value;
                    Notify("Stroke");
                }
            }
        }

        public DrawColor Fill
        {
            get { return _fill; }
            set
            {
                if (value != _fill)
                {
                    _fill = value;
                    Notify("Fill");
                }
            }
        }

        public double Thickness
        {
            get { return _thickness; }
            set
            {
                if (value != _thickness)
                {
                    _thickness = value;
                    Notify("Thickness");
                }
            }
        }

        public bool IsStroked
        {
            get { return _isStroked; }
            set
            {
                if (value != _isStroked)
                {
                    _isStroked = value;
                    Notify("IsStroked");
                }
            }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set
            {
                if (value != _isFilled)
                {
                    _isFilled = value;
                    Notify("IsFilled");
                }
            }
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
