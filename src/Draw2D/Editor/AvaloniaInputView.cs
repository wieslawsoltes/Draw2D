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

        public void Wheel(PointerWheelEventArgs e, IControl relativeTo)
        {
            Point point = e.GetPosition(relativeTo);
            ZoomDeltaTo(e.Delta.Y, point.X, point.Y);
            Invalidate(relativeTo, true);
        }

        public void Pressed(PointerPressedEventArgs e, IControl relativeTo)
        {
            if (e.Device.Captured == null && IsPanning == false)
            {
                IsPanning = true;
                var point = e.GetPosition(relativeTo);
                e.Device.Capture(relativeTo);
                StartPan(point.X, point.Y);
                Invalidate(relativeTo, true);
            }
        }

        public void Released(PointerReleasedEventArgs e, IControl relativeTo)
        {
            if (IsPanning == true)
            {
                e.Device.Capture(null);
                Invalidate(relativeTo, true);
                IsPanning = false;
            }
        }

        public void Moved(PointerEventArgs e, IVisual relativeTo)
        {
            if (IsPanning == true)
            {
                var point = e.GetPosition(relativeTo);
                PanTo(point.X, point.Y);
                Invalidate(relativeTo, true);
            }
        }

        public void Invalidate(IVisual relativeTo, bool redraw)
        {
            ZoomX = CurrentMatrix.M11;
            ZoomY = CurrentMatrix.M22;
            OffsetX = CurrentMatrix.M31;
            OffsetY = CurrentMatrix.M32;
            Debug.WriteLine($"OffsetX: {OffsetX} OffsetY: {OffsetY}");
            if (redraw)
            {
                relativeTo.InvalidateVisual();
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
            Debug.WriteLine($"[StartPan] PreviousPosition: {PanPosition}");
        }

        public void PanTo(double x, double y)
        {
            double dx = x - PanPosition.X;
            double dy = y - PanPosition.Y;
            Point delta = new Point(dx, dy);
            PanPosition = new Point(x, y);
            CurrentMatrix = MatrixHelper.TranslatePrepend(CurrentMatrix, delta.X, delta.Y);
            Debug.WriteLine($"[PanTo] PreviousPosition: {PanPosition} delta: {delta}");
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

    public class AvaloniaInputView : Border
    {
        private bool _drawWorking = false;
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
            _drawWorking = true;
            this.InvalidateVisual();
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            _drawWorking = false;
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

        private void GetOffset(out double ox, out double oy)
        {
            ox = _zoomState.OffsetX;
            oy = _zoomState.OffsetY;
        }

        private void HandlePointerWheelChanged(PointerWheelEventArgs e)
        {
            _zoomState.Wheel(e, this);
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var point = e.GetPosition(this);
                    GetOffset(out double ox, out double oy);
                    ctx.CurrentTool.LeftDown(ctx, point.X - ox, point.Y - oy, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                _zoomState.Pressed(e, this);

                if (this.DataContext is IToolContext ctx && _zoomState.IsPanning == false)
                {
                    var point = e.GetPosition(this);
                    GetOffset(out double ox, out double oy);
                    ctx.CurrentTool.RightDown(ctx, point.X - ox, point.Y - oy, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is IToolContext ctx)
                {
                    var point = e.GetPosition(this);
                    GetOffset(out double ox, out double oy);
                    if (ctx.Mode == EditMode.Mouse)
                    {
                        ctx.CurrentTool.LeftUp(ctx, point.X - ox, point.Y - oy, GetModifier(e.InputModifiers));
                    }
                    else if (ctx.Mode == EditMode.Touch)
                    {
                        ctx.CurrentTool.LeftDown(ctx, point.X - ox, point.Y - oy, GetModifier(e.InputModifiers));
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                _zoomState.Released(e, this);

                if (this.DataContext is IToolContext ctx && _zoomState.IsPanning == false)
                {
                    var point = e.GetPosition(this);
                    GetOffset(out double ox, out double oy);
                    ctx.CurrentTool.RightUp(ctx, point.X - ox, point.Y - oy, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            _zoomState.Moved(e, this);

            if (this.DataContext is IToolContext ctx && _zoomState.IsPanning == false)
            {
                var point = e.GetPosition(this);
                GetOffset(out double ox, out double oy);
                ctx.CurrentTool.Move(ctx, point.X - ox, point.Y - oy, GetModifier(e.InputModifiers));
            }
        }

        private void Draw(DrawingContext context, IToolContext ctx, bool drawWorking, double width, double height, double ox, double oy)
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
                context.FillRectangle(brush, new Rect(ox, oy, ctx.CurrentContainer.Width, ctx.CurrentContainer.Height));
            }

            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, ox, oy, DrawMode.Shape, null, null);
            ctx.Presenter.DrawContainer(context, ctx.CurrentContainer, ctx.Renderer, ox, oy, DrawMode.Point, null, null);

            if (drawWorking)
            {
                ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, ox, oy, DrawMode.Shape, null, null);
                ctx.Presenter.DrawContainer(context, ctx.WorkingContainer, ctx.Renderer, ox, oy, DrawMode.Point, null, null);
            }

            ctx.Presenter.DrawDecorators(context, ctx.CurrentContainer, ctx.Renderer, ox, oy, DrawMode.Shape);

            if (drawWorking)
            {
                ctx.Presenter.DrawDecorators(context, ctx.WorkingContainer, ctx.Renderer, ox, oy, DrawMode.Shape);
            }
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
                    _zoomState.Reset();
                    _zoomState.Center(width, height, ctx.CurrentContainer.Width, ctx.CurrentContainer.Height);
                    _zoomState.Invalidate(this, false);
                }

                GetOffset(out double ox, out double oy);

                if (CustomDraw)
                {
                    context.Custom(new CustomDrawOperation(ctx, _drawWorking, width, height, ox, oy));
                }
                else
                {
                    Draw(context, ctx, _drawWorking, width, height, ox, oy);
                }
            }
        }
    }
}
