using System;

namespace Draw2D.ViewModels.Tools;

[Flags]
public enum SelectionTargets
{
    None = 0,
    Shapes = 1,
    Guides = 2,
    All = Shapes | Guides
}