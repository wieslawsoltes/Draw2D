// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using SkiaSharp;

namespace Draw2D.Bounds
{
    // TODO: Experimental hit-testing for TryToHover
    /*
    var bounds = new SkiaHitTest(context.DocumentContainer?.ContainerView?.CurrentContainer);

    //if (bounds.Contains((float)target.X, (float)target.Y, ContainsMode.Bounds, out var rootShape, out var childShape))
    //{
    //    bounds.Dispose();
    //    return rootShape;
    //}

    var rect = SKRect.Create((float)(target.X - radius), (float)(target.Y - radius), (float)(radius + radius), (float)(radius + radius));
    if (bounds.Intersects(ref rect, out var rootShape, out var childShape))
    {
        bounds.Dispose();
        return rootShape;
    }

    //var rect = SKRect.Create((float)(target.X - radius), (float)(target.Y - radius), (float)(radius + radius), (float)(radius + radius));
    //var geometry = new SKPath() { FillType = SKPathFillType.Winding };
    //geometry.AddRect(rect);
    //if (bounds.Intersects(geometry, out var rootShape, out var childShape))
    //{
    //    bounds.Dispose();
    //    return rootShape;
    //}

    bounds.Dispose();

    return null;
    //*/

    public class SkiaHitTest : IDisposable
    {
        internal SkiaBoundsShapeRenderer _renderer;

        public SkiaHitTest(ICanvasContainer container)
        {
            _renderer = new SkiaBoundsShapeRenderer();
            _renderer._currentRootNode = -1;
            _renderer._rootNodes = new List<RootNode>();

            var points = new List<IPointShape>();

            container.GetPoints(points);

            foreach (var point in points)
            {
                _renderer._rootNodes.Add(new RootNode(point));
                _renderer._currentRootNode++;
                point.Draw(null, _renderer, 0.0, 0.0, 1.0, null, null);
            }

            foreach (var shape in container.Shapes)
            {
                _renderer._rootNodes.Add(new RootNode(shape));
                _renderer._currentRootNode++;
                shape.Draw(null, _renderer, 0.0, 0.0, 1.0, null, null);
            }
        }

        public bool Contains(float x, float y, ContainsMode mode, out IBaseShape rootShape, out IBaseShape childShape)
        {
            rootShape = null;
            childShape = null;
            for (int i = 0; i < _renderer._rootNodes.Count; i++)
            {
                if (_renderer._rootNodes[i].Contains(x, y, mode, out rootShape, out childShape))
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
            for (int i = 0; i < _renderer._rootNodes.Count; i++)
            {
                if (_renderer._rootNodes[i].Intersects(geometry, out rootShape, out childShape))
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
            for (int i = 0; i < _renderer._rootNodes.Count; i++)
            {
                if (_renderer._rootNodes[i].Intersects(ref rect, out rootShape, out childShape))
                {
                    return true;
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }
        }
    }
}
