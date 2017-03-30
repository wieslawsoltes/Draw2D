using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public abstract class ConnectableShape : ShapeObject
    {
        private ObservableCollection<PointShape> _points;

        public ObservableCollection<PointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ConnectableShape()
        {
            _points = new ObservableCollection<PointShape>();
        }

        public ConnectableShape(ObservableCollection<PointShape> points)
        {
            this.Points = points;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            foreach (var point in Points)
            {
                point.Draw(dc, r, dx, dy);
            }
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
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

            foreach (var point in Points)
            {
                point.Select(selected);
            }
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);

            foreach (var point in Points)
            {
                point.Deselect(selected);
            }
        }
    }
}
