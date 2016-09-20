using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class PointShape : BaseShape
    {
        private double _x;
        private double _y;
        private BaseShape _template;

        public double X
        {
            get { return _x; }
            set
            {
                if (value != _x)
                {
                    _x = value;
                    Notify("X");
                }
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (value != _y)
                {
                    _y = value;
                    Notify("Y");
                }
            }
        }

        public BaseShape Template
        {
            get { return _template; }
            set
            {
                if (value != _template)
                {
                    _template = value;
                    Notify("Template");
                }
            }
        }

        public PointShape()
        {
        }

        public PointShape(double x, double y, BaseShape template)
        {
            this.X = x;
            this.Y = y;
            this.Template = template;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            if (_template != null && r.Selected.Contains(this))
            {
                base.BeginTransform(dc, r);

                _template.Draw(dc, r, X + dx, Y + dy);

                base.EndTransform(dc, r);
            }
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }
    }
}
