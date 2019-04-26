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

namespace Draw2D.Editor
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

    public class InputService : Border, IInputService
    {
        private bool _initializedZoom = false;
        private IZoomService _zoomService = new ZoomService();
        private bool _customDraw = true;
        private IInputTarget _inputTarget = null;
        private IDrawTarget _drawTarget = null;

        public static readonly DirectProperty<InputService, IZoomService> ZoomProperty =
           AvaloniaProperty.RegisterDirect<InputService, IZoomService>(nameof(ZoomService), o => o.ZoomService, (o, v) => o.ZoomService = v);

        public static readonly DirectProperty<InputService, bool> CustomDrawProperty =
           AvaloniaProperty.RegisterDirect<InputService, bool>(nameof(CustomDraw), o => o.CustomDraw, (o, v) => o.CustomDraw = v);

        public static readonly DirectProperty<InputService, IInputTarget> InputTargetProperty =
           AvaloniaProperty.RegisterDirect<InputService, IInputTarget>(nameof(InputTarget), o => o.InputTarget, (o, v) => o.InputTarget = v);

        public static readonly DirectProperty<InputService, IDrawTarget> DrawTargetProperty =
           AvaloniaProperty.RegisterDirect<InputService, IDrawTarget>(nameof(DrawTarget), o => o.DrawTarget, (o, v) => o.DrawTarget = v);

        public IZoomService ZoomService
        {
            get { return _zoomService; }
            set { SetAndRaise(ZoomProperty, ref _zoomService, value); }
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

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Func<bool> IsCaptured { get; set; }

        public Action Redraw { get; set; }

        public InputService()
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

            if (_zoomService != null)
            {
                // FIXME:
                _zoomService.InputService = this;
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

            if (_zoomService != null)
            {
                // FIXME:
                _zoomService.InputService = null;
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

        private void GetOffset(out double dx, out double dy, out double zx, out double zy)
        {
            dx = _zoomService.OffsetX;
            dy = _zoomService.OffsetY;
            zx = _zoomService.ZoomX;
            zy = _zoomService.ZoomY;
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
            _zoomService.Wheel(e.Delta.Y, zpoint.X, zpoint.Y);
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
                _zoomService.Pressed(zpoint.X, zpoint.Y);

                if (_inputTarget != null && _zoomService.IsPanning == false)
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
                _zoomService.Released(zpoint.X, zpoint.Y);

                if (_inputTarget != null && _zoomService.IsPanning == false)
                {
                    var tpoint = AdjustTargetPoint(e.GetPosition(this));
                    _inputTarget.RightUp(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            var zpoint = AdjustPanPoint(e.GetPosition(this));
            _zoomService.Moved(zpoint.X, zpoint.Y);

            if (_inputTarget != null && _zoomService.IsPanning == false)
            {
                var tpoint = AdjustTargetPoint(e.GetPosition(this));
                _inputTarget.Move(tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
            }
        }

        public void ResetZoom(bool redraw)
        {
            _zoomService.Reset();
            _zoomService.Invalidate(redraw);
        }

        public void CenterZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                _zoomService.Center(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                _zoomService.Invalidate(redraw);
            }
        }

        public void FillZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                _zoomService.Fill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                _zoomService.Invalidate(redraw);
            }
        }

        public void UniformZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                _zoomService.Uniform(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                _zoomService.Invalidate(redraw);
            }
        }

        public void UniformToFillZoom(bool redraw)
        {
            if (_inputTarget != null)
            {
                _zoomService.UniformToFill(Bounds.Width, Bounds.Height, _inputTarget.GetWidth(), _inputTarget.GetHeight());
                _zoomService.Invalidate(redraw);
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
