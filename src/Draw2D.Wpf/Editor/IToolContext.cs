using System;
using System.Collections.Generic;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Style;

namespace Draw2D.Editor
{
    public interface IToolContext
    {
        HashSet<BaseShape> Selected { get; set; }
        IShapesContainer CurrentContainer { get; set; }
        IShapesContainer WorkingContainer { get; set; }
        DrawStyle Style { get; set; }
        BaseShape PointShape { get; set; }
        HitTest HitTest { get; set; }
    }
}
