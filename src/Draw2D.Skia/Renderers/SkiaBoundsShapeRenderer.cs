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
    public class SkiaBoundsShapeRenderer : IShapeRenderer
    {
        private int _currentRootNode = -1;
        private IList<RootNode> _rootNodes;

        public SkiaBoundsShapeRenderer(ICanvasContainer container)
        {
            _rootNodes = new List<RootNode>();

            var points = new List<IPointShape>();

            container.GetPoints(points);

            foreach (var point in points)
            {
                _rootNodes.Add(new RootNode(point));
                _currentRootNode++;
                point.Draw(null, this, 0.0, 0.0, 1.0, null, null);
            }

            foreach (var shape in container.Shapes)
            {
                _rootNodes.Add(new RootNode(shape));
                _currentRootNode++;
                shape.Draw(null, this, 0.0, 0.0, 1.0, null, null);
            }
        }

        public bool Contains(float x, float y, ContainsMode mode, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < _rootNodes.Count; i++)
            {
                if (_rootNodes[i].Contains(x, y, mode, out rootShape, out childShape))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(SKPath geometry, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < _rootNodes.Count; i++)
            {
                if (_rootNodes[i].Intersects(geometry, out rootShape, out childShape))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(ref SKRect rect, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < _rootNodes.Count; i++)
            {
                if (_rootNodes[i].Intersects(ref rect, out rootShape, out childShape))
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (_rootNodes != null)
            {
                for (int i = 0; i < _rootNodes.Count; i++)
                {
                    _rootNodes[i].Dispose();
                }
                _rootNodes = null;
            }
        }

        public void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddLine(null, line, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(line, styleId, dx, dy, scale, geometry));
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddCubic(null, cubicBezier, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(cubicBezier, styleId, dx, dy, scale, geometry));
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddQuad(null, quadraticBezier, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(quadraticBezier, styleId, dx, dy, scale, geometry));
        }

        public void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddConic(null, conic, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(conic, styleId, dx, dy, scale, geometry));
        }

        public void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SkiaHelper.ToSKPathFillType(path.FillType) };
            SkiaHelper.AddPath(null, path, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(path, styleId, dx, dy, scale, geometry));
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddRect(null, rectangle, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(rectangle, styleId, dx, dy, scale, geometry));
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddOval(null, ellipse, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(ellipse, styleId, dx, dy, scale, geometry));
        }

        public void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddText(null, text, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(text, styleId, dx, dy, scale, geometry));
        }
    }
}
