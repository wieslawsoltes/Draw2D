using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public class PolyLineTool : ToolBase
    {
        private enum State { Start, End };
        private State _state = State.Start;
        private LineShape _line = null;
        private List<PointShape> _points = null;

        public override string Name { get { return "PolyLine"; } }

        public PolyLineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.Start:
                    {
                        _points = new List<PointShape>();

                        PointShape point = null;
                        if (Settings.ConnectPoints)
                        {
                            point = context.HitTest.TryToGetPoint(context.CurrentContainer.Shapes, new Point2(x, y), Settings.HitTestRadius);
                        }

                        _line = new LineShape(
                            point ?? new PointShape(x, y, context.PointShape),
                            new PointShape(x, y, context.PointShape));
                        _line.Style = context.CurrentStyle;
                        _points.Add(_line.StartPoint);
                        _points.Add(_line.Point);
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Selected.Add(_line.StartPoint);
                        context.Selected.Add(_line.Point);
                        _state = State.End;
                    }
                    break;
                case State.End:
                    {
                        PointShape point = null;
                        if (Settings.ConnectPoints)
                        {
                            point = context.HitTest.TryToGetPoint(context.CurrentContainer.Shapes, new Point2(x, y), Settings.HitTestRadius);
                        }

                        if (point != null)
                        {
                            _line.Point = point;

                            _points[_points.Count - 1] = _line.Point;
                            if (!context.Selected.Contains(_line.Point))
                            {
                                context.Selected.Add(_line.Point);
                            }
                        }
                        else
                        {
                            _line.Point.X = x;
                            _line.Point.Y = y;
                        }

                        context.WorkingContainer.Shapes.Remove(_line);
                        context.CurrentContainer.Shapes.Add(_line);

                        _line = new LineShape(
                            _points.Last(),
                            new PointShape(x, y, context.PointShape));
                        _line.Style = context.CurrentStyle;
                        _points.Add(_line.Point);
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Selected.Add(_line.Point);

                        Intersections.ForEach(i => i.Clear(context));
                        Filters.ForEach(f => f.Clear(context));
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (_state)
            {
                case State.End:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.End:
                    {
                        _line.Point.X = x;
                        _line.Point.Y = y;
                        Intersections.ForEach(i => i.Clear(context));
                        Intersections.ForEach(i => i.Find(context, _line));
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            _state = State.Start;

            Intersections.ForEach(i => i.Clear(context));
            Filters.ForEach(f => f.Clear(context));

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                _line = null;
            }

            if (_points != null)
            {
                _points.ForEach(point => context.Selected.Remove(point));
                _points = null;
            }
        }
    }
}
