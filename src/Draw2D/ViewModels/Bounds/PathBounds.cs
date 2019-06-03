// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
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
}
