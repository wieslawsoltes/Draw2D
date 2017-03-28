using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class QuadraticBezierShape : ConnectableShape
    {
        private PointShape _startPoint;
        private PointShape _point1;
        private PointShape _point2;

        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        public PointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        public PointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        public QuadraticBezierShape()
            : base()
        {
        }

        public QuadraticBezierShape(PointShape startPoint, PointShape point1, PointShape point2)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            r.DrawQuadraticBezier(dc, this, Style, dx, dy);

            _startPoint.Draw(dc, r, dx, dy);
            _point1.Draw(dc, r, dx, dy);
            _point2.Draw(dc, r, dx, dy);

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_startPoint))
            {
                _startPoint.Move(selected, dx, dy);
            }

            if (!selected.Contains(_point1))
            {
                _point1.Move(selected, dx, dy);
            }

            if (!selected.Contains(_point2))
            {
                _point2.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            StartPoint.Select(selected);
            Point1.Select(selected);
            Point2.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            StartPoint.Deselect(selected);
            Point1.Deselect(selected);
            Point2.Deselect(selected);
        }
    }
}
