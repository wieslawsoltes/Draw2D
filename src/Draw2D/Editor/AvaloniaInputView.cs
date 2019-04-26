// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.MatrixExtensions;
using Avalonia.Media;
using Avalonia.VisualTree;
using Draw2D.ViewModels;

namespace Draw2D.Editor
{
    public class AvaloniaInputView : Border
    {
        private ZoomState _zoom = null;
        private bool _customDraw = true;

        public static readonly DirectProperty<AvaloniaInputView, ZoomState> ZoomProperty =
           AvaloniaProperty.RegisterDirect<AvaloniaInputView, ZoomState>(nameof(Zoom), o => o.Zoom, (o, v) => o.Zoom = v);

        public static readonly DirectProperty<AvaloniaInputView, bool> CustomDrawProperty =
           AvaloniaProperty.RegisterDirect<AvaloniaInputView, bool>(nameof(CustomDraw), o => o.CustomDraw, (o, v) => o.CustomDraw = v);

        public ZoomState Zoom
        {
            get { return _zoom; }
            set { SetAndRaise(ZoomProperty, ref _zoom, value); }
        }

        public bool CustomDraw
        {
            get { return _customDraw; }
            set { SetAndRaise(CustomDrawProperty, ref _customDraw, value); }
        }

        public AvaloniaInputView()
        {
            PointerWheelChanged += (sender, e) => HandlePointerWheelChanged(e);
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is IToolContext ctx)
            {
                var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
                if (md != null)
                {
                    ctx.Capture = () =>
                    {
                        if (md.Captured == null)
                        {
                            md.Capture(this);
                        }
                    };
                    ctx.Release = () =>
                    {
                        if (md.Captured != null)
                        {
                            md.Capture(null);
                        }
                    };
                    ctx.Invalidate = () =>
                    {
                        this.InvalidateVisual();
                    };
                }
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is IToolContext ctx)
            {
                ctx.Capture = null;
                ctx.Release = null;
                ctx.Invalidate = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            this.Focus();
            this.InvalidateVisual();
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            this.InvalidateVisual();
        }

        private Modifier GetModifier(InputModifiers inputModifiers)
        {
            Modifier modifier = Modifier.None;

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
            dx = _zoom.OffsetX;
            dy = _zoom.OffsetY;
            zx = _zoom.ZoomX;
            zy = _zoom.ZoomY;
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

        private Point AdjustToolPoint(Point point)
        {
            GetOffset(out double dx, out double dy, out double zx, out double zy);
            return new Point((point.X - dx) / zx, (point.Y - dy) / zy);
        }

        private void HandlePointerWheelChanged(PointerWheelEventArgs e)
        {
            Point zpoint = AdjustZoomPoint(e.GetPosition(this));
            _zoom.Wheel(e.Delta.Y, zpoint.X, zpoint.Y);
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var tpoint = AdjustToolPoint(e.GetPosition(this));
                    ctx.CurrentTool.LeftDown(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                Point zpoint = AdjustPanPoint(e.GetPosition(this));
                _zoom.Pressed(zpoint.X, zpoint.Y);

                if (this.DataContext is IToolContext ctx && _zoom.IsPanning == false)
                {
                    var tpoint = AdjustToolPoint(e.GetPosition(this));
                    ctx.CurrentTool.RightDown(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var tpoint = AdjustToolPoint(e.GetPosition(this));
                    if (ctx.Mode == EditMode.Mouse)
                    {
                        ctx.CurrentTool.LeftUp(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                    }
                    else if (ctx.Mode == EditMode.Touch)
                    {
                        ctx.CurrentTool.LeftDown(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                Point zpoint = AdjustPanPoint(e.GetPosition(this));
                _zoom.Released(zpoint.X, zpoint.Y);

                if (this.DataContext is IToolContext ctx && _zoom.IsPanning == false)
                {
                    var tpoint = AdjustToolPoint(e.GetPosition(this));
                    ctx.CurrentTool.RightUp(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            Point zpoint = AdjustPanPoint(e.GetPosition(this));
            _zoom.Moved(zpoint.X, zpoint.Y);

            if (this.DataContext is IToolContext ctx && _zoom.IsPanning == false)
            {
                var tpoint = AdjustToolPoint(e.GetPosition(this));
                ctx.CurrentTool.Move(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
            }
        }

        private void Draw(DrawingContext context, IToolContext ctx, double width, double height, double dx, double dy, double zx, double zy)
        {
            if (ctx.CurrentContainer.InputBackground != null)
            {
                var color = AvaloniaBrushCache.FromDrawColor(ctx.CurrentContainer.InputBackground);
                var brush = new SolidColorBrush(color);
                context.FillRectangle(brush, new Rect(0, 0, width, height));
            }

            var state = context.PushPreTransform(new Matrix(zx, 0.0, 0.0, zy, dx, dy));

            if (ctx.CurrentContainer.WorkBackground != null)
            {
                var color = AvaloniaBrushCache.FromDrawColor(ctx.CurrentContainer.WorkBackground);
                var brush = new SolidColorBrush(color);
                context.FillRectangle(brush, new Rect(0.0, 0.0, ctx.CurrentContainer.Width, ctx.CurrentContainer.Height));
            }

            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Point, null, null);

            ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Point, null, null);

            ctx.Presenter.DrawDecorators(context, ctx.CurrentContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape);
            ctx.Presenter.DrawDecorators(context, ctx.WorkingContainer, ctx.Renderer, 0.0, 0.0, DrawMode.Shape);

            state.Dispose();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is IToolContext ctx)
            {
                double width = Bounds.Width;
                double height = Bounds.Height;

                if (_zoom == null)
                {
                    _zoom = new ZoomState();

                    var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
                    if (md != null)
                    {
                        _zoom.Capture = () =>
                        {
                            if (md.Captured == null)
                            {
                                md.Capture(this);
                            }
                        };
                        _zoom.Release = () =>
                        {
                            if (md.Captured != null)
                            {
                                md.Capture(null);
                            }
                        };
                        _zoom.IsCaptured = () =>
                        {
                            return md.Captured != null;
                        };
                        _zoom.Redraw = () =>
                        {
                            this.InvalidateVisual();
                        };
                    }

                    _zoom.Reset();
                    _zoom.Center(width, height, ctx.CurrentContainer.Width, ctx.CurrentContainer.Height);
                    _zoom.Invalidate(false);
                }

                GetOffset(out double dx, out double dy, out double zx, out double zy);

                if (CustomDraw)
                {
                    context.Custom(new CustomDrawOperation(ctx, width, height, dx, dy, zx, zy));
                }
                else
                {
                    Draw(context, ctx, width, height, dx, dy, zx, zy);
                }
            }
        }
    }
}
