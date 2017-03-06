using System;
using System.Collections.ObjectModel;

namespace Draw2D.Models.Shapes
{
    public class FigureShape : GroupShape
    {
        private bool _isFilled;
        private bool _isClosed;

        public bool IsFilled
        {
            get { return _isFilled; }
            set
            {
                if (value != _isFilled)
                {
                    _isFilled = value;
                    Notify("IsFilled");
                }
            }
        }

        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                if (value != _isClosed)
                {
                    _isClosed = value;
                    Notify("IsClosed");
                }
            }
        }

        public FigureShape()
            : base()
        {
        }

        public FigureShape(ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Segments = shapes;
        }

        public FigureShape(string name)
            : this()
        {
            this.Name = name;
        }

        public FigureShape(string name, ObservableCollection<BaseShape> shapes)
            : base()
        {
            this.Name = name;
            this.Segments = shapes;
        }
    }
}
