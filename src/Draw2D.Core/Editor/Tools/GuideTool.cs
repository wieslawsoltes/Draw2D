// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Draw2D.Core.Shapes;

namespace Draw2D.Editor.Tools
{
    public class GuideTool : ToolBase
    {
        private LineShape _line = null;

        public enum State { Start, End };
        public State CurrentState = State.Start;

        public override string Name { get { return "Guide"; } }

        public GuideToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters?.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.Start:
                    {
                        _line = new LineShape()
                        {
                            StartPoint = new PointShape(x, y, null),
                            Point = new PointShape(x, y, null),
                            Style = Settings?.GuideStyle
                        };
                        context.WorkingContainer.Shapes.Add(_line);
                        CurrentState = State.End;
                    }
                    break;
                case State.End:
                    {
                        CurrentState = State.Start;
                        _line.Point.X = x;
                        _line.Point.Y = y;
                        context.WorkingContainer.Shapes.Remove(_line);
                        context.CurrentContainer.Guides.Add(_line);
                        _line = null;
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

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.End:
                    {
                        _line.Point.X = x;
                        _line.Point.Y = y;
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.Start;

            Filters?.ForEach(f => f.Clear(context));

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                _line = null;
            }
        }
    }
}
