// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Input
{
    public interface IZoomServiceState
    {
        double ZoomSpeed { get; set; }
        double ZoomX { get; set; }
        double ZoomY { get; set; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        bool IsPanning { get; set; }
        bool IsZooming { get; set; }
        FitMode InitFitMode { get; set; }
        FitMode AutoFitMode { get; set; }
    }
}
