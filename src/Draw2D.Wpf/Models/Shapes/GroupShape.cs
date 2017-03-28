using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class GroupShape : ConnectableShape
    {
        private ObservableCollection<ShapeObject> _segments;

        public ObservableCollection<ShapeObject> Segments
        {
            get => _segments;
            set => Update(ref _segments, value);
        }

        public GroupShape()
            : base()
        {
            _segments = new ObservableCollection<ShapeObject>();
        }

        public GroupShape(ObservableCollection<ShapeObject> segments)
            : base()
        {
            this.Segments = segments;
        }

        public GroupShape(string name)
            : this()
        {
            this.Name = name;
        }

        public GroupShape(string name, ObservableCollection<ShapeObject> segments)
            : base()
        {
            this.Name = name;
            this.Segments = segments;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            foreach (var segment in Segments)
            {
                segment.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            foreach (var segment in Segments)
            {
                if (!selected.Contains(segment))
                {
                    segment.Move(selected, dx, dy);
                }
            }

            base.Move(selected, dx, dy);
        }
    }
}
