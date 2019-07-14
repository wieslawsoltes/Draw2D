// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Style
{
    public class ITextPaint : IPaint
    {
        Typeface Typeface { get; set; }
        double FontSize { get; set; }
        bool LcdRenderText { get; set; }
        bool SubpixelText { get; set; }
        HAlign HAlign { get; set; }
        VAlign VAlign { get; set; }
    }
}
