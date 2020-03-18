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
    public class LineLineIntersection : PointIntersection
    {
        private LineLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Line-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineLineSettings Settings
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

            var lines = context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.OfType<LineShape>();
            if (lines.Any())
            {
                var a0 = line.StartPoint.ToPoint2();
                var b0 = line.Point.ToPoint2();
                foreach (var l in lines)
                {
                    var a1 = l.StartPoint.ToPoint2();
                    var b1 = l.Point.ToPoint2();
                    bool intersection = Line2.LineIntersectWithLine(a0, b0, a1, b1, out var clip);
                    if (intersection)
                    {
                        var point = new PointShape(clip.X, clip.Y, context?.DocumentContainer?.PointTemplate);
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
