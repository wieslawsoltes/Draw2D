// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Spatial;

namespace Draw2D.Core.Shapes
{
    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this PointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static PointShape FromPoint2(this Point2 point, ShapeObject template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }
    }
}
