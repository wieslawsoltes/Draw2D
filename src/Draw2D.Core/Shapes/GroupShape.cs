using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models.Containers;
using Draw2D.Models.Renderers;
using Draw2D.Models.Style;

namespace Draw2D.Models.Shapes
{
    public class GroupShape : ConnectableShape, IShapesContainer
    {
        private double _width;
        private double _height;
        private ObservableCollection<DrawStyle> _styles;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<ShapeObject> _shapes;

        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        public ObservableCollection<DrawStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        public ObservableCollection<LineShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public ObservableCollection<ShapeObject> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public GroupShape()
            : base()
        {
            _shapes = new ObservableCollection<ShapeObject>();
        }

        public GroupShape(ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public GroupShape(string name)
            : this()
        {
            this.Name = name;
        }

        public GroupShape(string name, ObservableCollection<ShapeObject> shapes)
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

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
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
