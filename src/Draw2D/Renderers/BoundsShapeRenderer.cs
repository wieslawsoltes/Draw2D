// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using SkiaSharp;

namespace Draw2D.Renderers
{
    public class BoundsShapeRenderer : IShapeRenderer
    {
        public readonly struct Node
        {
            public readonly BaseShape shape;
            public readonly string styleId;
            public readonly double dx;
            public readonly double dy;
            public readonly double scale;
            public readonly SKPath geometry;
            public readonly SKRect bounds;

            public Node(BaseShape shape, string styleId, double dx, double dy, double scale, SKPath geometry)
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

            public bool Intersects(ref Node node)
            {
                using (var result = this.geometry.Op(node.geometry, SKPathOp.Intersect))
                {
                    return result?.IsEmpty == false;
                }
            }
        }

        public IList<Node> Nodes;

        public BoundsShapeRenderer()
        {
            Nodes = new List<Node>();
        }

        public void Dispose()
        {
            if (Nodes != null)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].geometry.Dispose();
                }
                Nodes = null;
            }
        }

        public void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(line, styleId, dx, dy, scale, SkiaHelper.ToGeometry(line, dx, dy)));

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(cubicBezier, styleId, dx, dy, scale, SkiaHelper.ToGeometry(cubicBezier, dx, dy)));

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(quadraticBezier, styleId, dx, dy, scale, SkiaHelper.ToGeometry(quadraticBezier, dx, dy)));

        public void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(conic, styleId, dx, dy, scale, SkiaHelper.ToGeometry(conic, dx, dy)));

        public void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(path, styleId, dx, dy, scale, SkiaHelper.ToGeometry(path, dx, dy)));

        public void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(rectangle, styleId, dx, dy, scale, SkiaHelper.ToGeometry(rectangle, dx, dy)));

        public void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(ellipse, styleId, dx, dy, scale, SkiaHelper.ToGeometry(ellipse, dx, dy)));

        public void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale)
            => Nodes.Add(new Node(text, styleId, dx, dy, scale, SkiaHelper.ToGeometry(text, dx, dy)));
    }
}
