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
    public class ZoomState
    {
        public Matrix CurrentMatrix { get; set; }
        public Point PanPosition { get; set; }
        public bool IsPanning { get; set; }
        public double ZoomSpeed { get; set; } = 1.2;
        public double ZoomX { get; set; } = 1.0;
        public double ZoomY { get; set; } = 1.0;
        public double OffsetX { get; set; } = 0.0;
        public double OffsetY { get; set; } = 0.0;
        public Action Capture { get; set; }
        public Action Release { get; set; }
        public Action Redraw { get; set; }

        public void Wheel(double delta, double x, double y)
        {
            ZoomDeltaTo(delta, x, y);
            Invalidate(true);
        }

        public void Pressed(double x, double y)
        {
            if (e.Device.Captured == null && IsPanning == false)
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

        public void Reset()
        {
            CurrentMatrix = Matrix.Identity;
        }

        public void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double ox = (panelWidth - elementWidth) / 2;
            double oy = (panelHeight - elementHeight) / 2;
            CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, ox, oy);
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

        public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double zx = panelWidth / elementWidth;
            double zy = panelHeight / elementHeight;
            double cx = elementWidth / 2.0;
            double cy = elementHeight / 2.0;
            CurrentMatrix = MatrixHelper.ScaleAt(zx, zy, cx, cy);
        }

        public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double zx = panelWidth / elementWidth;
            double zy = panelHeight / elementHeight;
            double cx = elementWidth / 2.0;
            double cy = elementHeight / 2.0;
            double zoom = Math.Min(zx, zy);
            CurrentMatrix = MatrixHelper.ScaleAt(zoom, zoom, cx, cy);
        }

        public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            double zx = panelWidth / elementWidth;
            double zy = panelHeight / elementHeight;
            double cx = elementWidth / 2.0;
            double cy = elementHeight / 2.0;
            double zoom = Math.Max(zx, zy);
            CurrentMatrix = MatrixHelper.ScaleAt(zoom, zoom, cx, cy);
        }
    }
}
