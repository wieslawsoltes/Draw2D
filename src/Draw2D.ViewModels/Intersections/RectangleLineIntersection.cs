// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;
using Spatial;

namespace Draw2D.ViewModels.Intersections
{
    [DataContract(IsReference = true)]
    public class RectangleLineIntersection : PointIntersection
    {
        private RectangleLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Rectangle-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public RectangleLineSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Find(IToolContext context, IBaseShape shape)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            if (!Settings.IsEnabled)
            {
                return;
            }

            var rectangles = context.ContainerView?.CurrentContainer.Shapes.OfType<RectangleShape>();
            if (rectangles.Any())
            {
                foreach (var rectangle in rectangles)
                {
                    var rect = Rect2.FromPoints(rectangle.StartPoint.ToPoint2(), rectangle.Point.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    var intersections = Line2.LineIntersectsWithRect(p1, p2, rect, out double x0clip, out double y0clip, out double x1clip, out double y1clip);
                    if (intersections)
                    {
                        var point1 = new PointShape(x0clip, y0clip, context.PointTemplate);
                        point1.Owner = context.ContainerView?.WorkingContainer;
                        Intersections.Add(point1);
                        //context.ContainerView?.WorkingContainer.Shapes.Add(point1);
                        //context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point1);

                        var point2 = new PointShape(x1clip, y1clip, context.PointTemplate);
                        point2.Owner = context.ContainerView?.WorkingContainer;
                        Intersections.Add(point2);
                        //context.ContainerView?.WorkingContainer.Shapes.Add(point2);
                        //context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point2);
                    }
                }
            }
        }
    }
}
