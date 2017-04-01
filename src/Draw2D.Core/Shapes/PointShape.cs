using System.Collections.Generic;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class PointShape : ShapeObject
    {
        private double _x;
        private double _y;
        private ShapeObject _template;

        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        public ShapeObject Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public PointShape()
        {
        }

        public PointShape(double x, double y, ShapeObject template)
        {
            this.X = x;
            this.Y = y;
            this.Template = template;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            if (_template != null)
            {
                base.BeginTransform(dc, r);

                _template.Draw(dc, r, X + dx, Y + dy);

                base.EndTransform(dc, r);
            }
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public PointShape Clone()
        {
            return new PointShape()
            {
                X = this.X,
                Y = this.Y,
                Template = this.Template
            };
        }
    }
}
