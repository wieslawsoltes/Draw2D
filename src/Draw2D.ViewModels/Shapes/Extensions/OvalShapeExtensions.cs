// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using Spatial;

namespace Draw2D.ViewModels.Shapes
{
    public static class OvalShapeExtensions
    {
        public static Rect2 ToRect2(this OvalShape oval, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                oval.StartPoint.X, oval.StartPoint.Y,
                oval.Point.X, oval.Point.Y,
                dx, dy);
        }

        public static OvalShape FromRect2(this Rect2 rect)
        {
            return new OvalShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2())
            {
                Points = new ObservableCollection<IPointShape>()
            };
        }
    }
}
