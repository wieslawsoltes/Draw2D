using System;

namespace Draw2D.Editor.Filters
{
    [Flags]
    public enum LineSnapMode
    {
        None = 0,
        Point = 1,
        Middle = 2,
        Intersection = 4,
        Horizontal = 8,
        Vertical = 16,
        Nearest = 32
    }
}
