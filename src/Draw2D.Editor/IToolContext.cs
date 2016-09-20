using System;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Renderers;
using Draw2D.Models.Style;

namespace Draw2D.Editor
{
    public interface IToolContext
    {
        ShapesContainer Container { get; set; }
        ShapesContainer WorkingContainer { get; set; }
        DrawStyle Style { get; set; }
        BaseShape PointShape { get; set; }
        ShapeRenderer Renderer { get; set; }
        HitTest HitTest { get; set; }
    }
}
