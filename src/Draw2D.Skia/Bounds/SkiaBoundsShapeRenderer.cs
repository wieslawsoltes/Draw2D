// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Shapes;
using SkiaSharp;

namespace Draw2D.Bounds
{
    internal class SkiaBoundsShapeRenderer : IShapeRenderer
    {
        internal int _currentRootNode = -1;
        internal IList<RootNode> _rootNodes;

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

        public void DrawImage(object dc, ImageShape image, string styleId, double dx, double dy, double scale)
        {
            var geometry = new SKPath() { FillType = SKPathFillType.Winding };
            SkiaHelper.AddImage(null, image, dx, dy, geometry);
            _rootNodes[_currentRootNode].Children.Add(new ChildNode(image, styleId, dx, dy, scale, geometry));
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
    }
}
