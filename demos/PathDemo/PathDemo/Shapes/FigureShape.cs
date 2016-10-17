using System;
using System.Collections.ObjectModel;

namespace PathDemo
{
    public class FigureShape : ShapeBase
    {
        public ObservableCollection<ShapeBase> Segments;
        public bool IsFilled;
        public bool IsClosed;
    }
}
