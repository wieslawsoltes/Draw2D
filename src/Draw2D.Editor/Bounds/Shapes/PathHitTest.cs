// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class PathHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(PathShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var path = shape as PathShape;
            if (path == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            foreach (var pathPoint in path.Points)
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, hitTest) != null)
                {
                    return pathPoint;
                }
            }

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var figureHitTest = hitTest.Registered[figureShape.GetType()];
                    var result = figureHitTest.TryToGetPoint(figureShape, target, radius, hitTest);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public override ShapeObject Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var path = shape as PathShape;
            if (path == null)
                throw new ArgumentNullException("shape");

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var figureHitTest = hitTest.Registered[figureShape.GetType()];
                    var result = figureHitTest.Contains(figureShape, target, radius, hitTest);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return HitTestHelper.Contains(path.GetPoints(), target) ? shape : null;
        }

        public override ShapeObject Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var path = shape as PathShape;
            if (path == null)
                throw new ArgumentNullException("shape");

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var figureHitTest = hitTest.Registered[figureShape.GetType()];
                    var result = figureHitTest.Overlaps(figureShape, target, radius, hitTest);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return HitTestHelper.Overlap(path.GetPoints(), target) ? shape : null;
        }
    }
}
