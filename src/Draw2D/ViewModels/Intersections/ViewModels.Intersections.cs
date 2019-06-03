// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

namespace Draw2D.ViewModels.Intersections
{
    [DataContract(IsReference = true)]
    public abstract class PointIntersection : ViewModelBase, IPointIntersection
    {
        private IList<IPointShape> _intersections;

        public abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointShape> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        public abstract void Find(IToolContext context, IBaseShape shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(point);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(point);
            }
            Intersections.Clear();
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseLineSettings : Settings
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }

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
                    var rect = Rect2.FromPoints(ellipse.TopLeft.ToPoint2(), ellipse.BottomRight.ToPoint2());
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

    [DataContract(IsReference = true)]
    public class LineLineSettings : Settings
    {
        private bool _isEnabled;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }

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

            var lines = context.ContainerView?.CurrentContainer.Shapes.OfType<LineShape>();
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
                        var point = new PointShape(clip.X, clip.Y, context.PointTemplate);
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

    [DataContract(IsReference = true)]
    public class RectangleLineSettings : Settings
    {
        private bool _isEnabled;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }

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
                    var rect = Rect2.FromPoints(rectangle.TopLeft.ToPoint2(), rectangle.BottomRight.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    var intersections = Line2.LineIntersectsWithRect(p1, p2, rect, out double x0clip, out double y0clip, out double x1clip, out double y1clip);
                    if (intersections)
                    {
                        var point1 = new PointShape(x0clip, y0clip, context.PointTemplate);
                        point1.Owner = context.ContainerView?.WorkingContainer;
                        Intersections.Add(point1);
                        context.ContainerView?.WorkingContainer.Shapes.Add(point1);
                        context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point1);

                        var point2 = new PointShape(x1clip, y1clip, context.PointTemplate);
                        point2.Owner = context.ContainerView?.WorkingContainer;
                        Intersections.Add(point2);
                        context.ContainerView?.WorkingContainer.Shapes.Add(point2);
                        context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point2);
                    }
                }
            }
        }
    }
}
