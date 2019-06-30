// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;
using Spatial.DouglasPeucker;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class ScribbleTool : BaseTool, ITool
    {
        private ScribbleToolSettings _settings;
        private PathShape _path = null;
        private FigureShape _figure = null;
        private IPointShape _previousPoint = null;
        private IPointShape _nextPoint = null;

        public enum State
        {
            Start,
            Points
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.Start;

        [IgnoreDataMember]
        public string Title => "Scribble";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ScribbleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _path = new PathShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                FillRule = Settings.FillRule,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };

            _figure = new FigureShape()
            {
                Shapes = new ObservableCollection<IBaseShape>(),
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };

            _path.Shapes.Add(_figure);

            _previousPoint = new PointShape(x, y, context.PointTemplate);
            _previousPoint.Owner = null;

            context.ContainerView?.WorkingContainer.Shapes.Add(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Points;
        }

        private void PointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.Start;

            if (Settings?.Simplify ?? true)
            {
                var points = new List<IPointShape>();
                _path.GetPoints(points);
                var distinct = new List<IPointShape>(points.Distinct());
                IList<Vector2> vectors = new List<Vector2>(distinct.Select(p => new Vector2((float)p.X, (float)p.Y)));
                int count = vectors.Count;
                RDP rdp = new RDP();
                BitArray accepted = rdp.DouglasPeucker(vectors, 0, count - 1, Settings?.Epsilon ?? 1.0);
                int removed = 0;
                for (int i = 0; i <= count - 1; ++i)
                {
                    if (!accepted[i])
                    {
                        distinct.RemoveAt(i - removed);
                        ++removed;
                    }
                }

                _figure.Shapes.Clear();
                _figure.MarkAsDirty(true);

                if (distinct.Count >= 2)
                {
                    for (int i = 0; i < distinct.Count - 1; i++)
                    {
                        var line = new LineShape()
                        {
                            Points = new ObservableCollection<IPointShape>(),
                            StartPoint = distinct[i],
                            Point = distinct[i + 1],
                            Text = new Text(),
                            StyleId = context.StyleLibrary?.CurrentItem?.Title
                        };
                        _figure.Shapes.Add(line);
                    }
                }
            }

            context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            if (_path.Validate(true) == true)
            {
                context.ContainerView?.CurrentContainer.Shapes.Add(_path);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _path = null;
            _figure = null;
            _previousPoint = null;
            _nextPoint = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _nextPoint = new PointShape(x, y, context.PointTemplate);

            var line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = _previousPoint,
                Point = _nextPoint,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };
            if (line.StartPoint.Owner == null)
            {
                line.StartPoint.Owner = line;
            }
            if (line.Point.Owner == null)
            {
                line.Point.Owner = line;
            }

            _figure.Shapes.Add(line);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);

            _previousPoint = _nextPoint;
            _nextPoint = null;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.Start;

            FiltersClear(context);

            if (_path != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _path = null;
                _figure = null;
                _previousPoint = null;
                _nextPoint = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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
