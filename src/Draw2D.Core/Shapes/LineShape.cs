using System.Collections.Generic;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class LineShape : ConnectableShape
    {
        private PointShape _startPoint;
        private PointShape _point;

        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        public PointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        public LineShape()
            : base()
        {
        }

        public LineShape(PointShape startPoint, PointShape point)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point = point;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            r.DrawLine(dc, this, Style, dx, dy);

            if (r.Selected.Contains(_startPoint))
            {
                _startPoint.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(_point))
            {
                _point.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_startPoint))
            {
                _startPoint.Move(selected, dx, dy);
            }

            if (!selected.Contains(_point))
            {
                _point.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            StartPoint.Select(selected);
            Point.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            StartPoint.Deselect(selected);
            Point.Deselect(selected);
        }
    }
}
