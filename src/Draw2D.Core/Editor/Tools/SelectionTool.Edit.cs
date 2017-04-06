// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor.Tools
{
    public partial class SelectionTool : IEdit
    {
        private IList<ShapeObject> _shapesToCopy = null;

        private IEnumerable<PointShape> GetPoints(IEnumerable<ShapeObject> shapes)
        {
            foreach (var shape in shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
            }
        }

        private ShapeObject CopyShape(ShapeObject shape, IDictionary<PointShape, PointShape> distinctPointsCopy)
        {
            switch (shape)
            {
                case PointShape point:
                    {
                        return point.Copy();
                    }
                case LineShape line:
                    {
                        var copy = line.Copy();
                        copy.StartPoint = distinctPointsCopy[line.StartPoint];
                        copy.Point = distinctPointsCopy[line.Point];
                        foreach (var point in line.Points)
                        {
                            copy.Points.Add(distinctPointsCopy[point]);
                        }
                        return copy;
                    }
                case CubicBezierShape cubic:
                    {
                        // TODO: 
                    }
                    break;
                case QuadraticBezierShape quadratic:
                    {
                        // TODO: 
                    }
                    break;
                case FigureShape figure:
                    {
                        // TODO: 
                    }
                    break;
                case PathShape path:
                    {
                        // TODO: 
                    }
                    break;
                case GroupShape group:
                    {
                        // TODO: 
                    }
                    break;
                case ScribbleShape scribble:
                    {
                        // TODO: 
                    }
                    break;
                case RectangleShape rectangle:
                    {
                        var copy = rectangle.Copy();
                        copy.TopLeft = distinctPointsCopy[rectangle.TopLeft];
                        copy.BottomRight = distinctPointsCopy[rectangle.BottomRight];
                        return copy;
                    }
                case EllipseShape ellipse:
                    {
                        var copy = ellipse.Copy();
                        copy.TopLeft = distinctPointsCopy[ellipse.TopLeft];
                        copy.BottomRight = distinctPointsCopy[ellipse.BottomRight];
                        return copy;
                    }
            }

            return null;
        }

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
                    context.Renderer.Selected.Clear();

                    var distinctPoints = GetPoints(_shapesToCopy).Distinct();
                    var distinctPointsCopy = new Dictionary<PointShape, PointShape>();

                    foreach (var point in distinctPoints)
                    {
                        distinctPointsCopy[point] = point.Copy();
                    }

                    foreach (var shape in _shapesToCopy)
                    {
                        var copy = CopyShape(shape, distinctPointsCopy);
                        if (!(copy is PointShape))
                        {
                            copy.Select(context.Renderer.Selected);
                            context.CurrentContainer.Shapes.Add(copy);
                        }
                    }

                    context.Invalidate();

                    this.HaveSelection = true;
                    this.CurrentState = SelectionTool.State.None;
                }
            }
        }

        public void Delete(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
                foreach (var shape in context.Renderer.Selected)
                {
                    if (context.CurrentContainer.Shapes.Contains(shape))
                    {
                        context.CurrentContainer.Shapes.Remove(shape);
                    }
                }
                context.Renderer.Selected.Clear();
            }
        }

        public void Group(IToolContext context)
        {
            lock (context.Renderer.Selected)
            {
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

                this.HaveSelection = true;
                this.CurrentState = State.None;
            }
        }
    }
}
