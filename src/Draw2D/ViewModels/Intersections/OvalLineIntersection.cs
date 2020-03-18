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
    public class OvalLineIntersection : PointIntersection
    {
        private OvalLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Oval-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public OvalLineSettings Settings
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

            var ovals = context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.OfType<OvalShape>();
            if (ovals.Any())
            {
                foreach (var oval in ovals)
                {
                    var rect = Rect2.FromPoints(oval.StartPoint.ToPoint2(), oval.Point.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    Line2.LineIntersectsWithEllipse(p1, p2, rect, true, out var intersections);
                    if (intersections != null && intersections.Any())
                    {
                        foreach (var p in intersections)
                        {
                            var point = new PointShape(p.X, p.Y, context?.DocumentContainer?.PointTemplate);
                            point.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
                            Intersections.Add(point);
                            //context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(point);
                            //context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                            context.DocumentContainer?.ContainerView?.SelectionState?.Select(point);
                        }
                    }
                }
            }
        }
    }
}
