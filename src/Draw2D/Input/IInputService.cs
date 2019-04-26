// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Draw2D.Input
{
    public interface IInputService
    {
        Action Capture { get; set; }
        Action Release { get; set; }
        Func<bool> IsCaptured { get; set; }
        Action Redraw { get; set; }
    }
}
