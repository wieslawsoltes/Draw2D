// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Containers;
using Draw2D.Shapes;
using Spatial;

namespace Draw2D.Editor.Tools
{
    public static class CopyHelper
    {
        public static IEnumerable<PointShape> GetPoints(IEnumerable<BaseShape> shapes)
        {
            foreach (var shape in shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public static IDictionary<object, object> GetPointsCopyDict(IEnumerable<BaseShape> shapes)
        {
            var copy = new Dictionary<object, object>();

            foreach (var point in GetPoints(shapes).Distinct())
            {
                copy[point] = point.Copy(null);
            }

            return copy;
        }

        public static void Copy(CanvasContainer container, IEnumerable<BaseShape> shapes, ISelection selection)
        {
            var shared = GetPointsCopyDict(shapes);

            foreach (var shape in shapes)
            {
                if (shape is ICopyable copyable)
                {
                    var copy = (BaseShape)copyable.Copy(shared);
                    if (copy != null && !(copy is PointShape))
                    {
                        copy.Select(selection);
                        container.Shapes.Add(copy);
                    }
                }
            }
        }
    }

    public static class DeleteHelper
    {
        public static void Delete(CanvasContainer container, ISelection selection)
        {
            var paths = container.Shapes.OfType<PathShape>();
            var groups = container.Shapes.OfType<GroupShape>();
            var connectables = container.Shapes.OfType<ConnectableShape>();

            foreach (var shape in selection.Selected)
            {
                if (container.Shapes.Contains(shape))
                {
                    container.Shapes.Remove(shape);
                }
                else if (container.Guides.Contains(shape))
                {
                    if (shape is LineShape guide)
                    {
                        container.Guides.Remove(guide);
                    }
                }
                else
                {
                    if (shape is PointShape point)
                    {
                        TryToDelete(container, connectables, point);
                    }

                    if (paths.Count() > 0)
                    {
                        TryToDelete(container, paths, shape);
                    }

                    if (groups.Count() > 0)
                    {
                        TryToDelete(container, groups, shape);
                    }
                }
            }
        }

        public static bool TryToDelete(CanvasContainer container, IEnumerable<ConnectableShape> connectables, PointShape point)
        {
            foreach (var connectable in connectables)
            {
                if (connectable.Points.Contains(point))
                {
                    connectable.Points.Remove(point);
                    connectable.MarkAsDirty(true);

                    return true;
                }
            }

            return false;
        }

        public static bool TryToDelete(CanvasContainer container, IEnumerable<PathShape> paths, BaseShape shape)
        {
            foreach (var path in paths)
            {
                foreach (var figure in path.Figures)
                {
                    if (figure.Shapes.Contains(shape))
                    {
                        figure.Shapes.Remove(shape);
                        figure.MarkAsDirty(true);

                        if (figure.Shapes.Count <= 0)
                        {
                            path.Figures.Remove(figure);
                            path.MarkAsDirty(true);

                            if (path.Figures.Count <= 0)
                            {
                                container.Shapes.Remove(path);
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryToDelete(CanvasContainer container, IEnumerable<GroupShape> groups, BaseShape shape)
        {
            foreach (var group in groups)
            {
                if (group.Shapes.Contains(shape))
                {
                    group.Shapes.Remove(shape);
                    group.MarkAsDirty(true);

                    if (group.Shapes.Count <= 0)
                    {
                        container.Shapes.Remove(group);
                    }

                    return true;
                }
            }

            return false;
        }
    }

    public partial class SelectionTool : IEdit
    {
        private IList<BaseShape> _shapesToCopy = null;
        private BaseShape _hover = null;
        private bool _disconnected = false;

        public void Cut(IToolContext context)
        {
            Copy(context);
            Delete(context);
        }

        public void Copy(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                _shapesToCopy = context.Renderer.Selected.ToList();
            }
        }

        public void Paste(IToolContext context)
        {
            if (_shapesToCopy != null)
            {
                lock (context.Renderer.Selected)
                {
                    this.DeHover(context);
                    context.Renderer.Selected.Clear();

                    CopyHelper.Copy(context.CurrentContainer, _shapesToCopy, context.Renderer);

                    context.Invalidate?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Delete(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                DeleteHelper.Delete(context.CurrentContainer, context.Renderer);

                this.DeHover(context);
                context.Renderer.Selected.Clear();

                context.Invalidate?.Invoke();

                this.CurrentState = State.None;
            }
        }

        public void Group(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                this.DeHover(context);

                var shapes = context.Renderer.Selected.ToList();

                Delete(context);

                var group = new GroupShape();

                foreach (var shape in shapes)
                {
                    if (!(shape is PointShape))
                    {
                        group.Shapes.Add(shape);
                    }
                }

                group.Select(context.Renderer);
                context.CurrentContainer.Shapes.Add(group);

                context.Invalidate?.Invoke();

                this.CurrentState = State.None;
            }
        }

        public void SelectAll(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                this.DeHover(context);
                context.Renderer.Selected.Clear();

                foreach (var shape in context.CurrentContainer.Shapes)
                {
                    shape.Select(context.Renderer);
                }

                context.Invalidate?.Invoke();

                this.CurrentState = State.None;
            }
        }

        public void Hover(IToolContext context, BaseShape shape)
        {
            context.Renderer.Hover = shape;

            if (shape != null)
            {
                _hover = shape;
                _hover.Select(context.Renderer);
            }
        }

        public void DeHover(IToolContext context)
        {
            context.Renderer.Hover = null;

            if (_hover != null)
            {
                _hover.Deselect(context.Renderer);
                _hover = null;
            }
        }

        public void Connect(IToolContext context, PointShape point)
        {
            var target = context.HitTest.TryToGetPoint(
                context.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                point);
            if (target != point)
            {
                foreach (var item in context.CurrentContainer.Shapes)
                {
                    if (item is ConnectableShape connectable)
                    {
                        if (connectable.Connect(point, target))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, PointShape point)
        {
            foreach (var shape in context.CurrentContainer.Shapes)
            {
                if (shape is ConnectableShape connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.Renderer.Selected.Remove(point);
                            context.Renderer.Selected.Add(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, BaseShape shape)
        {
            if (shape is ConnectableShape connectable)
            {
                connectable.Deselect(context.Renderer);
                _disconnected = connectable.Disconnect();
                connectable.Select(context.Renderer);
            }
        }
    }
}
