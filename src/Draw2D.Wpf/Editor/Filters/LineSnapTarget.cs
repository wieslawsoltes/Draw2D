using System;

namespace Draw2D.Editor.Filters
{
    [Flags]
    public enum LineSnapTarget
    {
        None = 0,
        Guides = 1,
        Shapes = 2
    }
}
