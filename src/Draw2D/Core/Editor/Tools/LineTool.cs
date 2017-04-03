// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Draw2D.Core.Editor.Intersections;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor.Tools
{
    public class LineTool : ToolBase
    {
        private LineShape _line = null;

        public enum State { StartPoint, Point };
        public State CurrentState = State.StartPoint;

        public override string Name { get { return "Line"; } }

        public LineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters?.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        var next = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
                        _line = new LineShape()
                        {
                            StartPoint = next,
                            Point = next.Clone(),
                            Style = context.CurrentStyle
                        };
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Selected.Add(_line);
                        context.Capture();
                        context.Invalidate();
                        //context.Selected.Add(_line.StartPoint);
                        //context.Selected.Add(_line.Point);
                        CurrentState = State.Point;
                    }
                    break;
                case State.Point:
                    {
                        CurrentState = State.StartPoint;

                        //context.Selected.Remove(_line.Point);
                        _line.Point = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

                        context.WorkingContainer.Shapes.Remove(_line);
                        context.Selected.Remove(_line);
                        //context.Selected.Remove(_line.StartPoint);
                        //context.Selected.Remove(_line.Point);

                        Intersections?.ForEach(i => i.Clear(context));
                        Intersections?.ForEach(i => i.Find(context, _line));

                        if ((Settings?.SplitIntersections ?? false) && (Intersections?.Any(i => i.Intersections.Count > 0) ?? false))
                        {
                            LineHelper.SplitByIntersections(context, Intersections, _line);
                        }
                        else
                        {
                            context.CurrentContainer.Shapes.Add(_line);
                        }

                        _line = null;
                        Intersections?.ForEach(i => i.Clear(context));
                        context.Release();
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
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

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.Point:
                    {
                        _line.Point.X = x;
                        _line.Point.Y = y;
                        Intersections?.ForEach(i => i.Clear(context));
                        Intersections?.ForEach(i => i.Find(context, _line));
                        context.Invalidate();
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.StartPoint;

            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                context.Selected.Remove(_line);
                //context.Selected.Remove(_line.StartPoint);
                //context.Selected.Remove(_line.Point);
                _line = null;
            }

            context.Release();
            context.Invalidate();
        }
    }
}
