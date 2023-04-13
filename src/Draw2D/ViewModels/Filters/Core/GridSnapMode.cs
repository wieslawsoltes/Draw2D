﻿using System;

namespace Draw2D.ViewModels.Filters;

[Flags]
public enum GridSnapMode
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    All = Horizontal | Vertical
}