using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Draw2D.Models;
using Draw2D.Models.Shapes;

namespace PathDemo
{
    public interface IToolContext
    {
        ISet<ShapeObject> Selected { get; set; }
        ObservableCollection<ShapeObject> Shapes { get; set; }
        PointShape GetNextPoint(Point point);
        void Capture();
        void Release();
        void Invalidate();
    }
}
