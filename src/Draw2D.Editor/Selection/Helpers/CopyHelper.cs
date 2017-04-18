// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Selection.Helpers
{
    public static class CopyHelper
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

        public static IDictionary<PointShape, PointShape> CopyPoints(IEnumerable<ShapeObject> shapes)
        {
            var copy = new Dictionary<PointShape, PointShape>();

            foreach (var point in GetPoints(shapes).Distinct())
            {
                copy[point] = point.Copy();
            }

            return copy;
        }

        public static void Copy(IEnumerable<ShapeObject> shapes, IShapeContainer container, ISet<ShapeObject> selected)
        {
            var distinct = CopyPoints(shapes);

            foreach (var shape in shapes)
            {
                var copy = Copy(shape, distinct);
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
}
