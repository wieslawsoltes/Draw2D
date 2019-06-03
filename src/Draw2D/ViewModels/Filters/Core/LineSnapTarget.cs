// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
