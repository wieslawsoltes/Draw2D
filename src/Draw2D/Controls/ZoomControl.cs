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
using Draw2D.ViewModels;

namespace Draw2D.Controls
{
    internal struct CustomDrawOperation : ICustomDrawOperation
    {
        private readonly IDrawTarget _drawTarget;
        private readonly double _width;
        private readonly double _height;
        private readonly double _dx;
        private readonly double _dy;
        private readonly double _zx;
        private readonly double _zy;

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
        private IZoomServiceState _zoomServiceState;
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
            if (_zoomServiceState != null)
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
    
                    if (_inputTarget != null && _zoomServiceState.IsPanning == false)
                    {
                        var tpoint = AdjustTargetPoint(e.GetPosition(this));
                        _inputTarget.RightDown(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (_zoomServiceState != null)
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
    
                    if (_inputTarget != null && _zoomServiceState.IsPanning == false)
                    {
                        var tpoint = AdjustTargetPoint(e.GetPosition(this));
                        _inputTarget.RightUp(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (_zoomServiceState != null)
            {
                var zpoint = AdjustPanPoint(e.GetPosition(this));
                Moved(zpoint.X, zpoint.Y);
    
                if (_inputTarget != null && _zoomServiceState.IsPanning == false)
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
            ZoomDeltaTo(delta, x, y);
            Invalidate(true);
        }

        public void Pressed(double x, double y)
        {
            if (IsCaptured?.Invoke() == false && _zoomServiceState != null && _zoomServiceState.IsPanning == false)
            {
                _zoomServiceState.IsPanning = true;
                Capture?.Invoke();
                StartPan(x, y);
                Invalidate(true);
            }
        }

        public void Released(double x, double y)
        {
            if (_zoomServiceState != null && _zoomServiceState.IsPanning == true)
            {
                Release?.Invoke();
                Invalidate(true);
                _zoomServiceState.IsPanning = false;
            }
        }

        public void Moved(double x, double y)
        {
            if (_zoomServiceState != null && _zoomServiceState.IsPanning == true)
            {
                PanTo(x, y);
                Invalidate(true);
            }
        }

        public void Invalidate(bool redraw)
        {
            if (_zoomServiceState != null)
            {
                _zoomServiceState.ZoomX = CurrentMatrix.M11;
                _zoomServiceState.ZoomY = CurrentMatrix.M22;
                _zoomServiceState.OffsetX = CurrentMatrix.M31;
                _zoomServiceState.OffsetY = CurrentMatrix.M32;
                if (redraw)
                {
                    Redraw?.Invoke();
                }
            }
        }

        public void ZoomTo(double zoom, double x, double y)
        {
            if (_zoomServiceState != null)
            {
                CurrentMatrix = new Matrix(zoom, 0, 0, zoom, x - (zoom * x), y - (zoom * y)) * CurrentMatrix;
            }
        }

        public void ZoomDeltaTo(double delta, double x, double y)
        {
            if (_zoomServiceState != null)
            {
                ZoomTo(delta > 0 ? _zoomServiceState.ZoomSpeed : 1 / _zoomServiceState.ZoomSpeed, x, y);
            }
        }

        public void StartPan(double x, double y)
        {
            if (_zoomServiceState != null)
            {
                PanPosition = new Point(x, y);
            }
        }

        public void PanTo(double x, double y)
        {
            if (_zoomServiceState != null)
            {
                double dx = x - PanPosition.X;
                double dy = y - PanPosition.Y;
                Point delta = new Point(dx, dy);
                PanPosition = new Point(x, y);
                CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, delta.X, delta.Y) * CurrentMatrix;
            }
        }

        public void Reset()
        {
            if (_zoomServiceState != null)
            {
                CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
            }
        }

        public void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null)
            {
                double ox = (panelWidth - elementWidth) / 2;
                double oy = (panelHeight - elementHeight) / 2;
                CurrentMatrix = new Matrix(1.0, 0.0, 0.0, 1.0, ox, oy);
            }
        }

        public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double ox = (panelWidth - elementWidth * zx) / 2;
                double oy = (panelHeight - elementHeight * zy) / 2;
                CurrentMatrix = new Matrix(zx, 0.0, 0.0, zy, ox, oy);
            }
        }

        public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double zoom = Math.Min(zx, zy);
                double ox = (panelWidth - elementWidth * zoom) / 2;
                double oy = (panelHeight - elementHeight * zoom) / 2;
                CurrentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
            }
        }

        public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight)
        {
            if (_zoomServiceState != null)
            {
                double zx = panelWidth / elementWidth;
                double zy = panelHeight / elementHeight;
                double zoom = Math.Max(zx, zy);
                double ox = (panelWidth - elementWidth * zoom) / 2;
                double oy = (panelHeight - elementHeight * zoom) / 2;
                CurrentMatrix = new Matrix(zoom, 0.0, 0.0, zoom, ox, oy);
            }
        }

        public void ResetZoom(bool redraw)
        {
            if (_zoomServiceState != null)
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

            if (_inputTarget != null)
            {
                // TODO: Do not inject dependencies.
                _drawTarget.InputService = this;
                _drawTarget.ZoomService = this;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (_inputTarget != null)
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

            if (_zoomServiceState != null && _drawTarget != null)
            {
                bool isValidZoom = 
                    _zoomServiceState.ZoomX != double.NaN 
                    && _zoomServiceState.ZoomY != double.NaN 
                    && _zoomServiceState.OffsetX != double.NaN 
                    && _zoomServiceState.OffsetY != double.NaN;

                if (isValidZoom == false)
                {
                    switch (_zoomServiceState.InitialAutoFitMode)
                    {
                        case AutoFitMode.None:
                        case AutoFitMode.Reset:
                            ResetZoom(false);
                            break;
                        case AutoFitMode.Center:
                            CenterZoom(false);
                            break;
                        case AutoFitMode.Fill:
                            FillZoom(false);
                            break;
                        case AutoFitMode.Uniform:
                            UniformZoom(false);
                            break;
                        case AutoFitMode.UniformToFill:
                            UniformToFillZoom(false);
                            break;
                    }
                }
                else
                {
                    switch (_zoomServiceState.AutoFitMode)
                    {
                        case AutoFitMode.None:
                            break;
                        case AutoFitMode.Reset:
                            ResetZoom(false);
                            break;
                        case AutoFitMode.Center:
                            CenterZoom(false);
                            break;
                        case AutoFitMode.Fill:
                            FillZoom(false);
                            break;
                        case AutoFitMode.Uniform:
                            UniformZoom(false);
                            break;
                        case AutoFitMode.UniformToFill:
                            UniformToFillZoom(false);
                            break;
                    }
                }

                GetOffset(out double dx, out double dy, out double zx, out double zy);

                if (_zoomServiceState.CustomDraw)
                {
                    context.Custom(new CustomDrawOperation(_drawTarget, Bounds.Width, Bounds.Height, dx, dy, zx, zy));
                }
                else
                {
                    _drawTarget.Draw(context, Bounds.Width, Bounds.Height, dx, dy, zx, zy);
                }
            }
        }
    }
}
