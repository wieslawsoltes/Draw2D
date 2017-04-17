// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public class CopyHelper
    {
        public static IEnumerable<PointShape> GetPoints(IEnumerable<ShapeObject> shapes)
        {
            foreach (var shape in shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        public static void Copy(IEnumerable<ShapeObject> shapes, IShapeContainer container, ISet<ShapeObject> selected)
        {
            var distinctPoints = GetPoints(shapes).Distinct();
            var distinctPointsCopy = new Dictionary<PointShape, PointShape>();

            foreach (var point in distinctPoints)
            {
                distinctPointsCopy[point] = point.Copy();
            }

            foreach (var shape in shapes)
            {
                var copy = Copy(shape, distinctPointsCopy);
                if (copy != null && !(copy is PointShape))
                {
                    copy.Select(selected);
                    container.Shapes.Add(copy);
                }
            }
        }

        public static ShapeObject Copy(ShapeObject shape, IDictionary<PointShape, PointShape> distinct)
        {
            switch (shape)
            {
                case PointShape point:
                    return Copy(point, distinct);
                case LineShape line:
                    return Copy(line, distinct);
                case CubicBezierShape cubic:
                    return Copy(cubic, distinct);
                case QuadraticBezierShape quadratic:
                    return Copy(quadratic, distinct);
                case FigureShape figure:
                    return Copy(figure, distinct);
                case PathShape path:
                    return Copy(path, distinct);
                case GroupShape group:
                    return Copy(group, distinct);
                case RectangleShape rectangle:
                    return Copy(rectangle, distinct);
                case EllipseShape ellipse:
                    return Copy(ellipse, distinct);
            }
            return null;
        }

        public static ShapeObject Copy(PointShape point, IDictionary<PointShape, PointShape> distinct)
        {
            return point.Copy();
        }

        public static ShapeObject Copy(LineShape line, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = line.Copy();
            copy.StartPoint = distinct[line.StartPoint];
            copy.Point = distinct[line.Point];
            foreach (var point in line.Points)
            {
                copy.Points.Add(distinct[point]);
            }
            return copy;
        }

        public static ShapeObject Copy(CubicBezierShape cubic, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = cubic.Copy();
            copy.StartPoint = distinct[cubic.StartPoint];
            copy.Point1 = distinct[cubic.Point1];
            copy.Point2 = distinct[cubic.Point2];
            copy.Point3 = distinct[cubic.Point3];
            foreach (var point in cubic.Points)
            {
                copy.Points.Add(distinct[point]);
            }
            return copy;
        }

        public static ShapeObject Copy(QuadraticBezierShape quadratic, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = quadratic.Copy();
            copy.StartPoint = distinct[quadratic.StartPoint];
            copy.Point1 = distinct[quadratic.Point1];
            copy.Point2 = distinct[quadratic.Point2];
            foreach (var point in quadratic.Points)
            {
                copy.Points.Add(distinct[point]);
            }
            return copy;
        }

        public static ShapeObject Copy(FigureShape figure, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = figure.Copy();
            foreach (var figureShape in figure.Shapes)
            {
                copy.Shapes.Add(Copy(figureShape, distinct));
            }
            return copy;
        }

        public static ShapeObject Copy(PathShape path, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = path.Copy();

            foreach (var figure in path.Figures)
            {
                var figureCopy = figure.Copy();
                foreach (var figureShape in figure.Shapes)
                {
                    figureCopy.Shapes.Add(Copy(figureShape, distinct));
                }
                copy.Figures.Add(figureCopy);
            }
            return copy;
        }

        public static ShapeObject Copy(GroupShape group, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = group.Copy();
            foreach (var point in group.Points)
            {
                copy.Points.Add(distinct[point]);
            }
            foreach (var groupShape in group.Shapes)
            {
                copy.Shapes.Add(Copy(groupShape, distinct));
            }
            return copy;
        }

        public static ShapeObject Copy(RectangleShape rectangle, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = rectangle.Copy();
            copy.TopLeft = distinct[rectangle.TopLeft];
            copy.BottomRight = distinct[rectangle.BottomRight];
            foreach (var point in rectangle.Points)
            {
                copy.Points.Add(distinct[point]);
            }
            return copy;
        }

        public static ShapeObject Copy(EllipseShape ellipse, IDictionary<PointShape, PointShape> distinct)
        {
            var copy = ellipse.Copy();
            copy.TopLeft = distinct[ellipse.TopLeft];
            copy.BottomRight = distinct[ellipse.BottomRight];
            foreach (var point in ellipse.Points)
            {
                copy.Points.Add(distinct[point]);
            }
            return copy;
        }
    }

    public partial class SelectionTool : IEdit
    {
        private IList<ShapeObject> _shapesToCopy = null;
        private ShapeObject _hover = null;
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

                    CopyHelper.Copy(_shapesToCopy, context.CurrentContainer, context.Renderer.Selected);

                    context.Invalidate();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Delete(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                var container = context.CurrentContainer;
                var paths = container.Shapes.OfType<PathShape>();
                var groups = container.Shapes.OfType<GroupShape>();
                var connectables = container.Shapes.OfType<ConnectableShape>();

                foreach (var shape in context.Renderer.Selected)
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
                            Delete(container, connectables, point);
                        }

                        if (paths.Count() > 0)
                        {
                            Delete(container, paths, shape);
                        }

                        if (groups.Count() > 0)
                        {
                            Delete(container, groups, shape);
                        }
                    }
                }

                this.DeHover(context);
                context.Renderer.Selected.Clear();

                context.Invalidate();

                this.CurrentState = State.None;
            }
        }

        private static bool Delete(IShapeContainer container, IEnumerable<ConnectableShape> connectables, PointShape point)
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

        private bool Delete(IShapeContainer container, IEnumerable<PathShape> paths, ShapeObject shape)
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

        private bool Delete(IShapeContainer container, IEnumerable<GroupShape> groups, ShapeObject shape)
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
                    //if (shape is PointShape point)
                    //{
                    //    group.Points.Add(point);
                    //}
                    //else
                    //{
                    //    group.Shapes.Add(shape);
                    //}
                }

                group.Select(context.Renderer.Selected);
                context.CurrentContainer.Shapes.Add(group);

                context.Invalidate();

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
                    shape.Select(context.Renderer.Selected);
                }

                context.Invalidate();

                this.CurrentState = State.None;
            }
        }

        public void Hover(IToolContext context, ShapeObject shape)
        {
            if (shape != null)
            {
                _hover = shape;
                _hover.Select(context.Selected);
            }
        }

        public void DeHover(IToolContext context)
        {
            if (_hover != null)
            {
                _hover.Deselect(context.Selected);
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
                            context.Selected.Remove(point);
                            context.Selected.Add(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, ShapeObject shape)
        {
            if (shape is ConnectableShape connectable)
            {
                connectable.Deselect(context.Selected);
                _disconnected = connectable.Disconnect();
                connectable.Select(context.Selected);
            }
        }
    }
}
