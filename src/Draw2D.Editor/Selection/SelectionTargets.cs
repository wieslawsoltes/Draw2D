using System;

namespace Draw2D.Editor.Selection
{
    [Flags]
    public enum SelectionTargets
    {
        None = 0,
        Shapes = 1,
        Guides = 2
    }
}
