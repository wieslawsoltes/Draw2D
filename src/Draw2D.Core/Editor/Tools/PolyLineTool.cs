using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Tools
{
    public class PolyLineTool : ToolBase
    {
        private LineShape _line = null;
        private List<PointShape> _points = null;

        public enum State { Start, End };
        public State CurrentState = State.Start;

        public override string Name { get { return "PolyLine"; } }

        public PolyLineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.Start:
                    {
                        _points = new List<PointShape>();
                        _line = new LineShape()
                        {
                            StartPoint = context.GetNextPoint(x, y, Settings.ConnectPoints, Settings.HitTestRadius),
                            Point = context.GetNextPoint(x, y, false, 0.0),
                            Style = context.CurrentStyle
                        };
                        _points.Add(_line.StartPoint);
                        _points.Add(_line.Point);
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Selected.Add(_line.StartPoint);
                        context.Selected.Add(_line.Point);
                        CurrentState = State.End;
                    }
                    break;
                case State.End:
                    {
                        context.Selected.Remove(_line.Point);
                        _line.Point = context.GetNextPoint(x, y, Settings.ConnectPoints, Settings.HitTestRadius);
                        _points[_points.Count - 1] = _line.Point;

                        if (!context.Selected.Contains(_line.Point))
                        {
                            context.Selected.Add(_line.Point);
                        }

                        context.WorkingContainer.Shapes.Remove(_line);
                        context.CurrentContainer.Shapes.Add(_line);

                        _line = new LineShape()
                        {
                            StartPoint = _points.Last(),
                            Point = context.GetNextPoint(x, y, false, 0.0),
                            Style = context.CurrentStyle
                        };
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

            switch (CurrentState)
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

            switch (CurrentState)
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

            CurrentState = State.Start;

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
