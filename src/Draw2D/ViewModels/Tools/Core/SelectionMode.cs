using System;

namespace Draw2D.ViewModels.Tools;

[Flags]
public enum SelectionMode
{
    None = 0,
    Point = 1,
    Shape = 2,
    All = Point | Shape
}