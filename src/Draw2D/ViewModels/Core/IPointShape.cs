// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels
{
    public interface IPointShape : IBaseShape
    {
        double X { get; set; }
        double Y { get; set; }
        HAlign HAlign { get; set; }
        VAlign VAlign { get; set; }
        IBaseShape Template { get; set; }
    }
}
