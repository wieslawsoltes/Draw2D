// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Style
{
    public interface IShapeStyle : INode, IDirty, ICopyable
    {
        bool IsStroked { get; set; }
        bool IsFilled { get; set; }
        bool IsText { get; set; }
        IPaint StrokePaint { get; set; }
        IPaint FillPaint { get; set; }
        IPaint TextPaint { get; set; }
    }
}
