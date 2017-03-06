using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace PathDemo
{
    public interface IToolContext
    {
        ObservableCollection<ShapeBase> Shapes { get; set; }
        HashSet<ShapeBase> Selected { get; set; }
        PointShape GetNextPoint(Point point);
        void Capture();
        void Release();
        void Invalidate();
    }
}
