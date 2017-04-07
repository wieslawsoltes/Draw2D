// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Core.Containers;
using Draw2D.Core.Editor.Bounds;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Editor.Tools
{
    public partial class PathTool : IToolContext
    {
        public ShapeRenderer Renderer
        {
            get => _context?.Renderer;
            set
            {
                if (_context != null)
                {
                    _context.Renderer = value;
                }
            }
        }

        public ISet<ShapeObject> Selected
        {
            get => _context?.Selected;
            set
            {
                if (_context != null)
                {
                    _context.Selected = value;
                }
            }
        }

        public IShapesContainer CurrentContainer
        {
            get => _figure;
            set => throw new InvalidCastException("Can't cast current container as a figure.");
        }

        public IShapesContainer WorkingContainer
        {
            get => _figure;
            set => throw new InvalidCastException("Can't cast current container as a figure.");
        }

        public DrawStyle CurrentStyle { get; set; }

        public ShapeObject PointShape { get; set; }

        public IHitTest HitTest { get; set; }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            return _nextPoint ?? _context?.GetNextPoint(x, y, connect, radius);
        }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }
    }

    public partial class PathTool : ToolBase
    {
        private ToolBase _currentSubTool;
        private ToolBase _previousSubTool;
        private PointShape _nextPoint;
        private IToolContext _context;
        private PathShape _path;
        private FigureShape _figure;

        public override string Name { get { return "Path"; } }

        public PathToolSettings Settings { get; set; }

        public ObservableCollection<ToolBase> SubTools { get; set; }

        public ToolBase CurrentSubTool
        {
            get => _currentSubTool;
            set
            {
                _previousSubTool = _currentSubTool;
                _currentSubTool = value;
            }
        }

        public PathTool()
        {
            Capture = () => _context?.Capture();
            Release = () => _context?.Release();
            Invalidate = () => _context?.Invalidate();

            SubTools = new ObservableCollection<ToolBase>
            {
                new LineTool(),
                new CubicBezierTool(),
                new QuadraticBezierTool(),
                new MoveTool(this)
            };

            CurrentSubTool = SubTools[0];
        }

        public PointShape GetLastPoint()
        {
            if (_path.Figures.Count > 0)
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
                FillRule = PathFillRule.EvenOdd,
                Style = context.CurrentStyle
            };

            context.CurrentContainer.Shapes.Add(_path);
            context.Selected.Add(_path);
        }

        public void Move()
        {
            _figure = new FigureShape()
            {
                IsFilled = true,
                IsClosed = true
            };
            _path.Figures.Add(_figure);

            if (_previousSubTool != null)
            {
                CurrentSubTool = _previousSubTool;
            }
        }

        private void SetCurrentContext(IToolContext context) => _context = context;

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters?.Any(f => f.Process(context, ref x, ref y));

            if (_path == null)
            {
                Create(context);
                Move();
            }

            SetCurrentContext(context);
            CurrentSubTool.LeftDown(this, x, y, modifier);

            switch (CurrentSubTool)
            {
                case LineTool lineTool:
                    {
                        if (lineTool.CurrentState == LineTool.State.StartPoint)
                        {
                            _nextPoint = GetLastPoint();
                            CurrentSubTool.LeftDown(this, x, y, modifier);
                            _nextPoint = null;
                        }
                    }
                    break;
                case CubicBezierTool cubicBezierTool:
                    {
                        if (cubicBezierTool.CurrentState == CubicBezierTool.State.StartPoint)
                        {
                            _nextPoint = GetLastPoint();
                            CurrentSubTool.LeftDown(this, x, y, modifier);
                            _nextPoint = null;
                        }
                    }
                    break;
                case QuadraticBezierTool quadraticBezierTool:
                    {
                        if (quadraticBezierTool.CurrentState == QuadraticBezierTool.State.StartPoint)
                        {
                            _nextPoint = GetLastPoint();
                            CurrentSubTool.LeftDown(this, x, y, modifier);
                            _nextPoint = null;
                        }
                    }
                    break;
            }

            SetCurrentContext(null);
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            Clean(context);
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            SetCurrentContext(context);
            CurrentSubTool.Move(this, x, y, modifier);
            SetCurrentContext(null);
        }

        public void CleanSubTool(IToolContext context)
        {
            SetCurrentContext(context);
            CurrentSubTool.Clean(this);
            SetCurrentContext(null);
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanSubTool(context);

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.WorkingContainer.Shapes.Remove(_path);
                context.Selected.Remove(_path);
                _previousSubTool = null;
                _nextPoint = null;
                _context = null;
                _path = null;
                _figure = null;
            }
        }
    }
}
