using System;
using System.Collections.ObjectModel;
using Draw2D.Models.Shapes;

namespace Draw2D.Models.Containers
{
    public class ShapesContainer : BaseObject
    {
        private double _width;
        private double _height;
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
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<BaseShape>();
        }

        public ShapesContainer(ObservableCollection<BaseShape> shapes)
        {
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
            this.Guides = new ObservableCollection<LineShape>();
            this.Shapes = shapes;
        }

        public ShapesContainer(string name, ObservableCollection<LineShape> guides, ObservableCollection<BaseShape> shapes)
        {
            this.Name = name;
            this.Guides = guides;
            this.Shapes = shapes;
        }
    }
}
