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
        private ZoomState _zoomState;

        public static readonly StyledProperty<bool> CustomDrawProperty =
            AvaloniaProperty.Register<AvaloniaInputView, bool>(nameof(CustomDraw), true);

        public bool CustomDraw
        {
            get { return GetValue(CustomDrawProperty); }
            set { SetValue(CustomDrawProperty, value); }
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
            dx = _zoomState.OffsetX;
            dy = _zoomState.OffsetY;
            zx = _zoomState.ZoomX;
            zy = _zoomState.ZoomY;
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
            _zoomState.Wheel(e.Delta.Y, zpoint.X, zpoint.Y);
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
                _zoomState.Pressed(zpoint.X, zpoint.Y);

                if (this.DataContext is IToolContext ctx && _zoomState.IsPanning == false)
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
                _zoomState.Released(zpoint.X, zpoint.Y);

                if (this.DataContext is IToolContext ctx && _zoomState.IsPanning == false)
                {
                    var tpoint = AdjustToolPoint(e.GetPosition(this));
                    ctx.CurrentTool.RightUp(ctx, tpoint.X, tpoint.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            Point zpoint = AdjustPanPoint(e.GetPosition(this));
            _zoomState.Moved(zpoint.X, zpoint.Y);

            if (this.DataContext is IToolContext ctx && _zoomState.IsPanning == false)
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

            if (ctx.CurrentContainer.WorkBackground != null)
            {
                var color = AvaloniaBrushCache.FromDrawColor(ctx.CurrentContainer.WorkBackground);
                var brush = new SolidColorBrush(color);
                context.FillRectangle(brush, new Rect(dx * zx, dy * zy, ctx.CurrentContainer.Width * zx, ctx.CurrentContainer.Height * zy));
            }

            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, dx, dy, zx, zy, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, dx, dy, zx, zy, DrawMode.Point, null, null);

            ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, dx, dy, zx, zy, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, dx, dy, zx, zy, DrawMode.Point, null, null);

            ctx.Presenter.DrawDecorators(context, ctx.CurrentContainer, ctx.Renderer, dx, dy, zx, zy, DrawMode.Shape);
            ctx.Presenter.DrawDecorators(context, ctx.WorkingContainer, ctx.Renderer, dx, dy, zx, zy, DrawMode.Shape);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is IToolContext ctx)
            {
                double width = Bounds.Width;
                double height = Bounds.Height;

                if (_zoomState == null)
                {
                    _zoomState = new ZoomState();

                    var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
                    if (md != null)
                    {
                        _zoomState.Capture = () =>
                        {
                            if (md.Captured == null)
                            {
                                md.Capture(this);
                            }
                        };
                        _zoomState.Release = () =>
                        {
                            if (md.Captured != null)
                            {
                                md.Capture(null);
                            }
                        };
                        _zoomState.IsCaptured = () =>
                        {
                            return md.Captured != null;
                        };
                        _zoomState.Redraw = () =>
                        {
                            this.InvalidateVisual();
                        };
                    }

                    _zoomState.Reset();
                    _zoomState.Center(width, height, ctx.CurrentContainer.Width, ctx.CurrentContainer.Height);
                    _zoomState.Invalidate(false);
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
