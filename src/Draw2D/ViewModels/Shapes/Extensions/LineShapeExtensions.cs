﻿using System.Collections.ObjectModel;
using Spatial;

namespace Draw2D.ViewModels.Shapes;

public static class LineShapeExtensions
{
    public static Line2 ToLine2(this LineShape line, double dx = 0.0, double dy = 0.0)
    {
        return Line2.FromPoints(
            line.StartPoint.X, line.StartPoint.Y,
            line.Point.X, line.Point.Y,
            dx, dy);
    }

    public static LineShape FromLine2(this Line2 line)
    {
        return new LineShape(line.A.FromPoint2(), line.B.FromPoint2())
        {
            Points = new ObservableCollection<IPointShape>()
        };
    }
}