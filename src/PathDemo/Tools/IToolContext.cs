using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Models;
using Draw2D.Models.Shapes;

namespace PathDemo.Tools
{
    public interface IToolContext
    {
        ISet<ShapeObject> Selected { get; set; }
        ObservableCollection<ShapeObject> Shapes { get; set; }
        PointShape GetNextPoint(double x, double y);
        void Capture();
        void Release();
        void Invalidate();
    }
}
