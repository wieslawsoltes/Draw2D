// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class ConicBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            if (conic.StartPoint.Bounds?.TryToGetPoint(conic.StartPoint, target, radius, hitTest) != null)
            {
                return conic.StartPoint;
            }

            if (conic.Point1.Bounds?.TryToGetPoint(conic.Point1, target, radius, hitTest) != null)
            {
                return conic.Point1;
            }

            if (conic.Point2.Bounds?.TryToGetPoint(conic.Point2, target, radius, hitTest) != null)
            {
                return conic.Point2;
            }

            foreach (var point in conic.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            conic.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            conic.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class ContainerBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerPoint in container.Points)
            {
                if (containerPoint.Bounds?.TryToGetPoint(containerPoint, target, radius, hitTest) != null)
                {
                    return containerPoint;
                }
            }
#if USE_CONTAINER_SHAPES
            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.TryToGetPoint(containerShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }
#endif
            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.Contains(containerShape, target, radius, hitTest);
                if (result != null)
                {
                    return container;
                }
            }
            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CanvasContainer container))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var containerShape in container.Shapes)
            {
                var result = containerShape.Bounds?.Overlaps(containerShape, target, radius, hitTest);
                if (result != null)
                {
                    return container;
                }
            }
            return null;
        }
    }

    [DataContract(IsReference = true)]
    public class CubicBezierBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            if (cubicBezier.StartPoint.Bounds?.TryToGetPoint(cubicBezier.StartPoint, target, radius, hitTest) != null)
            {
                return cubicBezier.StartPoint;
            }

            if (cubicBezier.Point1.Bounds?.TryToGetPoint(cubicBezier.Point1, target, radius, hitTest) != null)
            {
                return cubicBezier.Point1;
            }

            if (cubicBezier.Point2.Bounds?.TryToGetPoint(cubicBezier.Point2, target, radius, hitTest) != null)
            {
                return cubicBezier.Point2;
            }

            if (cubicBezier.Point3.Bounds?.TryToGetPoint(cubicBezier.Point3, target, radius, hitTest) != null)
            {
                return cubicBezier.Point3;
            }

            foreach (var point in cubicBezier.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            cubicBezier.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            cubicBezier.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            if (box.TopLeft.Bounds?.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (box.BottomRight.Bounds?.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class FigureBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is FigureShape figure))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figureShape in figure.Shapes)
            {
                var result = figureShape.Bounds?.TryToGetPoint(figureShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is FigureShape figure))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figureShape in figure.Shapes)
            {
                var result = figureShape.Bounds?.Contains(figureShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is FigureShape figure))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figureShape in figure.Shapes)
            {
                var result = figureShape.Bounds?.Overlaps(figureShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }

    [DataContract(IsReference = true)]
    public class GroupBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupPoint in group.Points)
            {
                if (groupPoint.Bounds?.TryToGetPoint(groupPoint, target, radius, hitTest) != null)
                {
                    return groupPoint;
                }
            }
#if USE_GROUP_SHAPES
            foreach (var groupShape in group.Shapes)
            {
                var result = groupShape.Bounds?.TryToGetPoint(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }
#endif
            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupShape in group.Shapes)
            {
                var result = groupShape.Bounds?.Contains(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupShape in group.Shapes)
            {
                var result = groupShape.Bounds?.Overlaps(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }
    }

    [DataContract(IsReference = true)]
    public class LineBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            if (line.StartPoint.Bounds?.TryToGetPoint(line.StartPoint, target, radius, hitTest) != null)
            {
                return line.StartPoint;
            }

            if (line.Point.Bounds?.TryToGetPoint(line.Point, target, radius, hitTest) != null)
            {
                return line.Point;
            }

            foreach (var point in line.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            var nearest = target.NearestOnLine(a, b);
            double distance = target.DistanceTo(nearest);
            return distance < radius ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            return Line2.LineIntersectsWithRect(a, b, target, out _, out _, out _, out _) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class PathBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var pathPoint in path.Points)
            {
                if (pathPoint.Bounds?.TryToGetPoint(pathPoint, target, radius, hitTest) != null)
                {
                    return pathPoint;
                }
            }

            foreach (var pathShape in path.Shapes)
            {
                var result = pathShape.Bounds?.TryToGetPoint(pathShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var pathShape in path.Shapes)
            {
                var result = pathShape.Bounds?.Contains(pathShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }
#if USE_PATH_FIGURES
            if (path.Shapes.Count > 1)
            {
                foreach (var pathShape in path.Shapes)
                {
                    var pathShapePoints = new List<IPointShape>();
                    pathShape.GetPoints(pathShapePoints);

                    if (HitTestHelper.Contains(pathShapePoints, target))
                    {
                        return pathShape;
                    }
                }
            }
#endif
            var points = new List<IPointShape>();
            path.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var pathShape in path.Shapes)
            {
                var result = pathShape.Bounds?.Overlaps(pathShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }

            var points = new List<IPointShape>();
            path.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class PointBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is IPointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is IPointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is IPointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            if (quadraticBezier.StartPoint.Bounds?.TryToGetPoint(quadraticBezier.StartPoint, target, radius, hitTest) != null)
            {
                return quadraticBezier.StartPoint;
            }

            if (quadraticBezier.Point1.Bounds?.TryToGetPoint(quadraticBezier.Point1, target, radius, hitTest) != null)
            {
                return quadraticBezier.Point1;
            }

            if (quadraticBezier.Point2.Bounds?.TryToGetPoint(quadraticBezier.Point2, target, radius, hitTest) != null)
            {
                return quadraticBezier.Point2;
            }

            foreach (var point in quadraticBezier.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            quadraticBezier.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<IPointShape>();
            quadraticBezier.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            if (box.TopLeft.Bounds?.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (box.BottomRight.Bounds?.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class ReferenceBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ReferenceShape reference))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var point in reference.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ReferenceShape reference))
            {
                throw new ArgumentNullException("shape");
            }

            if (reference.Template?.Bounds != null)
            {
                var adjustedTarget = new Point2(target.X - reference.X, target.Y - reference.Y);
                var result = reference.Template.Bounds.Contains(reference.Template, adjustedTarget, radius, hitTest);
                if (result != null)
                {
                    return reference;
                }
            }

            return null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ReferenceShape reference))
            {
                throw new ArgumentNullException("shape");
            }

            if (reference.Template?.Bounds != null)
            {
                var adjustedTarget = new Rect2(target.X - reference.X, target.Y - reference.Y, target.Width, target.Height);
                var result = reference.Template.Bounds.Overlaps(reference.Template, adjustedTarget, radius, hitTest);
                if (result != null)
                {
                    return reference;
                }
            }

            return null;
        }
    }

    [DataContract(IsReference = true)]
    public class TextBounds : ViewModelBase, IBounds
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            if (box.TopLeft.Bounds?.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (box.BottomRight.Bounds?.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (point.Bounds?.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }
}

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class HitTest : ViewModelBase, IHitTest
    {
        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius)
        {
            return shape.Bounds?.TryToGetPoint(shape, target, radius, this);
        }

        public IPointShape TryToGetPoint(IEnumerable<IBaseShape> shapes, Point2 target, double radius, IPointShape exclude)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = TryToGetPoint(shape, target, radius);
                if (result != null && result != exclude)
                {
                    return result;
                }
            }
            return null;
        }

        public IBaseShape TryToGetShape(IEnumerable<IBaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = shape.Bounds?.Contains(shape, target, radius, this);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public ISet<IBaseShape> TryToGetShapes(IEnumerable<IBaseShape> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<IBaseShape>();
            foreach (var shape in shapes.Reverse())
            {
                var result = shape.Bounds?.Overlaps(shape, target, radius, this);
                if (result != null)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}

namespace Draw2D.ViewModels.Bounds
{
    public static class HitTestHelper
    {
        public static MonotoneChain MC => new MonotoneChain();

        public static SeparatingAxisTheorem SAT => new SeparatingAxisTheorem();

        public static Vector2[] ToSelection(Rect2 rect)
        {
            return new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };
        }

        public static void ToConvexHull(IEnumerable<IPointShape> points, out int k, out Vector2[] convexHull)
        {
            Vector2[] vertices = new Vector2[points.Count()];
            int i = 0;
            foreach (var point in points)
            {
                vertices[i] = new Vector2(point.X, point.Y);
                i++;
            }
            MC.ConvexHull(vertices, out convexHull, out k);
        }

        public static bool Contains(IEnumerable<IPointShape> points, Point2 point)
        {
            ToConvexHull(points, out int k, out Vector2[] convexHull);
            bool contains = false;
            for (int i = 0, j = k - 2; i < k - 1; j = i++)
            {
                if (((convexHull[i].Y > point.Y) != (convexHull[j].Y > point.Y))
                    && (point.X < (convexHull[j].X - convexHull[i].X) * (point.Y - convexHull[i].Y) / (convexHull[j].Y - convexHull[i].Y) + convexHull[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        public static bool Overlap(IEnumerable<IPointShape> points, Vector2[] selectionState)
        {
            ToConvexHull(points, out int k, out Vector2[] convexHull);
            Vector2[] vertices = convexHull.Take(k).ToArray();
            return SAT.Overlap(selectionState, vertices);
        }

        public static bool Overlap(IEnumerable<IPointShape> points, Rect2 rect)
        {
            return Overlap(points, ToSelection(rect));
        }
    }
}
