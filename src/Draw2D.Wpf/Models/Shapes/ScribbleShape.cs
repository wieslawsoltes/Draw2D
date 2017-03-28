using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class ScribbleShape : ShapeObject
    {
        private PointShape _start;
        private ObservableCollection<PointShape> _points;

        public PointShape Start
        {
            get => _start;
            set => Update(ref _start, value);
        }

        public ObservableCollection<PointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ScribbleShape()
            : base()
        {
        }

        public ScribbleShape(PointShape start, ObservableCollection<PointShape> points)
            : base()
        {
            this.Start = start;
            this.Points = points;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (_points.Count >= 1)
            {
                r.DrawPolyLine(dc, _start, _points, Style, dx, dy);
            }

            _start.Draw(dc, r, dx, dy);

            foreach (var point in _points)
            {
                point.Draw(dc, r, dx, dy);
            }

            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_start))
            {
                _start.Move(selected, dx, dy);
            }

            foreach (var point in Points)
            {
                if (!selected.Contains(point))
                {
                    point.Move(selected, dx, dy);
                }
            }
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);

            Start.Select(selected);

            foreach (var point in Points)
            {
                point.Select(selected);
            }
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);

            Start.Deselect(selected);

            foreach (var point in Points)
            {
                point.Deselect(selected);
            }
        }
    }
}
