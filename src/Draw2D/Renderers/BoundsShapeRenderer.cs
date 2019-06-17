// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using SkiaSharp;

namespace Draw2D.Renderers
{
    public class BoundsShapeRenderer : IShapeRenderer
    {
        public readonly struct Root : IDisposable
        {
            public readonly IBaseShape shape;
            public readonly IList<Node> nodes;

            public Root(IBaseShape shape)
            {
                this.shape = shape;
                this.nodes = new List<Node>();
            }

            public bool Contains(float x, float y, out IBaseShape shape)
            {
                shape = null;
                for (int i = 0; i < nodes.Count; i++)
                {
                    //if (nodes[i].geometry.Contains(x, y))
                    if (nodes[i].bounds.Contains(x, y))
                    {
                        shape = this.shape;
                        return true;
                    }
                }
                return false;
            }

            public bool Intersects(SKPath geometry, out IBaseShape shape)
            {
                shape = null;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Intersects(geometry))
                    {
                        shape = this.shape;
                        return true;
                    }
                }
                return false;
            }

            public bool Intersects(ref SKRect rect, out IBaseShape shape)
            {
                shape = null;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].bounds.IntersectsWith(rect))
                    {
                        shape = this.shape;
                        return true;
                    }
                }
                return false;
            }

            public void Dispose()
            {
                for (int i = 0; i < nodes?.Count; i++)
                {
                    nodes[i].Dispose();
                }
            }
        }

        public readonly struct Node : IDisposable
        {
            public readonly IBaseShape shape;
            public readonly string styleId;
            public readonly double dx;
            public readonly double dy;
            public readonly double scale;
            public readonly SKPath geometry;
            public readonly SKRect bounds;

            public Node(IBaseShape shape, string styleId, double dx, double dy, double scale, SKPath geometry)
            {
                this.shape = shape;
                this.styleId = styleId;
                this.dx = dx;
                this.dy = dy;
                this.scale = scale;
                this.geometry = geometry;
                geometry.GetBounds(out this.bounds);
            }

            public bool Contains(float x, float y)
            {
                return geometry.Contains(x, y);
            }

            public bool Intersects(SKPath geometry)
            {
                using (var result = this.geometry.Op(geometry, SKPathOp.Intersect))
                {
                    return result?.IsEmpty == false;
                }
            }

            public bool Intersects(ref SKRect rect)
            {
                return this.bounds.IntersectsWith(rect);
            }

            public void Dispose()
            {
                geometry?.Dispose();
            }
        }

        private int _current = -1;
        private IList<Root> _roots;

        private BoundsShapeRenderer()
        {
        }

        public static BoundsShapeRenderer Create(ICanvasContainer container)
        {
            var renderer = new BoundsShapeRenderer();

            renderer._roots = new List<Root>();

            var points = new List<IPointShape>();

            container.GetPoints(points);

            foreach (var point in points)
            {
                renderer._roots.Add(new Root(point));
                renderer._current++;
                point.Draw(null, renderer, 0.0, 0.0, 1.0, null, null);
            }

            foreach (var shape in container.Shapes)
            {
                renderer._roots.Add(new Root(shape));
                renderer._current++;
                shape.Draw(null, renderer, 0.0, 0.0, 1.0, null, null);
            }

            return renderer;
        }

        public bool Contains(float x, float y, out IBaseShape shape)
        {
            shape = null;
            for (int i = 0; i < _roots.Count; i++)
            {
                if (_roots[i].Contains(x, y, out shape))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(SKPath geometry, out IBaseShape shape)
        {
            shape = null;
            for (int i = 0; i < _roots.Count; i++)
            {
                if (_roots[i].Intersects(geometry, out shape))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(ref SKRect rect, out IBaseShape shape)
        {
            shape = null;
            for (int i = 0; i < _roots.Count; i++)
            {
                if (_roots[i].Intersects(ref rect, out shape))
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (_roots != null)
            {
                for (int i = 0; i < _roots.Count; i++)
                {
                    _roots[i].Dispose();
                }
                _roots = null;
            }
        }

        public void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(line, styleId, dx, dy, scale, SkiaHelper.ToGeometry(line, dx, dy)));

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(cubicBezier, styleId, dx, dy, scale, SkiaHelper.ToGeometry(cubicBezier, dx, dy)));

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(quadraticBezier, styleId, dx, dy, scale, SkiaHelper.ToGeometry(quadraticBezier, dx, dy)));

        public void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(conic, styleId, dx, dy, scale, SkiaHelper.ToGeometry(conic, dx, dy)));

        public void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(path, styleId, dx, dy, scale, SkiaHelper.ToGeometry(path, dx, dy)));

        public void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(rectangle, styleId, dx, dy, scale, SkiaHelper.ToGeometry(rectangle, dx, dy)));

        public void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(ellipse, styleId, dx, dy, scale, SkiaHelper.ToGeometry(ellipse, dx, dy)));

        public void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale)
            => _roots[_current].nodes.Add(new Node(text, styleId, dx, dy, scale, SkiaHelper.ToGeometry(text, dx, dy)));
    }
}
