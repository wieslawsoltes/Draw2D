using System.Collections.Generic;
using Draw2D.Input;

namespace Draw2D.ViewModels.Tools
{
    public interface ITool
    {
        string Title { get; }
        IList<IPointIntersection> Intersections { get; set; }
        IPointIntersection CurrentIntersection { get; set; }
        IList<IPointFilter> Filters { get; set; }
        IPointFilter CurrentFilter { get; set; }
        void LeftDown(IToolContext context, double x, double y, Modifier modifier);
        void LeftUp(IToolContext context, double x, double y, Modifier modifier);
        void RightDown(IToolContext context, double x, double y, Modifier modifier);
        void RightUp(IToolContext context, double x, double y, Modifier modifier);
        void Move(IToolContext context, double x, double y, Modifier modifier);
        void Clean(IToolContext context);
    }
}
