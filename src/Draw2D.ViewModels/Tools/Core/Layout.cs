// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    internal readonly struct Box
    {
        public readonly IBaseShape shape;
        public readonly IList<IPointShape> points;
        public readonly double ax;
        public readonly double ay;
        public readonly double bx;
        public readonly double by;
        public readonly double cx;
        public readonly double cy;
        public readonly double w;
        public readonly double h;

        public static int CompareHorizontalLeft(Box box1, Box box2)
        {
            return (box1.ax > box2.ax) ? 1 : ((box1.ax < box2.ax) ? -1 : 0);
        }

        public static int CompareHorizontalRight(Box box1, Box box2)
        {
            return (box1.bx > box2.bx) ? 1 : ((box1.bx < box2.bx) ? -1 : 0);
        }

        public static int CompareVerticalTop(Box box1, Box box2)
        {
            return (box1.ay > box2.ay) ? 1 : ((box1.ay < box2.ay) ? -1 : 0);
        }

        public static int CompareVerticalBottom(Box box1, Box box2)
        {
            return (box1.by > box2.by) ? 1 : ((box1.by < box2.by) ? -1 : 0);
        }

        public static int CompareWidth(Box box1, Box box2)
        {
            return (box1.w > box2.w) ? 1 : ((box1.w < box2.w) ? -1 : 0);
        }

        public static int CompareHeight(Box box1, Box box2)
        {
            return (box1.h > box2.h) ? 1 : ((box1.h < box2.h) ? -1 : 0);
        }

        public static void GetBox(IList<IPointShape> points, out double ax, out double ay, out double bx, out double by)
        {
            ax = double.MaxValue;
            ay = double.MaxValue;
            bx = double.MinValue;
            by = double.MinValue;

            foreach (var point in points)
            {
                ax = Math.Min(ax, point.X);
                ay = Math.Min(ay, point.Y);
                bx = Math.Max(bx, point.X);
                by = Math.Max(by, point.Y);
            }
        }

        public Box(IBaseShape shape)
        {
            this.shape = shape;
            this.points = new List<IPointShape>();
            this.shape.GetPoints(points);
            GetBox(this.points, out this.ax, out this.ay, out this.bx, out this.by);
            this.cx = (this.ax + this.bx) / 2.0;
            this.cy = (this.ay + this.by) / 2.0;
            this.w = Math.Abs(this.bx - this.ax);
            this.h = Math.Abs(this.by - this.ay);
        }
    }

    internal readonly struct Bounds
    {
        public readonly IList<Box> boxes;
        public readonly double ax;
        public readonly double ay;
        public readonly double bx;
        public readonly double by;
        public readonly double cx;
        public readonly double cy;
        public readonly double w;
        public readonly double h;

        public Bounds(IList<Box> boxes)
        {
            this.boxes = boxes;
            this.ax = double.MaxValue;
            this.ay = double.MaxValue;
            this.bx = double.MinValue;
            this.by = double.MinValue;
            foreach (var box in this.boxes)
            {
                this.ax = Math.Min(this.ax, box.ax);
                this.ay = Math.Min(this.ay, box.ay);
                this.bx = Math.Max(this.bx, box.bx);
                this.by = Math.Max(this.by, box.by);
            }
            this.cx = (this.ax + this.bx) / 2.0;
            this.cy = (this.ay + this.by) / 2.0;
            this.w = Math.Abs(this.bx - this.ax);
            this.h = Math.Abs(this.by - this.ay);
        }
    }

    internal enum StackMode
    {
        Horizontal,
        Vertical
    }

    internal enum DistributeMode
    {
        Horizontal,
        Vertical
    }

    internal enum AlignMode
    {
        Left,
        Centered,
        Right,
        Top,
        Center,
        Bottom
    }

    internal static class Layout
    {
        public static void Stack(IToolContext context, StackMode mode)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                var boxes = new List<Box>();

                foreach (var shape in shapes)
                {
                    if (!(shape is IPointShape))
                    {
                        boxes.Add(new Box(shape));
                    }
                }

                if (boxes.Count >= 2)
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    var bounds = new Bounds(boxes);

                    switch (mode)
                    {
                        case StackMode.Horizontal:
                            {
                                boxes.Sort(Box.CompareHorizontalLeft);
                                boxes[0].shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                double offset = boxes[0].ax + boxes[0].w;
                                for (int i = 1; i <= boxes.Count - 1; i++)
                                {
                                    var box = boxes[i];
                                    double dx = offset - box.ax;
                                    box.shape.Move(context.DocumentContainer.ContainerView.SelectionState, dx, 0.0);
                                    box.shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                    offset += box.w;
                                }
                            }
                            break;
                        case StackMode.Vertical:
                            {
                                boxes.Sort(Box.CompareVerticalTop);
                                boxes[0].shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                double offset = boxes[0].ay + boxes[0].h;
                                for (int i = 1; i <= boxes.Count - 1; i++)
                                {
                                    var box = boxes[i];
                                    double dy = offset - box.ay;
                                    box.shape.Move(context.DocumentContainer.ContainerView.SelectionState, 0.0, dy);
                                    box.shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                    offset += box.h;
                                }
                            }
                            break;
                    }

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
        }

        public static void Distribute(IToolContext context, DistributeMode mode)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                var boxes = new List<Box>();

                foreach (var shape in shapes)
                {
                    if (!(shape is IPointShape))
                    {
                        boxes.Add(new Box(shape));
                    }
                }

                if (boxes.Count > 2)
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    var bounds = new Bounds(boxes);

                    double sw = 0.0;
                    double sh = 0.0;

                    foreach (var box in boxes)
                    {
                        sw += box.w;
                        sh += box.h;
                    }

                    double gaph = (bounds.w - sw) / (boxes.Count - 1);
                    double gapv = (bounds.h - sh) / (boxes.Count - 1);

                    switch (mode)
                    {
                        case DistributeMode.Horizontal:
                            {
                                boxes.Sort(Box.CompareHorizontalLeft);
                                boxes[0].shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                double offset = boxes[0].ax + boxes[0].w + gaph;
                                for (int i = 1; i <= boxes.Count - 2; i++)
                                {
                                    var box = boxes[i];
                                    double dx = offset - box.ax;
                                    box.shape.Move(context.DocumentContainer.ContainerView.SelectionState, dx, 0.0);
                                    box.shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                    offset += box.w + gaph;
                                }
                                boxes[boxes.Count - 1].shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                            }
                            break;
                        case DistributeMode.Vertical:
                            {
                                boxes.Sort(Box.CompareVerticalTop);
                                boxes[0].shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                double offset = boxes[0].ay + boxes[0].h + gapv;
                                for (int i = 1; i <= boxes.Count - 2; i++)
                                {
                                    var box = boxes[i];
                                    double dy = offset - box.ay;
                                    box.shape.Move(context.DocumentContainer.ContainerView.SelectionState, 0.0, dy);
                                    box.shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                                    offset += box.h + gapv;
                                }
                                boxes[boxes.Count - 1].shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                            }
                            break;
                    }

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
        }

        public static void Align(IToolContext context, AlignMode mode)
        {
            if (context.DocumentContainer?.ContainerView?.SelectionState != null)
            {
                var shapes = new List<IBaseShape>(context.DocumentContainer.ContainerView.SelectionState?.Shapes);
                var boxes = new List<Box>();

                foreach (var shape in shapes)
                {
                    if (!(shape is IPointShape))
                    {
                        boxes.Add(new Box(shape));
                    }
                }

                if (boxes.Count > 1)
                {
                    context.DocumentContainer?.ContainerView?.SelectionState?.Dehover();
                    context.DocumentContainer?.ContainerView?.SelectionState?.Clear();

                    var bounds = new Bounds(boxes);

                    foreach (var box in boxes)
                    {
                        double dx = 0.0;
                        double dy = 0.0;

                        switch (mode)
                        {
                            case AlignMode.Left:
                                dx = bounds.ax - box.ax;
                                break;
                            case AlignMode.Centered:
                                dx = bounds.cx - ((box.ax + box.bx) / 2.0);
                                break;
                            case AlignMode.Right:
                                dx = bounds.bx - box.bx;
                                break;
                            case AlignMode.Top:
                                dy = bounds.ay - box.ay;
                                break;
                            case AlignMode.Center:
                                dy = bounds.cy - ((box.ay + box.by) / 2.0);
                                break;
                            case AlignMode.Bottom:
                                dy = bounds.by - box.by;
                                break;
                        }

                        if (dx != 0.0 || dy != 0.0)
                        {
                            box.shape.Move(context.DocumentContainer.ContainerView.SelectionState, dx, dy);
                        }
                        box.shape.Select(context.DocumentContainer.ContainerView.SelectionState);
                    }

                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
        }

        public static void SwapShape(ICanvasContainer container, IBaseShape shape, int insertIndex, int removeIndex)
        {
            if (container != null && shape != null && insertIndex >= 0 && removeIndex >= 0)
            {
                container.Shapes.Insert(insertIndex, shape);
                container.Shapes.RemoveAt(removeIndex);
                container.MarkAsDirty(true);
            }
        }

        public static void Swap(ICanvasContainer container, IBaseShape shape, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                SwapShape(container, shape, targetIndex + 1, sourceIndex);
            }
            else
            {
                if (container.Shapes.Count + 1 > sourceIndex + 1)
                {
                    SwapShape(container, shape, targetIndex, sourceIndex + 1);
                }
            }
        }

        public static void BringToFront(IToolContext context, IBaseShape source)
        {
            var container = context.DocumentContainer?.ContainerView?.CurrentContainer;
            if (container != null)
            {
                var shapes = container.Shapes;
                int sourceIndex = shapes.IndexOf(source);
                int targetIndex = shapes.Count - 1;
                if (targetIndex >= 0 && sourceIndex != targetIndex)
                {
                    Swap(container, source, sourceIndex, targetIndex);
                }
            }
        }

        public static void BringForward(IToolContext context, IBaseShape source)
        {
            var container = context.DocumentContainer?.ContainerView?.CurrentContainer;
            if (container != null)
            {
                var shapes = container.Shapes;
                int sourceIndex = shapes.IndexOf(source);
                int targetIndex = sourceIndex + 1;
                if (targetIndex < shapes.Count)
                {
                    Swap(container, source, sourceIndex, targetIndex);
                }
            }
        }

        public static void SendBackward(IToolContext context, IBaseShape source)
        {
            var container = context.DocumentContainer?.ContainerView?.CurrentContainer;
            if (container != null)
            {
                var shapes = container.Shapes;
                int sourceIndex = shapes.IndexOf(source);
                int targetIndex = sourceIndex - 1;
                if (targetIndex >= 0)
                {
                    Swap(container, source, sourceIndex, targetIndex);
                }
            }
        }

        public static void SendToBack(IToolContext context, IBaseShape source)
        {
            var container = context.DocumentContainer?.ContainerView?.CurrentContainer;
            if (container != null)
            {
                var shapes = container.Shapes;
                int sourceIndex = shapes.IndexOf(source);
                int targetIndex = 0;
                if (sourceIndex != targetIndex)
                {
                    Swap(container, source, sourceIndex, targetIndex);
                }
            }
        }
    }
}
