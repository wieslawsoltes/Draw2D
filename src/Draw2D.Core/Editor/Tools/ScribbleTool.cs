using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;
using Draw2D.Spatial.DouglasPeucker;

namespace Draw2D.Editor.Tools
{
    public class ScribbleTool : ToolBase
    {
        private ScribbleShape _scribble = null;

        public enum State { Start, Points };
        public State CurrentState = State.Start;

        public override string Name { get { return "Scribble"; } }

        public ScribbleToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.Start:
                    {
                        _scribble = new ScribbleShape(
                            new PointShape(x, y, context.PointShape),
                            new ObservableCollection<PointShape>());
                        _scribble.Style = context.CurrentStyle;
                        context.WorkingContainer.Shapes.Add(_scribble);
                        CurrentState = State.Points;
                    }
                    break;
            }
        }

        public override void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Points:
                    {
                        CurrentState = State.Start;
                        if (Settings.Simplify)
                        {
                            List<Vector2> points = _scribble.Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToList();
                            int count = _scribble.Points.Count;
                            RDP rdp = new RDP();
                            BitArray accepted = rdp.DouglasPeucker(points, 0, count - 1, Settings.Epsilon);
                            int removed = 0;
                            for (int i = 0; i <= count - 1; ++i)
                            {
                                if (!accepted[i])
                                {
                                    _scribble.Points.RemoveAt(i - removed);
                                    ++removed;
                                }
                            }
                        }
                        context.WorkingContainer.Shapes.Remove(_scribble);
                        context.CurrentContainer.Shapes.Add(_scribble);
                        _scribble = null;
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Points:
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
                case State.Points:
                    {
                        _scribble.Points.Add(new PointShape(x, y, context.PointShape));
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.Start;

            Filters.ForEach(f => f.Clear(context));

            if (_scribble != null)
            {
                context.WorkingContainer.Shapes.Remove(_scribble);
                _scribble = null;
            }
        }
    }
}
