using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
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
        public new string Title => "Scribble";

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
                FillType = Settings.FillType,
                Text = new Text(),
                StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
            };
            _path.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;

            _figure = new FigureShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };
            _figure.Owner = _path;

            _path.Shapes.Add(_figure);

            _previousPoint = new PointShape(x, y, context?.DocumentContainer?.PointTemplate);
            _previousPoint.Owner = null;

            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_path);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

            context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

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
                            StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
                        };
                        line.Owner = _figure;
                        line.StartPoint.Owner = line;
                        line.Point.Owner = line;
                        _figure.Shapes.Add(line);
                    }
                }
            }

            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_path);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

            if (_path.Validate(true) == true)
            {
                _path.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_path);
                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _path = null;
            _figure = null;
            _previousPoint = null;
            _nextPoint = null;

            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _nextPoint = new PointShape(x, y, context?.DocumentContainer?.PointTemplate);

            var line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = _previousPoint,
                Point = _nextPoint,
                Text = new Text(),
                StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
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
            context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);

            _previousPoint = _nextPoint;
            _nextPoint = null;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.Start;

            FiltersClear(context);

            if (_path != null)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_path);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _path = null;
                _figure = null;
                _previousPoint = null;
                _nextPoint = null;
            }

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
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
