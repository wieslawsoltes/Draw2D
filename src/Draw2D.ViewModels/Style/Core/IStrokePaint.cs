// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Style
{
    public class IStrokePaint : IPaint
    {
        double StrokeWidth { get; set; }
        StrokeCap StrokeCap { get; set; }
        StrokeJoin StrokeJoin { get; set; }
        double StrokeMiter { get; set; }
        bool IsScaled { get; set; }
    }
}
