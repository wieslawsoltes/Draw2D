using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Filters
{
    public class LineSnapPointFilter : PointFilter
    {
        public LineSnapSettings Settings { get; set; }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.Target.HasFlag(LineSnapTarget.Guides))
            {
                if (Process(context, ref x, ref y, context.Container.Guides))
                {
                    return true;
                }
            }

            if (Settings.Target.HasFlag(LineSnapTarget.Shapes))
            {
                if (Process(context, ref x, ref y, context.Container.Shapes.OfType<LineShape>()))
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
                Point2 snap;
                LineSnapMode mode;
                var result = SnapLines(lines, Settings.Mode, Settings.Treshold, new Point2(x, y), out snap, out mode);
                if (result)
                {
                    x = snap.X;
                    y = snap.Y;

                    if (Settings.EnableGuides)
                    {
                        Clear(context);
                        PointGuides(context, x, y);
                    }

                    Debug.WriteLine("Line Snap {0} ({1})", mode, Settings.Target);
                    return true;
                }
                Clear(context);
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape(
                new PointShape(0, y, null),
                new PointShape(context.Container.Width, y, null));
            horizontal.Style = Settings.GuideStyle;

            var vertical = new LineShape(
                new PointShape(x, 0, null),
                new PointShape(x, context.Container.Height, null));
            vertical.Style = Settings.GuideStyle;

            Guides.Add(horizontal);
            context.WorkingContainer.Shapes.Add(horizontal);
            context.Renderer.Selected.Add(horizontal);

            Guides.Add(vertical);
            context.WorkingContainer.Shapes.Add(vertical);
            context.Renderer.Selected.Add(vertical);
        }

        public static bool SnapLinesToPoint(IEnumerable<LineShape> lines, double treshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
        
            foreach (var line in lines)
            {
                var distance0 = line.Start.ToPoint2().DistanceTo(point);
                if (distance0 < treshold)
                {
                    snap = new Point2(line.Start.X, line.Start.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
                var distance1 = line.End.ToPoint2().DistanceTo(point);
                if (distance1 < treshold)
                {
                    snap = new Point2(line.End.X, line.End.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToMiddle(IEnumerable<LineShape> lines, double treshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
        
            foreach (var line in lines)
            {
                var middle = Line2.Middle(line.Start.ToPoint2(), line.End.ToPoint2());
                var distance = middle.DistanceTo(point);
                if (distance < treshold)
                {
                    snap = middle;
                    result = LineSnapMode.Middle;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToIntersection(IEnumerable<LineShape> lines, double treshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
        
            foreach (var line0 in lines)
            {
                foreach (var line1 in lines)
                {
                    if (line0 == line1)
                        continue;
                    Point2 clip;
                    if (Line2.LineIntersectWithLine(line0.Start.ToPoint2(), line0.End.ToPoint2(), line1.Start.ToPoint2(), line1.End.ToPoint2(), out clip))
                    {
                        var distance = clip.DistanceTo(point);
                        if (distance < treshold)
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

        public static bool SnapLineToHorizontal(IEnumerable<LineShape> lines, double treshold, Point2 point, out Point2 snap, out LineSnapMode result, out double horizontal)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
            horizontal = default(double);
            
            foreach (var line in lines)
            {
                if (point.Y >= line.Start.Y - treshold && point.Y <= line.Start.Y + treshold)
                {
                    snap = new Point2(point.X, line.Start.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.Start.Y;
                    return true;
                }
                if (point.Y >= line.End.Y - treshold && point.Y <= line.End.Y + treshold)
                {
                    snap = new Point2(point.X, line.Start.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.End.Y;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToVertical(IEnumerable<LineShape> lines, double treshold, Point2 point, out Point2 snap, out LineSnapMode result, out double vertical)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
            vertical = default(double);
            
            foreach (var line in lines)
            {
                if (point.X >= line.Start.X - treshold && point.X <= line.Start.X + treshold)
                {
                    snap = new Point2(line.Start.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.Start.X;
                    return true;
                }
                if (point.X >= line.End.X - treshold && point.X <= line.End.X + treshold)
                {
                    snap = new Point2(line.End.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.End.X;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToNearest(IEnumerable<LineShape> lines, double treshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);
            
            foreach (var line in lines)
            {
                var nearest = point.NearestOnLine(line.Start.ToPoint2(), line.End.ToPoint2());
                var distance = nearest.DistanceTo(point);
                if (distance < treshold)
                {
                    snap = nearest;
                    result = LineSnapMode.Nearest;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLines(IEnumerable<LineShape> lines, LineSnapMode mode, double treshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default(Point2);
            result = default(LineSnapMode);

            if (mode.HasFlag(LineSnapMode.Point))
            {
                if (SnapLinesToPoint(lines, treshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Middle))
            {
                if (SnapLineToMiddle(lines, treshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Intersection))
            {
                if (SnapLineToIntersection(lines, treshold, point, out snap, out result))
                {
                    return true;
                }
            }

            double horizontal = default(double);
            double vertical = default(double);

            if (mode.HasFlag(LineSnapMode.Horizontal))
            {
                SnapLineToHorizontal(lines, treshold, point, out snap, out result, out horizontal);
            }

            if (mode.HasFlag(LineSnapMode.Vertical))
            {
                SnapLinesToVertical(lines, treshold, point, out snap, out result, out vertical);
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
                if (SnapLinesToNearest(lines, treshold, point, out snap, out result))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
