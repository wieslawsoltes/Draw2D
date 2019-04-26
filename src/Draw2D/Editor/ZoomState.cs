// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.MatrixExtensions;
using Avalonia.Media;
using Avalonia.VisualTree;
using Draw2D.ViewModels;

namespace Draw2D.Editor
{
    public class ZoomState : AvaloniaObject
    {
        private double _zoomSpeed = 1.2;
        private double _zoomX = 1.0;
        private double _zoomY = 1.0;
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;

        public static readonly DirectProperty<ZoomState, double> ZoomSpeedProperty =
           AvaloniaProperty.RegisterDirect<ZoomState, double>(nameof(ZoomSpeed), o => o.ZoomSpeed, (o, v) => o.ZoomSpeed = v);

        public static readonly DirectProperty<ZoomState, double> ZoomXProperty =
           AvaloniaProperty.RegisterDirect<ZoomState, double>(nameof(ZoomX), o => o.ZoomX, (o, v) => o.ZoomX = v);

        public static readonly DirectProperty<ZoomState, double> ZoomYProperty =
           AvaloniaProperty.RegisterDirect<ZoomState, double>(nameof(ZoomY), o => o.ZoomY, (o, v) => o.ZoomY = v);

        public static readonly DirectProperty<ZoomState, double> OffsetXProperty =
           AvaloniaProperty.RegisterDirect<ZoomState, double>(nameof(OffsetX), o => o.OffsetX, (o, v) => o.OffsetX = v);

        public static readonly DirectProperty<ZoomState, double> OffsetYProperty =
           AvaloniaProperty.RegisterDirect<ZoomState, double>(nameof(OffsetY), o => o.OffsetY, (o, v) => o.OffsetY = v);

        public double ZoomSpeed
        {
            get { return _zoomSpeed; }
            set { SetAndRaise(ZoomSpeedProperty, ref _zoomSpeed, value); }
        }

        public double ZoomX
        {
            get { return _zoomX; }
            set { SetAndRaise(ZoomXProperty, ref _zoomX, value); }
        }

        public double ZoomY
        {
            get { return _zoomY; }
            set { SetAndRaise(ZoomYProperty, ref _zoomY, value); }
        }

        public double OffsetX
        {
            get { return _offsetX; }
            set { SetAndRaise(OffsetXProperty, ref _offsetX, value); }
        }

        public double OffsetY
        {
            get { return _offsetY; }
            set { SetAndRaise(OffsetYProperty, ref _offsetY, value); }
        }

        public bool IsPanning { get; set; }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Func<bool> IsCaptured { get; set; }

        public Action Redraw { get; set; }

        private Matrix CurrentMatrix { get; set; }

        private Point PanPosition { get; set; }

        public void Wheel(double delta, double x, double y)
        {
            ZoomDeltaTo(delta, x, y);
            Invalidate(true);
        }

        public void Pressed(double x, double y)
        {
            if (IsCaptured() == false && IsPanning == false)
            {
                IsPanning = true;
                Capture();
                StartPan(x, y);
                Invalidate(true);
            }
        }

        public void Released(double x, double y)
        {
            if (IsPanning == true)
            {
                Release();
                Invalidate(true);
                IsPanning = false;
            }
        }

        public void Moved(double x, double y)
        {
            if (IsPanning == true)
            {
                PanTo(x, y);
                Invalidate(true);
            }
        }

        public void Invalidate(bool redraw)
        {
            ZoomX = CurrentMatrix.M11;
            ZoomY = CurrentMatrix.M22;
            OffsetX = CurrentMatrix.M31;
            OffsetY = CurrentMatrix.M32;
            if (redraw)
            {
                Redraw();
            }
        }

        public void ZoomTo(double zoom, double x, double y)
        {
            CurrentMatrix = MatrixHelper.ScaleAtPrepend(CurrentMatrix, zoom, zoom, x, y);
        }

        public void ZoomDeltaTo(double delta, double x, double y)
        {
            ZoomTo(delta > 0 ? ZoomSpeed : 1 / ZoomSpeed, x, y);
        }

        public void StartPan(double x, double y)
        {
            PanPosition = new Point(x, y);
        }

        public void PanTo(double x, double y)
        {
            double dx = x - PanPosition.X;
            double dy = y - PanPosition.Y;
            Point delta = new Point(dx, dy);
            PanPosition = new Point(x, y);
            CurrentMatrix = MatrixHelper.TranslatePrepend(CurrentMatrix, delta.X, delta.Y);
        }

        public void Reset()
        {
            CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
        }

        public void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double ox = (panelWidth - elementWidth) / 2;
            double oy = (panelHeight - elementHeight) / 2;
            CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, ox, oy);
        }

        public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double zx = panelWidth / elementWidth;
            double zy = panelHeight / elementHeight;
            double ox = (panelWidth - elementWidth * zx) / 2;
            double oy = (panelHeight - elementHeight * zy) / 2;
            CurrentMatrix = new Matrix(zx, 0.0, 0.0, zy, ox, oy);
        }

        public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double zx = panelWidth / elementWidth;
            double zy = panelHeight / elementHeight;
            double zoom = Math.Min(zx, zy);
            double ox = (panelWidth - elementWidth * zoom) / 2;
            double oy = (panelHeight - elementHeight * zoom) / 2;
            CurrentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
        }

        public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double zx = panelWidth / elementWidth;
            double zy = panelHeight / elementHeight;
            double zoom = Math.Max(zx, zy);
            double ox = (panelWidth - elementWidth * zoom) / 2;
            double oy = (panelHeight - elementHeight * zoom) / 2;
            CurrentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
        }
    }
}
