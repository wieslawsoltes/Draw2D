using System;
using System.Collections.ObjectModel;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.Models.Containers
{
    public class ShapesContainer : BaseObject, IShapesContainer
    {
        private double _width;
        private double _height;
        private ObservableCollection<DrawStyle> _styles;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<BaseShape> _shapes;

        public double Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    Notify("Width");
                }
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    Notify("Height");
                }
            }
        }

        public ObservableCollection<DrawStyle> Styles
        {
            get { return _styles; }
            set
            {
                if (value != _styles)
                {
                    _styles = value;
                    Notify("Styles");
                }
            }
        }

        public ObservableCollection<LineShape> Guides
        {
            get { return _guides; }
            set
            {
                if (value != _guides)
                {
                    _guides = value;
                    Notify("Guides");
                }
            }
        }

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
