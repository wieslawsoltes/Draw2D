using System;

namespace Draw2D.Editor.Selection
{
    [Flags]
    public enum SelectionMode
    {
        None = 0,
        Point = 1,
        Shape = 2
    }
}
