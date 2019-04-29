// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.VisualTree;
using Draw2D.Input;

namespace Draw2D.Controls
{
    internal struct CustomDrawOperation : ICustomDrawOperation
    {
        private IDrawTarget _drawTarget;
        private double _width;
        private double _height;
        private double _dx;
        private double _dy;
        private double _zx;
        private double _zy;

        public Rect Bounds { get; }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public CustomDrawOperation(IDrawTarget drawTarget, double width, double height, double dx, double dy, double zx, double zy)
        {
            _drawTarget = drawTarget;
            _width = width;
            _height = height;
            _dx = dx;
            _dy = dy;
            _zx = zx;
            _zy = zy;
            Bounds = new Rect(0, 0, width, height);
        }

        public void Render(IDrawingContextImpl context)
        {
            var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
            if (canvas != null && _drawTarget != null)
            {
                _drawTarget.Draw(canvas, _width, _height, _dx, _dy, _zx, _zy);
            }
        }

        public void Dispose()
        {
        }
    }

    public class ZoomControl : Border, IInputService, IZoomService
    {
        private double _zoomSpeed = 1.2;
        private double _zoomX = 1.0;
        private double _zoomY = 1.0;
        private double _offsetX = 0.0;
        private double _offsetY = 0.0;
        private bool _initializedZoom = false;
        private bool _customDraw = true;
        private IInputTarget _inputTarget = null;
        private IDrawTarget _drawTarget = null;

        public static readonly DirectProperty<ZoomControl, double> ZoomSpeedProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, double>(nameof(ZoomSpeed), o => o.ZoomSpeed, (o, v) => o.ZoomSpeed = v);

        public static readonly DirectProperty<ZoomControl, double> ZoomXProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, double>(nameof(ZoomX), o => o.ZoomX, (o, v) => o.ZoomX = v);

        public static readonly DirectProperty<ZoomControl, double> ZoomYProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, double>(nameof(ZoomY), o => o.ZoomY, (o, v) => o.ZoomY = v);

        public static readonly DirectProperty<ZoomControl, double> OffsetXProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, double>(nameof(OffsetX), o => o.OffsetX, (o, v) => o.OffsetX = v);

        public static readonly DirectProperty<ZoomControl, double> OffsetYProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, double>(nameof(OffsetY), o => o.OffsetY, (o, v) => o.OffsetY = v);

        public static readonly DirectProperty<ZoomControl, bool> CustomDrawProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, bool>(nameof(CustomDraw), o => o.CustomDraw, (o, v) => o.CustomDraw = v);

        public static readonly DirectProperty<ZoomControl, IInputTarget> InputTargetProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IInputTarget>(nameof(InputTarget), o => o.InputTarget, (o, v) => o.InputTarget = v);

        public static readonly DirectProperty<ZoomControl, IDrawTarget> DrawTargetProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IDrawTarget>(nameof(DrawTarget), o => o.DrawTarget, (o, v) => o.DrawTarget = v);

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

        public bool CustomDraw
        {
            get { return _customDraw; }
            set { SetAndRaise(CustomDrawProperty, ref _customDraw, value); }
        }

        public IInputTarget InputTarget
        {
            get { return _inputTarget; }
            set { SetAndRaise(InputTargetProperty, ref _inputTarget, value); }
        }

        public IDrawTarget DrawTarget
        {
            get { return _drawTarget; }
            set { SetAndRaise(DrawTargetProperty, ref _drawTarget, value); }
        }

        public bool IsPanning { get; set; }

        private Matrix CurrentMatrix { get; set; }

        private Point PanPosition { get; set; }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Func<bool> IsCaptured { get; set; }

        public Action Redraw { get; set; }

        public ZoomControl()
        {
            PointerWheelChanged += (sender, e) => HandlePointerWheelChanged(e);
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
            if (md != null)
            {
                this.Capture = () =>
                {
                    if (md.Captured == null)
                    {
                        md.Capture(this);
                    }
                };

                this.Release = () =>
                {
                    if (md.Captured != null)
                    {
                        md.Capture(null);
                    }
                };

                this.IsCaptured = () =>
                {
                    return md.Captured != null;
                };

                this.Redraw = () =>
                {
                    this.InvalidateVisual();
                };
            }

            if (_inputTarget != null)
            {
                // FIXME:
                _inputTarget.InputService = this;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (_inputTarget != null)
            {
                // FIXME:
                _inputTarget.InputService = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            this.Focus();
        }

        private Modifier GetModifier(InputModifiers inputModifiers)
        {
            var modifier = Modifier.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= Modifier.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= Modifier.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= Modifier.Shift;
            }

            return modifier;
        }

        public void Wheel(double delta, double x, double y)
        {
            ZoomDeltaTo(delta, x, y);
            Invalidate(true);
        }

        public void Pressed(double x, double y)
        {
            if (IsCaptured?.Invoke() == false && IsPanning == false)
            {
                IsPanning = true;
                Capture?.Invoke();
                StartPan(x, y);
                Invalidate(true);
            }
        }

        public void Released(double x, double y)
        {
            if (IsPanning == true)
            {
                Release?.Invoke();
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
                Redraw?.Invoke();
            }
        }

        public void ZoomTo(double zoom, double x, double y)
        {
            CurrentMatrix = new Matrix(zoom, 0, 0, zoom, x - (zoom * x), y - (zoom * y)) * CurrentMatrix;
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
            CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, delta.X, delta.Y) * CurrentMatrix;
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

        private void GetOffset(out double dx, out double dy, out double zx, out double zy)
        {
            dx = OffsetX;
            dy = OffsetY;
            zx = ZoomX;
            zy = ZoomY;
        }

        private Point AdjustPanPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point(point.X / zx, point.Y / zy);
        }

        private Point AdjustZoomPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point((point.X - dx) / zx, (point.Y - dy) / zy);
        }

        private Point AdjustTargetPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point((point.X - dx) / zx, (point.Y - dy) / zy);
        }

        private void HandlePointerWheelChanged(PointerWheelEventArgs e)
        {
            var zpoint = AdjustZoomPoint(e.GetPosition(this));
            Wheel(e.Delta.Y, zpoint.X, zpoint.Y);
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (_inputTarget != null)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.LeftDown(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                var zpoint = AdjustPanPoint(e.GetPosition(this));
                Pressed(zpoint.X, zpoint.Y);

                if (_inputTarget != null && IsPanning == false)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.RightDown(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (_inputTarget != null)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    InputTarget.LeftUp(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                var zpoint = AdjustPanPoint(e.GetPosition(this));
                Released(zpoint.X, zpoint.Y);

                if (_inputTarget != null && IsPanning == false)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.RightUp(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            var zpoint = AdjustPanPoint(e.GetPosition(this));
            Moved(zpoint.X, zpoint.Y);

            if (_inputTarget != null && IsPanning == false)
            {
                var tpoint = AdjustTargetPoint(e.GetPosition(this));
                _inputTarget.Move(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
            }
        }

        public void ResetZoom(bool redraw)
        {
            Reset();
            Invalidate(redraw);
        }

        public void CenterZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                Center(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void FillZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                Fill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void UniformZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                Uniform(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void UniformToFillZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                UniformToFill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_drawTarget != null)
            {
                double width = Bounds.Width;
                double height = Bounds.Height;

                if (_initializedZoom == false)
                {
                    CenterZoom(false);
                    _initializedZoom = true;
                }

                GetOffset(out double dx, out double dy, out double zx, out double zy);

                if (CustomDraw)
                {
                    context.Custom(new CustomDrawOperation(_drawTarget, width, height, dx, dy, zx, zy));
                }
                else
                {
                    _drawTarget.Draw(context, width, height, dx, dy, zx, zy);
                }
            }
        }
    }
}
