using System;
using System.Collections.ObjectModel;

namespace PathDemo
{
    public class PathShape : ShapeBase
    {
        public ObservableCollection<FigureShape> Figures;
        public PathFillRule FillRule;
    }
}
