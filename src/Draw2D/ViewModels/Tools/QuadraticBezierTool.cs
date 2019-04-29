// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    public class QuadraticBezierToolSettings : SettingsBase
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

    public class QuadraticBezierTool : ViewModelBase, ITool
    {
        private QuadraticBezierShape _quadraticBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2
        }

        public State CurrentState { get; set; } = State.StartPoint;

        public string Title => "QuadraticBezier";

        public QuadraticBezierToolSettings Settings { get; set; }

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.GetNextPoint(x, y, false, 0.0);
            _quadraticBezier = new QuadraticBezierShape()
            {
                StartPoint = next,
                Point1 = (PointShape)next.Copy(null),
                Point2 = (PointShape)next.Copy(null),
                Style = context.CurrentStyle
            };
            context.WorkingContainer.Shapes.Add(_quadraticBezier);
            context.Selection.Selected.Add(_quadraticBezier);
            context.Selection.Selected.Add(_quadraticBezier.StartPoint);
            context.Selection.Selected.Add(_quadraticBezier.Point1);
            context.Selection.Selected.Add(_quadraticBezier.Point2);

            context.ZoomControl?.Capture?.Invoke();
            context.ZoomControl?.Redraw?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.Selection.Selected.Remove(_quadraticBezier);
            context.Selection.Selected.Remove(_quadraticBezier.StartPoint);
            context.Selection.Selected.Remove(_quadraticBezier.Point1);
            context.Selection.Selected.Remove(_quadraticBezier.Point2);
            context.WorkingContainer.Shapes.Remove(_quadraticBezier);

            _quadraticBezier.Point1 = context.GetNextPoint(x, y, false, 0.0);
            context.CurrentContainer.Shapes.Add(_quadraticBezier);
            _quadraticBezier = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ZoomControl?.Release?.Invoke();
            context.ZoomControl?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.Selection.Selected.Remove(_quadraticBezier.Point2);
            _quadraticBezier.Point2 = context.GetNextPoint(x, y, false, 0.0);
            context.Selection.Selected.Add(_quadraticBezier.Point2);

            CurrentState = State.Point1;

            context.ZoomControl?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ZoomControl?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ZoomControl?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;
            _quadraticBezier.Point2.X = x;
            _quadraticBezier.Point2.Y = y;

            context.ZoomControl?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_quadraticBezier != null)
            {
                context.WorkingContainer.Shapes.Remove(_quadraticBezier);
                context.Selection.Selected.Remove(_quadraticBezier);
                context.Selection.Selected.Remove(_quadraticBezier.StartPoint);
                context.Selection.Selected.Remove(_quadraticBezier.Point1);
                context.Selection.Selected.Remove(_quadraticBezier.Point2);
                _quadraticBezier = null;
            }

            context.ZoomControl?.Release?.Invoke();
            context.ZoomControl?.Redraw?.Invoke();
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
                case State.Point1:
                    {
                        Point1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        Point2Internal(context, x, y, modifier);
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
                case State.Point1:
                case State.Point2:
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
                case State.Point1:
                    {
                        MovePoint1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        MovePoint2Internal(context, x, y, modifier);
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
