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
    public class EllipseLineIntersection : PointIntersection
    {
        private EllipseLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Ellipse-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EllipseLineSettings Settings
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

            var ellipses = context.ContainerView?.CurrentContainer.Shapes.OfType<EllipseShape>();
            if (ellipses.Any())
            {
                foreach (var ellipse in ellipses)
                {
                    var rect = Rect2.FromPoints(ellipse.StartPoint.ToPoint2(), ellipse.Point.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    Line2.LineIntersectsWithEllipse(p1, p2, rect, true, out var intersections);
                    if (intersections != null && intersections.Any())
                    {
                        foreach (var p in intersections)
                        {
                            var point = new PointShape(p.X, p.Y, context.PointTemplate);
                            point.Owner = context.ContainerView?.WorkingContainer;
                            Intersections.Add(point);
                            context.ContainerView?.WorkingContainer.Shapes.Add(point);
                            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                            context.ContainerView?.SelectionState?.Select(point);
                        }
                    }
                }
            }
        }
    }
}
