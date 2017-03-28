using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArcDemo
{
    public class RenderCanvas : Canvas
    {
        enum State { StartPoint, Point }
        State _state;
        Arc _arc;
        List<Arc> Arcs;

        public RenderCanvas()
        {
            Arcs = new List<Arc>();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            switch (_state)
            {
                case State.StartPoint:
                    {
                        var point = e.GetPosition(this);
                        _arc = new Arc()
                        {
                            StartPoint = point,
                            Point = point
                        };
                        Arcs.Add(_arc);
                        this.CaptureMouse();
                        this.InvalidateVisual();
                        _state = State.Point;
                    }
                    break;
                case State.Point:
                    {
                        this.ReleaseMouseCapture();
                        _state = State.StartPoint;
                        var point = e.GetPosition(this);
                        _arc.Point = point;
                        _arc = null;
                    }
                    break;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            switch (_state)
            {
                case State.Point:
                    {
                        var point = e.GetPosition(this);
                        _arc.Point = point;
                        this.InvalidateVisual();
                    }
                    break;
            }
        }

        double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (double)Math.Sqrt(dx * dx + dy * dy);
        }

        double Angle(Point a, Point b)
        {
            return (double)Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        void DrawArc(DrawingContext dc, Arc arc)
        {
            var distance = Distance(arc.StartPoint, arc.Point);
            var size = new Size(distance / 2, distance / 2);

            var rect = new Rect(arc.StartPoint, new Size(distance, distance / 2));
            var angle = Angle(arc.StartPoint, arc.Point) * 180.0 / Math.PI;
            var tt = new TranslateTransform(0, -distance / 2);
            var rt = new RotateTransform(angle, arc.StartPoint.X, arc.StartPoint.Y);
            dc.PushTransform(rt);
            dc.PushTransform(tt);
            dc.DrawRectangle(null, new Pen(Brushes.Cyan, 2.0), rect);
            dc.Pop();
            dc.Pop();

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(arc.StartPoint, false, false);
                context.ArcTo(arc.Point, size, 0, true, SweepDirection.Clockwise, true, false);
            }
            dc.DrawGeometry(null, new Pen(Brushes.Red, 2.0), geometry);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            foreach (var arc in Arcs)
            {
                DrawArc(dc, arc);
            }
        }
    }
}
