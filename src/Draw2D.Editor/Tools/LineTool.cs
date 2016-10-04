using System;
using System.Linq;
using Draw2D.Editor.Intersections;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public class LineTool : ToolBase
    {
        private enum State { StartPoint, Point };
        private State _state = State.StartPoint;
        private LineShape _line = null;

        public override string Name { get { return "Line"; } }

        public LineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.StartPoint:
                    {
                        PointShape point = null;
                        if (Settings.ConnectPoints)
                        {
                            point = context.HitTest.TryToGetPoint(context.CurrentContainer.Shapes, new Point2(x, y), Settings.HitTestRadius);
                        }

                        _line = new LineShape(
                            point ?? new PointShape(x, y, context.PointShape),
                            new PointShape(x, y, context.PointShape));
                        _line.Style = context.Style;
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Selected.Add(_line.StartPoint);
                        context.Selected.Add(_line.Point);
                        _state = State.Point;
                    }
                    break;
                case State.Point:
                    {
                        _state = State.StartPoint;

                        PointShape point = null;
                        if (Settings.ConnectPoints)
                        {
                            point = context.HitTest.TryToGetPoint(context.CurrentContainer.Shapes, new Point2(x, y), Settings.HitTestRadius);
                        }

                        if (point != null)
                        {
                            _line.Point = point;
                        }
                        else
                        {
                            _line.Point.X = x;
                            _line.Point.Y = y;
                        }

                        context.WorkingContainer.Shapes.Remove(_line);
                        context.Selected.Remove(_line.StartPoint);
                        context.Selected.Remove(_line.Point);

                        Intersections.ForEach(i => i.Clear(context));
                        Intersections.ForEach(i => i.Find(context, _line));

                        if (Settings.SplitIntersections && Intersections.Any(i => i.Intersections.Count > 0))
                        {
                            LineHelper.SplitByIntersections(context, Intersections, _line);
                        }
                        else
                        {
                            context.CurrentContainer.Shapes.Add(_line);
                        }

                        _line = null;
                        Intersections.ForEach(i => i.Clear(context));
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (_state)
            {
                case State.Point:
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
                case State.Point:
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

            _state = State.StartPoint;

            Intersections.ForEach(i => i.Clear(context));
            Filters.ForEach(f => f.Clear(context));

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                context.Selected.Remove(_line.StartPoint);
                context.Selected.Remove(_line.Point);
                _line = null;
            }
        }
    }
}
