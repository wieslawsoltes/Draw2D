using System;

namespace Draw2D.Models.Style
{
    public class DrawColor : BaseObject
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        public byte A
        {
            get { return _a; }
            set
            {
                if (value != _a)
                {
                    _a = value;
                    Notify("A");
                }
            }
        }

        public byte R
        {
            get { return _r; }
            set
            {
                if (value != _r)
                {
                    _r = value;
                    Notify("R");
                }
            }
        }

        public byte G
        {
            get { return _g; }
            set
            {
                if (value != _g)
                {
                    _g = value;
                    Notify("G");
                }
            }
        }

        public byte B
        {
            get { return _b; }
            set
            {
                if (value != _b)
                {
                    _b = value;
                    Notify("B");
                }
            }
        }

        public DrawColor()
        {
        }

        public DrawColor(byte a, byte r, byte g, byte b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }
}
