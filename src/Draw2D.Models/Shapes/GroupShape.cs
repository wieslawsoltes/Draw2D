using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class GroupShape : ConnectableShape
    {
        private ObservableCollection<BaseShape> _shapes;

        public ObservableCollection<BaseShape> Shapes
        {
            get { return _shapes; }
            set
            {
                if (value != _shapes)
                {
                    _shapes = value;
                    Notify("Shapes");
                }
            }
        }

        public GroupShape()
            : base()
        {
            _shapes = new ObservableCollection<BaseShape>();
        }

        public GroupShape(ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public GroupShape(string name)
            : this()
        {
            this.Name = name;
        }

        public GroupShape(string name, ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            foreach (var shape in Shapes)
            {
                if (!selected.Contains(shape))
                {
                    shape.Move(selected, dx, dy);
                }
            }

            base.Move(selected, dx, dy);
        }
    }
}
