using System;
using System.Collections.Generic;
using Draw2D.Editor.Bounds;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Editor
{
    public interface IToolContext
    {
        ShapeRenderer Renderer { get; set; }
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
