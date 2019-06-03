// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Spatial;

namespace Draw2D.ViewModels.Shapes
{
    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this IPointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static IPointShape FromPoint2(this Point2 point, IBaseShape template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }
    }
}
