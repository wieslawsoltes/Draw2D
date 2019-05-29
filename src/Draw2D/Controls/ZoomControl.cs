// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Draw2D.Input;

namespace Draw2D.Controls
{
    public class ZoomControl : Border, IInputService, IZoomService
    {
        private IZoomServiceState _zoomServiceState = null;
        private IInputTarget _inputTarget = null;
        private IDrawTarget _drawTarget = null;

        public static readonly DirectProperty<ZoomControl, IZoomServiceState> ZoomServiceStateProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IZoomServiceState>(nameof(ZoomServiceState), o => o.ZoomServiceState, (o, v) => o.ZoomServiceState = v);

        public static readonly DirectProperty<ZoomControl, IInputTarget> InputTargetProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IInputTarget>(nameof(InputTarget), o => o.InputTarget, (o, v) => o.InputTarget = v);

        public static readonly DirectProperty<ZoomControl, IDrawTarget> DrawTargetProperty =
           AvaloniaProperty.RegisterDirect<ZoomControl, IDrawTarget>(nameof(DrawTarget), o => o.DrawTarget, (o, v) => o.DrawTarget = v);

        public IZoomServiceState ZoomServiceState
        {
            get { return _zoomServiceState; }
            set { SetAndRaise(ZoomServiceStateProperty, ref _zoomServiceState, value); }
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

        private void GetOffset(out double dx, out double dy, out double zx, out double zy)
        {
            dx = _zoomServiceState.OffsetX;
            dy = _zoomServiceState.OffsetY;
            zx = _zoomServiceState.ZoomX;
            zy = _zoomServiceState.ZoomY;
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
            if (_zoomServiceState != null)
            {
                var zpoint = AdjustZoomPoint(e.GetPosition(this));
                Wheel(e.Delta.Y, zpoint.X, zpoint.Y);
            }
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (e.MouseButton == MouseButton.Left)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.LeftDown(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
                else if (e.MouseButton == MouseButton.Right)
                {
                    var zpoint = AdjustPanPoint(e.GetPosition(this));
                    Pressed(zpoint.X, zpoint.Y);

                    if (_zoomServiceState.IsPanning == false)
                    {
                        var tpoint = AdjustTargetPoint(e.GetPosition(this));
                        _inputTarget.RightDown(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (e.MouseButton == MouseButton.Left)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.LeftUp(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
                else if (e.MouseButton == MouseButton.Right)
                {
                    var zpoint = AdjustPanPoint(e.GetPosition(this));
                    Released(zpoint.X, zpoint.Y);

                    if (_zoomServiceState.IsPanning == false)
                    {
                        var tpoint = AdjustTargetPoint(e.GetPosition(this));
                        _inputTarget.RightUp(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                var zpoint = AdjustPanPoint(e.GetPosition(this));
                Moved(zpoint.X, zpoint.Y);

                if (_zoomServiceState.IsPanning == false)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.Move(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
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
            if (_zoomServiceState != null && _inputTarget != null)
            {
                _zoomServiceState.IsZooming = true;
                _zoomServiceState.AutoFitMode = FitMode.None;
                ZoomDeltaTo(delta, x, y);
                Invalidate(true);
            }
        }

        public void Pressed(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null && _zoomServiceState.IsPanning == false && IsCaptured?.Invoke() == false)
            {
                _zoomServiceState.IsPanning = true;
                _zoomServiceState.AutoFitMode = FitMode.None;
                Capture?.Invoke();
                StartPan(x, y);
                Invalidate(true);
            }
        }

        public void Released(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null && _zoomServiceState.IsPanning == true)
            {
                Release?.Invoke();
                Invalidate(true);
                _zoomServiceState.IsPanning = false;
            }
        }

        public void Moved(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null && _zoomServiceState.IsPanning == true)
            {
                PanTo(x, y);
                Invalidate(true);
            }
        }

        public void Invalidate(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                if (redraw)
                {
                    Redraw?.Invoke();
                }
            }
        }

        private void UpdateCurrentMatrix()
        {
            CurrentMatrix = new Matrix(
                _zoomServiceState.ZoomX,
                0,
                0,
                _zoomServiceState.ZoomY,
                _zoomServiceState.OffsetX,
                _zoomServiceState.OffsetY);
        }

        private void UpdateZoomServiceState()
        {
            _zoomServiceState.ZoomX = CurrentMatrix.M11;
            _zoomServiceState.ZoomY = CurrentMatrix.M22;
            _zoomServiceState.OffsetX = CurrentMatrix.M31;
            _zoomServiceState.OffsetY = CurrentMatrix.M32;
        }

        public void ZoomTo(double zoom, double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                UpdateCurrentMatrix();
                CurrentMatrix = new Matrix(zoom, 0, 0, zoom, x - (zoom * x), y - (zoom * y)) * CurrentMatrix;
                UpdateZoomServiceState();
            }
        }

        public void ZoomDeltaTo(double delta, double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                ZoomTo(delta > 0 ? _zoomServiceState.ZoomSpeed : 1 / _zoomServiceState.ZoomSpeed, x, y);
            }
        }

        public void StartPan(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                PanPosition = new Point(x, y);
            }
        }

        public void PanTo(double x, double y)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double dx = x - PanPosition.X;
                double dy = y - PanPosition.Y;
                Point delta = new Point(dx, dy);
                PanPosition = new Point(x, y);
                UpdateCurrentMatrix();
                CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, delta.X, delta.Y) * CurrentMatrix;
                UpdateZoomServiceState();
            }
        }

        public void Reset()
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
                UpdateZoomServiceState();
            }
        }

        public void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double ox = (panelWidth - elementWidth) / 2;
                double oy = (panelHeight - elementHeight) / 2;
                CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double ox = (panelWidth - elementWidth * zx) / 2;
                double oy = (panelHeight - elementHeight * zy) / 2;
                CurrentMatrix = new Matrix(zx, 0.0, 0.0, zy, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double zoom = Math.Min(zx, zy);
                double ox = (panelWidth - elementWidth * zoom) / 2;
                double oy = (panelHeight - elementHeight * zoom) / 2;
                CurrentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double zoom = Math.Max(zx, zy);
                double ox = (panelWidth - elementWidth * zoom) / 2;
                double oy = (panelHeight - elementHeight * zoom) / 2;
                CurrentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
                UpdateZoomServiceState();
            }
        }

        public void ResetZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Reset();
                Invalidate(redraw);
            }
        }

        public void CenterZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Center(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void FillZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Fill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void UniformZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                Uniform(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
        }

        public void UniformToFillZoom(bool redraw)
        {
            if (_zoomServiceState != null && _inputTarget != null)
            {
                UniformToFill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                Invalidate(redraw);
            }
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

            if (_inputTarget != null && _drawTarget != null)
            {
                // TODO: Do not inject dependencies.
                _drawTarget.InputService = this;
                _drawTarget.ZoomService = this;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (_inputTarget != null && _drawTarget != null)
            {
                // TODO: Do not inject dependencies.
                _drawTarget.InputService = null;
                _drawTarget.ZoomService = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            this.Focus();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_zoomServiceState != null && _inputTarget != null && _drawTarget != null)
            {
                bool initializeZoom =
                    double.IsNaN(_zoomServiceState.ZoomX)
                    || double.IsNaN(_zoomServiceState.ZoomY)
                    || double.IsNaN(_zoomServiceState.OffsetX)
                    || double.IsNaN(_zoomServiceState.OffsetY);

                if (initializeZoom == true)
                {
                    switch (_zoomServiceState.InitFitMode)
                    {
                        case FitMode.None:
                        case FitMode.Reset:
                            ResetZoom(false);
                            break;
                        case FitMode.Center:
                            CenterZoom(false);
                            break;
                        case FitMode.Fill:
                            FillZoom(false);
                            break;
                        case FitMode.Uniform:
                            UniformZoom(false);
                            break;
                        case FitMode.UniformToFill:
                            UniformToFillZoom(false);
                            break;
                    }
                }

                if (initializeZoom == false && _zoomServiceState.IsPanning == false && _zoomServiceState.IsZooming == false)
                {
                    switch (_zoomServiceState.AutoFitMode)
                    {
                        case FitMode.None:
                            break;
                        case FitMode.Reset:
                            ResetZoom(false);
                            break;
                        case FitMode.Center:
                            CenterZoom(false);
                            break;
                        case FitMode.Fill:
                            FillZoom(false);
                            break;
                        case FitMode.Uniform:
                            UniformZoom(false);
                            break;
                        case FitMode.UniformToFill:
                            UniformToFillZoom(false);
                            break;
                    }
                }

                GetOffset(out double dx, out double dy, out double zx, out double zy);

                _drawTarget.Draw(context, Bounds.Width, Bounds.Height, dx, dy, zx, zy);

                if (_zoomServiceState.IsZooming == true)
                {
                    _zoomServiceState.IsZooming = false;
                }
            }
        }
    }
}
