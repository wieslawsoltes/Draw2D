// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public class PathHitTest : HitTestBase
    {
        public override Type TargetType { get { return typeof(PathShape); } }

        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var path = shape as PathShape;
            if (path == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(PointShape)];

            foreach (var groupPoint in path.Points)
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, registered) != null)
                {
                    return groupPoint;
                }
            }

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var hitTest = registered[figureShape.GetType()];
                    var result = hitTest.TryToGetPoint(figureShape, target, radius, registered);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public override bool Contains(ShapeObject shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var path = shape as PathShape;
            if (path == null)
                throw new ArgumentNullException("shape");

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var hitTest = registered[figureShape.GetType()];
                    var result = hitTest.Contains(figureShape, target, radius, registered);
                    if (result == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Overlaps(ShapeObject shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var path = shape as PathShape;
            if (path == null)
                throw new ArgumentNullException("shape");

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var hitTest = registered[figureShape.GetType()];
                    var result = hitTest.Overlaps(figureShape, target, radius, registered);
                    if (result == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
