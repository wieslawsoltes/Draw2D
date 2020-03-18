// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Style
{
    public interface IPaint : INode, IDirty, ICopyable
    {
        PaintStyle PaintStyle { get; set; }
        // Common
        ArgbColor Color { get; set; }
        bool IsAntialias { get; set; }
        IPaintEffects Effects { get; set; }
        // Stroke
        double StrokeWidth { get; set; }
        StrokeCap StrokeCap { get; set; }
        StrokeJoin StrokeJoin { get; set; }
        double StrokeMiter { get; set; }
        bool IsScaled { get; set; }
        // Text
        Typeface Typeface { get; set; }
        double FontSize { get; set; }
        bool LcdRenderText { get; set; }
        bool SubpixelText { get; set; }
        HAlign HAlign { get; set; }
        VAlign VAlign { get; set; }
    }
}
