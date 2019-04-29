// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;
using Spatial.DouglasPeucker;

namespace Draw2D.ViewModels.Tools
{
    public class ScribbleToolSettings : SettingsBase
    {
        private bool _simplify;
        private double _epsilon;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;

        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }

        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }
    }

    public class ScribbleTool : ViewModelBase, ITool
    {
        private PathShape _path = null;
        private FigureShape _figure = null;
        private PointShape _previousPoint = null;
        private PointShape _nextPoint = null;

        public enum State
        {
            Start,
            Points
        }

        public State CurrentState { get; set; } = State.Start;

        public string Title => "Scribble";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public ScribbleToolSettings Settings { get; set; }

        private void StartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _path = new PathShape()
            {
                FillRule = Settings.FillRule,
                Style = context.CurrentStyle
            };

            _figure = new FigureShape()
            {
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };

            _path.Figures.Add(_figure);

            _previousPoint = new PointShape(x, y, context.PointShape);

            context.WorkingContainer.Shapes.Add(_path);

            context.InputService?.Capture?.Invoke();
            context.InputService?.Redraw?.Invoke();

            CurrentState = State.Points;
        }

        private void PointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.Start;

            if (Settings?.Simplify ?? true)
            {
                IList<PointShape> points = _path.GetPoints().Distinct().ToList();
                IList<Vector2> vectors = points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToList();
                int count = vectors.Count;
                RDP rdp = new RDP();
                BitArray accepted = rdp.DouglasPeucker(vectors, 0, count - 1, Settings?.Epsilon ?? 1.0);
                int removed = 0;
                for (int i = 0; i <= count - 1; ++i)
                {
                    if (!accepted[i])
                    {
                        points.RemoveAt(i - removed);
                        ++removed;
                    }
                }

                _figure.Shapes.Clear();
                _figure.MarkAsDirty(true);

                if (points.Count >= 2)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var line = new LineShape()
                        {
                            StartPoint = points[i],
                            Point = points[i + 1],
                            Style = context.CurrentStyle
                        };
                        _figure.Shapes.Add(line);
                    }
                }
            }

            context.WorkingContainer.Shapes.Remove(_path);

            if (_path.Validate(true) == true)
            {
                context.CurrentContainer.Shapes.Add(_path);
            }

            _path = null;
            _figure = null;
            _previousPoint = null;
            _nextPoint = null;

            Filters?.ForEach(f => f.Clear(context));

            context.InputService?.Release?.Invoke();
            context.InputService?.Redraw?.Invoke();
        }

        private void MoveStartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.InputService?.Redraw?.Invoke();
        }

        private void MovePointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _nextPoint = new PointShape(x, y, context.PointShape);

            var line = new LineShape()
            {
                StartPoint = _previousPoint,
                Point = _nextPoint,
                Style = context.CurrentStyle
            };

            _figure.Shapes.Add(line);

            _previousPoint = _nextPoint;
            _nextPoint = null;

            context.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.Start;

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.WorkingContainer.Shapes.Remove(_path);
                _path = null;
                _figure = null;
                _previousPoint = null;
                _nextPoint = null;
            }

            context.InputService?.Release?.Invoke();
            context.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Start:
                    {
                        StartInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Points:
                    {
                        PointsInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Points:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Start:
                    {
                        MoveStartInternal(context, x, y, modifier);
                    }
                    break;
                case State.Points:
                    {
                        MovePointsInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }
}
