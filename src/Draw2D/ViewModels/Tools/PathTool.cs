// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using PanAndZoom;

namespace Draw2D.ViewModels.Tools
{
    public class PathToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;
        private ObservableCollection<ITool> _tools;
        private ITool _currentTool;
        private ITool _previousTool;

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

        public ObservableCollection<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                PreviousTool = _currentTool;
                Update(ref _currentTool, value);
            }
        }

        public ITool PreviousTool
        {
            get => _previousTool;
            set => Update(ref _previousTool, value);
        }
    }

    public partial class PathTool : IToolContext
    {
        private IToolContext _context;
        private PointShape _nextPoint;

        public IShapeRenderer Renderer
        {
            get => _context.Renderer;
            set => SetRenderer(value);
        }

        public IHitTest HitTest
        {
            get => _context.HitTest;
            set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
        }

        public CanvasContainer CurrentContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {CurrentContainer} property value.");
        }

        public CanvasContainer WorkingContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {WorkingContainer} property value.");
        }

        public ShapeStyle CurrentStyle
        {
            get => _context.CurrentStyle;
            set => throw new InvalidOperationException($"Can not set {CurrentStyle} property value.");
        }

        public BaseShape PointShape
        {
            get => _context.PointShape;
            set => throw new InvalidOperationException($"Can not set {PointShape} property value.");
        }

        public Action Capture
        {
            get => _context.Capture;
            set => throw new InvalidOperationException($"Can not set {Capture} property value.");
        }

        public Action Release
        {
            get => _context.Release;
            set => throw new InvalidOperationException($"Can not set {Release} property value.");
        }

        public Action Invalidate
        {
            get => _context.Invalidate;
            set => throw new InvalidOperationException($"Can not set {Invalidate} property value.");
        }

        public IList<ITool> Tools
        {
            get => _context.Tools;
            set => throw new InvalidOperationException($"Can not set {Tools} property value.");
        }

        public ITool CurrentTool
        {
            get => _context.CurrentTool;
            set => throw new InvalidOperationException($"Can not set {CurrentTool} property value.");
        }

        public EditMode Mode
        {
            get => _context.Mode;
            set => throw new InvalidOperationException($"Can not set {Mode} property value.");
        }

        public ICanvasPresenter Presenter
        {
            get => _context.Presenter;
            set => throw new InvalidOperationException($"Can not set {Presenter} property value.");
        }

        public ISelection Selection
        {
            get => _context.Selection;
            set => throw new InvalidOperationException($"Can not set {Selection} property value.");
        }

        public IPanAndZoom Zoom
        {
            get => _context.Zoom;
            set => throw new InvalidOperationException($"Can not set {Zoom} property value.");
        }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            return _nextPoint ?? _context.GetNextPoint(x, y, connect, radius);
        }

        public void SetTool(string name)
        {
            _context.SetTool(name);
        }

        private void SetContext(IToolContext context)
        {
            _context = context;
        }

        private void SetRenderer(IShapeRenderer renderer)
        {
            if (_context != null)
            {
                _context.Renderer = renderer;
            }
        }

        private void SetNextPoint(PointShape point)
        {
            _nextPoint = point;
        }
    }

    public partial class PathTool : ViewModelBase, ITool
    {
        private PathShape _path;
        private FigureShape _figure;

        public string Title => "Path";

        public IList<PointIntersectionBase> Intersections { get; set; }

        public IList<PointFilterBase> Filters { get; set; }

        public PathToolSettings Settings { get; set; }

        public PointShape GetLastPoint()
        {
            if (_path?.Figures.Count > 0)
            {
                var shapes = _path.Figures[_path.Figures.Count - 1].Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[shapes.Count - 1])
                    {
                        case LineShape line:
                            return line.Point;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.Point3;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.Point2;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public void Create(IToolContext context)
        {
            _path = new PathShape()
            {
                FillRule = Settings.FillRule,
                Style = context.CurrentStyle
            };

            context.WorkingContainer.Shapes.Add(_path);
            context.Selection.Selected.Add(_path);
        }

        public void Move()
        {
            _figure = new FigureShape()
            {
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };
            _path.Figures.Add(_figure);

            if (Settings.PreviousTool != null)
            {
                Settings.CurrentTool = Settings.PreviousTool;
            }
        }

        public void CleanCurrentTool(IToolContext context)
        {
            SetContext(context);
            Settings.CurrentTool?.Clean(this);
            SetContext(null);
            UpdateCache(context);
        }

        public void UpdateCache(IToolContext context)
        {
            if (_path != null)
            {
                _figure.MarkAsDirty(true);
                _path.Invalidate(context.Renderer, 0.0, 0.0);
            }
        }

        private void DownInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            if (_path == null)
            {
                Create(context);
                Move();
            }

            SetContext(context);
            Settings.CurrentTool?.LeftDown(this, x, y, modifier);

            switch (Settings.CurrentTool)
            {
                case LineTool lineTool:
                    {
                        if (lineTool.CurrentState == LineTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case CubicBezierTool cubicBezierTool:
                    {
                        if (cubicBezierTool.CurrentState == CubicBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case QuadraticBezierTool quadraticBezierTool:
                    {
                        if (quadraticBezierTool.CurrentState == QuadraticBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
            }

            SetContext(null);
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            SetContext(context);
            Settings.CurrentTool.Move(this, x, y, modifier);
            SetContext(null);
        }

        private void CleanInternal(IToolContext context)
        {
            CleanCurrentTool(context);

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.WorkingContainer.Shapes.Remove(_path);
                context.Selection.Selected.Remove(_path);

                if (_path.Validate(true) == true)
                {
                    context.CurrentContainer.Shapes.Add(_path);
                }

                Settings.PreviousTool = null;
                SetNextPoint(null);
                SetContext(null);

                _path = null;
                _figure = null;
            }
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            DownInternal(context, x, y, modifier);
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            this.Clean(context);
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            MoveInternal(context, x, y, modifier);
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }
}
