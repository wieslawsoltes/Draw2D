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

namespace Draw2D.ViewModels.Filters
{
    [Flags]
    public enum GridSnapMode
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        All = Horizontal | Vertical
    }

    [Flags]
    public enum LineSnapMode
    {
        None = 0,
        Point = 1,
        Middle = 2,
        Intersection = 4,
        Horizontal = 8,
        Vertical = 16,
        Nearest = 32,
        All = Point | Middle | Intersection | Horizontal | Vertical | Nearest
    }

    [Flags]
    public enum LineSnapTarget
    {
        None = 0,
        Shapes = 1,
        All = Shapes
    }

    [DataContract(IsReference = true)]
    public abstract class PointFilter : ViewModelBase, IPointFilter
    {
        private IList<IBaseShape> _guides;

        public abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IBaseShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(guide);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(guide);
            }
            Guides.Clear();
        }
    }

    [DataContract(IsReference = true)]
    public class GridSnapSettings : Settings
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private GridSnapMode _mode;
        private double _gridSizeX;
        private double _gridSizeY;
        private string _guideStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GridSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double GridSizeX
        {
            get => _gridSizeX;
            set => Update(ref _gridSizeX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double GridSizeY
        {
            get => _gridSizeY;
            set => Update(ref _gridSizeY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }

    [DataContract(IsReference = true)]
    public class GridSnapPointFilter : PointFilter
    {
        private GridSnapSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Grid-Snap";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GridSnapSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.IsEnabled == false)
            {
                return false;
            }

            if (Settings.Mode != GridSnapMode.None)
            {
                bool haveSnapToGrid = false;

                if (Settings.Mode.HasFlag(GridSnapMode.Horizontal))
                {
                    x = (double)SnapGrid((decimal)x, (decimal)Settings.GridSizeX);
                    haveSnapToGrid = true;
                }

                if (Settings.Mode.HasFlag(GridSnapMode.Vertical))
                {
                    y = (double)SnapGrid((decimal)y, (decimal)Settings.GridSizeY);
                    haveSnapToGrid = true;
                }

                if (Settings.EnableGuides && haveSnapToGrid)
                {
                    PointGuides(context, x, y);
                }

                return haveSnapToGrid;
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.ContainerView?.Width ?? 0, y, null),
                StyleId = Settings.GuideStyle
            };
            horizontal.StartPoint.Owner = horizontal;
            horizontal.Point.Owner = horizontal;

            var vertical = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.ContainerView?.Height ?? 0, null),
                StyleId = Settings.GuideStyle
            };
            vertical.StartPoint.Owner = vertical;
            vertical.Point.Owner = vertical;

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.ContainerView?.WorkingContainer.Shapes.Add(horizontal);
            context.ContainerView?.WorkingContainer.Shapes.Add(vertical);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
        }

        public static decimal SnapGrid(decimal value, decimal size)
        {
            decimal r = value % size;
            return r >= size / 2.0m ? value + size - r : value - r;
        }
    }

    [DataContract(IsReference = true)]
    public class LineSnapSettings : Settings
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private LineSnapMode _mode;
        private LineSnapTarget _target;
        private double _threshold;
        private string _guideStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapTarget Target
        {
            get => _target;
            set => Update(ref _target, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Threshold
        {
            get => _threshold;
            set => Update(ref _threshold, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }

    [DataContract(IsReference = true)]
    public class LineSnapPointFilter : PointFilter
    {
        private LineSnapSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Line-Snap";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.IsEnabled == false)
            {
                return false;
            }

            if (Settings.Target.HasFlag(LineSnapTarget.Shapes))
            {
                if (Process(context, ref x, ref y, context.ContainerView?.CurrentContainer.Shapes.OfType<LineShape>()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Process(IToolContext context, ref double x, ref double y, IEnumerable<LineShape> lines)
        {
            if (lines.Any() && Settings.Mode != LineSnapMode.None)
            {
                var result = SnapLines(lines, Settings.Mode, Settings.Threshold, new Point2(x, y), out var snap, out _);
                if (result)
                {
                    x = snap.X;
                    y = snap.Y;

                    if (Settings.EnableGuides)
                    {
                        Clear(context);
                        PointGuides(context, x, y);
                    }

                    return true;
                }
                Clear(context);
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.ContainerView?.Width ?? 0, y, null),
                StyleId = Settings.GuideStyle
            };
            horizontal.StartPoint.Owner = horizontal;
            horizontal.Point.Owner = horizontal;

            var vertical = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.ContainerView?.Height ?? 0, null),
                StyleId = Settings.GuideStyle
            };
            vertical.StartPoint.Owner = vertical;
            vertical.Point.Owner = vertical;

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.ContainerView?.WorkingContainer.Shapes.Add(horizontal);
            context.ContainerView?.WorkingContainer.Shapes.Add(vertical);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
        }

        public static bool SnapLinesToPoint(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line in lines)
            {
                var distance0 = line.StartPoint.ToPoint2().DistanceTo(point);
                if (distance0 < threshold)
                {
                    snap = new Point2(line.StartPoint.X, line.StartPoint.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
                var distance1 = line.Point.ToPoint2().DistanceTo(point);
                if (distance1 < threshold)
                {
                    snap = new Point2(line.Point.X, line.Point.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToMiddle(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line in lines)
            {
                var middle = Line2.Middle(line.StartPoint.ToPoint2(), line.Point.ToPoint2());
                var distance = middle.DistanceTo(point);
                if (distance < threshold)
                {
                    snap = middle;
                    result = LineSnapMode.Middle;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToIntersection(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line0 in lines)
            {
                foreach (var line1 in lines)
                {
                    if (line0 == line1)
                    {
                        continue;
                    }

                    if (Line2.LineIntersectWithLine(line0.StartPoint.ToPoint2(), line0.Point.ToPoint2(), line1.StartPoint.ToPoint2(), line1.Point.ToPoint2(), out var clip))
                    {
                        var distance = clip.DistanceTo(point);
                        if (distance < threshold)
                        {
                            snap = clip;
                            result = LineSnapMode.Intersection;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool SnapLineToHorizontal(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result, out double horizontal)
        {
            snap = default;
            result = default;
            horizontal = default;

            foreach (var line in lines)
            {
                if (point.Y >= line.StartPoint.Y - threshold && point.Y <= line.StartPoint.Y + threshold)
                {
                    snap = new Point2(point.X, line.StartPoint.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.StartPoint.Y;
                    return true;
                }
                if (point.Y >= line.Point.Y - threshold && point.Y <= line.Point.Y + threshold)
                {
                    snap = new Point2(point.X, line.StartPoint.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.Point.Y;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToVertical(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result, out double vertical)
        {
            snap = default;
            result = default;
            vertical = default;

            foreach (var line in lines)
            {
                if (point.X >= line.StartPoint.X - threshold && point.X <= line.StartPoint.X + threshold)
                {
                    snap = new Point2(line.StartPoint.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.StartPoint.X;
                    return true;
                }
                if (point.X >= line.Point.X - threshold && point.X <= line.Point.X + threshold)
                {
                    snap = new Point2(line.Point.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.Point.X;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToNearest(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line in lines)
            {
                var nearest = point.NearestOnLine(line.StartPoint.ToPoint2(), line.Point.ToPoint2());
                var distance = nearest.DistanceTo(point);
                if (distance < threshold)
                {
                    snap = nearest;
                    result = LineSnapMode.Nearest;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLines(IEnumerable<LineShape> lines, LineSnapMode mode, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            if (mode.HasFlag(LineSnapMode.Point))
            {
                if (SnapLinesToPoint(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Middle))
            {
                if (SnapLineToMiddle(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Intersection))
            {
                if (SnapLineToIntersection(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            double horizontal = default;
            double vertical = default;

            if (mode.HasFlag(LineSnapMode.Horizontal))
            {
                SnapLineToHorizontal(lines, threshold, point, out snap, out result, out horizontal);
            }

            if (mode.HasFlag(LineSnapMode.Vertical))
            {
                SnapLinesToVertical(lines, threshold, point, out snap, out result, out vertical);
            }

            if (result.HasFlag(LineSnapMode.Horizontal) || result.HasFlag(LineSnapMode.Vertical))
            {
                if (result.HasFlag(LineSnapMode.Vertical) || result.HasFlag(LineSnapMode.Horizontal))
                {
                    double x = result.HasFlag(LineSnapMode.Vertical) ? vertical : point.X;
                    double y = result.HasFlag(LineSnapMode.Horizontal) ? horizontal : point.Y;
                    snap = new Point2(x, y);
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Nearest))
            {
                if (SnapLinesToNearest(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
