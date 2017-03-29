using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Draw2D.Models;
using Draw2D.Models.Shapes;

namespace PathDemo.Tools
{
    public class PathTool : ToolBase, IToolContext
    {
        private IToolContext _context;

        public ISet<ShapeObject> Selected
        {
            get => _context?.Selected;
            set => _context?.Selected = value ?? throw new Exception("Context is not set.");
        }

        public ObservableCollection<ShapeObject> Shapes
        {
            get => Figure?.Segments ?? _context?.Shapes;
            set => Figure?.Segments = value ?? _context?.Shapes = value ?? throw new Exception("Context is not set.");
        }

        public ObservableCollection<ToolBase> SubTools { get; set; }

        private ToolBase _currentSubTool;

        public ToolBase CurrentSubTool
        {
            get { return _currentSubTool; }
            set
            {
                if (_currentSubTool is MoveTool)
                {
                    PreviousSubTool = _currentSubTool;
                }
                _currentSubTool = value;
            }
        }

        public ToolBase PreviousSubTool;
        public PointShape NextPoint;
        public PathShape Path;
        public FigureShape Figure;

        public PathTool()
        {
            SubTools = new ObservableCollection<ToolBase>
            {
                new LineTool() { Name = "Line" },
                new CubicBezierTool() { Name = "CubicBezier" },
                new QuadraticBezierTool() { Name = "QuadraticBezier" },
                new MoveTool(this) { Name = "Move" }
            };
            CurrentSubTool = SubTools[0];
        }

        public PointShape GetNextPoint(Point point)
        {
            if (_context != null)
            {
                return NextPoint ?? _context.GetNextPoint(point);
            }
            else
            {
                throw new Exception("Context is not set.");
            }
        }

        public void Capture()
        {
            _context?.Capture() ?? throw new Exception("Context is not set.");
        }

        public void Release()
        {
            _context?.Release() ?? throw new Exception("Context is not set.")
        }

        public void Invalidate()
        {
            _context?.Invalidate() ?? throw new Exception("Context is not set.");
        }

        private PointShape GetLastPoint()
        {
            var segments = Path.Figures[Path.Figures.Count - 1].Segments;
            var lastSegment = segments[segments.Count - 1];
            if (lastSegment is LineShape line)
            {
                return line.Point;
            }
            else if (lastSegment is CubicBezierShape cubicBezier)
            {
                return cubicBezier.Point3;
            }
            else if (lastSegment is QuadraticBezierShape quadraticBezier)
            {
                return quadraticBezier.Point2;
            }
            throw new Exception("Could not find last path point.");
        }

        public void NewPath(IToolContext context)
        {
            Path = new PathShape()
            {
                Figures = new ObservableCollection<FigureShape>(),
                FillRule = PathFillRule.EvenOdd
            };

            context.Shapes.Add(Path);
            context.Selected.Add(Path);
        }

        public void NewFigure()
        {
            Figure = new FigureShape()
            {
                Segments = new ObservableCollection<ShapeObject>(),
                IsFilled = true,
                IsClosed = true
            };

            Path.Figures.Add(Figure);
        }

        public override void LeftDown(IToolContext context, Point point)
        {
            if (Path == null)
            {
                NewPath(context);
                NewFigure();
            }

            _context = context;

            CurrentSubTool.LeftDown(this, point);

            if (CurrentSubTool is LineTool lineTool)
            {
                if (lineTool.CurrentState == LineTool.LineToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, point);
                    NextPoint = null;
                }
            }
            else if (CurrentSubTool is CubicBezierTool cubicBezierTool)
            {
                if (cubicBezierTool.CurrentState == CubicBezierTool.CubicBezierToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, point);
                    NextPoint = null;
                }
            }
            else if (CurrentSubTool is QuadraticBezierTool quadraticBezierTool)
            {
                if (quadraticBezierTool.CurrentState == QuadraticBezierTool.QuadraticBezierToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, point);
                    NextPoint = null;
                }
            }

            _context = null;
        }

        public override void RightDown(IToolContext context, Point point)
        {
            _context = context;

            CurrentSubTool.RightDown(this, point);

            _context = null;

            context.Selected.Remove(Path);
            Path = null;
            Figure = null;
        }

        public override void Move(IToolContext context, Point point)
        {
            _context = context;

            CurrentSubTool.Move(this, point);

            _context = null;
        }
    }
}
