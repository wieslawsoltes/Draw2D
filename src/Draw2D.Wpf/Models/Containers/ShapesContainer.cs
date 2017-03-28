using System;
using System.Collections.ObjectModel;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.Models.Containers
{
    public class ShapesContainer : IdObject, IShapesContainer
    {
        private double _width;
        private double _height;
        private ObservableCollection<DrawStyle> _styles;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<BaseShape> _shapes;

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

        public ObservableCollection<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public ShapesContainer()
        {
            _styles = new ObservableCollection<DrawStyle>();
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<BaseShape>();
        }

        public ShapesContainer(ObservableCollection<BaseShape> shapes)
        {
            this.Styles = new ObservableCollection<DrawStyle>();
            this.Guides = new ObservableCollection<LineShape>();
            this.Shapes = shapes;
        }

        public ShapesContainer(string name)
            : this()
        {
            this.Name = name;
        }

        public ShapesContainer(string name, ObservableCollection<BaseShape> shapes)
        {
            this.Name = name;
            this.Styles = new ObservableCollection<DrawStyle>();
            this.Guides = new ObservableCollection<LineShape>();
            this.Shapes = shapes;
        }

        public ShapesContainer(string name, ObservableCollection<LineShape> guides, ObservableCollection<BaseShape> shapes)
        {
            this.Name = name;
            this.Styles = new ObservableCollection<DrawStyle>();
            this.Guides = guides;
            this.Shapes = shapes;
        }

        public ShapesContainer(string name, ObservableCollection<DrawStyle> styles, ObservableCollection<LineShape> guides, ObservableCollection<BaseShape> shapes)
        {
            this.Name = name;
            this.Styles = styles;
            this.Guides = guides;
            this.Shapes = shapes;
        }
    }
}
