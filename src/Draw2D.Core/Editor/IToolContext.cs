using System;
using System.Collections.Generic;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.Editor
{
    public interface IToolContext
    {
        ISet<ShapeObject> Selected { get; set; }
        IShapesContainer CurrentContainer { get; set; }
        IShapesContainer WorkingContainer { get; set; }
        DrawStyle CurrentStyle { get; set; }
        ShapeObject PointShape { get; set; }
        HitTest HitTest { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
        Action Capture { get; set; }
        Action Release { get; set; }
        Action Invalidate { get; set; }
    }
}
