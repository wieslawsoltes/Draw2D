// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Input
{
    public interface IZoomService
    {
        IInputService InputService { get; set; }
        double ZoomSpeed { get; set; }
        double ZoomX { get; set; }
        double ZoomY { get; set; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        bool IsPanning { get; set; }
        void Wheel(double delta, double x, double y);
        void Pressed(double x, double y);
        void Released(double x, double y);
        void Moved(double x, double y);
        void Invalidate(bool redraw);
        void ZoomTo(double zoom, double x, double y);
        void ZoomDeltaTo(double delta, double x, double y);
        void StartPan(double x, double y);
        void PanTo(double x, double y);
        void Reset();
        void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
    }
}
