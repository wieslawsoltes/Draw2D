// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor.Tools
{
    public partial class PathTool : ToolBase
    {
        private ToolBase _currentSubTool;
        private ToolBase _previousSubTool;
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
                PreviousSubTool = _currentSubTool;
                Update(ref _currentSubTool, value);
            }
        }

        public ToolBase PreviousSubTool
        {
            get => _previousSubTool;
            set => Update(ref _previousSubTool, value);
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

            if (PreviousSubTool != null)
            {
                CurrentSubTool = PreviousSubTool;
            }
        }

        public void CleanSubTool(IToolContext context)
        {
            SetContext(context);
            CurrentSubTool?.Clean(this);
            SetContext(null);
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
            CurrentSubTool?.LeftDown(this, x, y, modifier);

            switch (CurrentSubTool)
            {
                case LineTool lineTool:
                    {
                        if (lineTool.CurrentState == LineTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            CurrentSubTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case CubicBezierTool cubicBezierTool:
                    {
                        if (cubicBezierTool.CurrentState == CubicBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            CurrentSubTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case QuadraticBezierTool quadraticBezierTool:
                    {
                        if (quadraticBezierTool.CurrentState == QuadraticBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            CurrentSubTool?.LeftDown(this, x, y, modifier);
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
            CurrentSubTool.Move(this, x, y, modifier);
            SetContext(null);
        }

        private void CleanInternal(IToolContext context)
        {
            CleanSubTool(context);

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.WorkingContainer.Shapes.Remove(_path);
                context.Selected.Remove(_path);
                PreviousSubTool = null;
                SetNextPoint(null);
                SetContext(null);
                _path = null;
                _figure = null;
            }
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            DownInternal(context, x, y, modifier);
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            this.Clean(context);
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            MoveInternal(context, x, y, modifier);
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
