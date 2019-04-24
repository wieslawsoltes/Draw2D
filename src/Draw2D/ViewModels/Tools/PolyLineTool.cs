// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    public class PolyLineToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    public class PolyLineTool : ViewModelBase, ITool
    {
        private LineShape _line = null;
        private IList<PointShape> _points = null;

        public enum State
        {
            StartPoint,
            Point
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public string Title => "PolyLine";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public PolyLineToolSettings Settings { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _points = new ObservableCollection<PointShape>();
            _line = new LineShape()
            {
                StartPoint = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                Point = context.GetNextPoint(x, y, false, 0.0),
                Style = context?.CurrentStyle
            };
            _points.Add(_line.StartPoint);
            _points.Add(_line.Point);
            context.WorkingContainer.Shapes.Add(_line);
            context.Selection.Selected.Add(_line);
            context.Selection.Selected.Add(_line.StartPoint);
            context.Selection.Selected.Add(_line.Point);

            context.Capture?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Selection.Selected.Remove(_line);
            context.Selection.Selected.Remove(_line.Point);
            _line.Point = context.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _points[_points.Count - 1] = _line.Point;

            if (!context.Selection.Selected.Contains(_line.Point))
            {
                context.Selection.Selected.Add(_line.Point);
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
            context.Selection.Selected.Add(_line);
            context.Selection.Selected.Add(_line.Point);

            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            context.Invalidate?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _line.Point.X = x;
            _line.Point.Y = y;

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.WorkingContainer.Shapes.Remove(_line);
                context.Selection.Selected.Remove(_line);
                _line = null;
            }

            if (_points != null)
            {
                _points.ForEach(point => context.Selection.Selected.Remove(point));
                _points = null;
            }

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point:
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
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
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
