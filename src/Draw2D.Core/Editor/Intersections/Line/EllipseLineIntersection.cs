// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Intersections.Line
{
    public class EllipseLineIntersection : PointIntersection
    {
        public override string Name { get { return "Ellipse-Line"; } }
        public EllipseLineSettings Settings { get; set; }

        public override void Find(IToolContext context, ShapeObject shape)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            if (!Settings.IsEnabled)
                return;

            var ellipses = context.CurrentContainer.Shapes.OfType<EllipseShape>();
            if (ellipses.Any())
            {
                foreach (var ellipse in ellipses)
                {
                    var rect = Rect2.FromPoints(ellipse.TopLeft.ToPoint2(), ellipse.BottomRight.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    IList<Point2> intersections;
                    Line2.LineIntersectsWithEllipse(p1, p2, rect, true, out intersections);
                    if (intersections != null && intersections.Any())
                    {
                        foreach (var p in intersections)
                        {
                            var point = new PointShape(p.X, p.Y, context.PointShape);
                            Intersections.Add(point);
                            context.WorkingContainer.Shapes.Add(point);
                            context.Selected.Add(point);
                        }
                    }
                }
            }
        }
    }
}
