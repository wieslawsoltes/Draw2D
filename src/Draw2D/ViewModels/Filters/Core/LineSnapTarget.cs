using System;

namespace Draw2D.ViewModels.Filters
{
    [Flags]
    public enum LineSnapTarget
    {
        None = 0,
        Shapes = 1,
        All = Shapes
    }
}
