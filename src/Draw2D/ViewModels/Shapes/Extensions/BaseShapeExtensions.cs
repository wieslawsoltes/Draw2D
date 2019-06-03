// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Draw2D.ViewModels.Shapes
{
    public static class BaseShapeExtensions
    {
        public static void GetBox(this IList<IPointShape> points, out double ax, out double ay, out double bx, out double by)
        {
            ax = double.MaxValue;
            ay = double.MaxValue;
            bx = double.MinValue;
            by = double.MinValue;

            foreach (var point in points)
            {
                ax = Math.Min(ax, point.X);
                ay = Math.Min(ay, point.Y);
                bx = Math.Max(bx, point.X);
                by = Math.Max(by, point.Y);
            }
        }

        public static void GetBox(this IBaseShape shape, out double ax, out double ay, out double bx, out double by)
        {
            var points = new List<IPointShape>();
            shape.GetPoints(points);
            GetBox(points, out ax, out ay, out bx, out by);
        }
    }
}
