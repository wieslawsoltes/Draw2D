using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace PathDemo
{
    public class PathTool : ToolBase, IToolContext
    {
        private IToolContext _context;

        public ObservableCollection<ShapeBase> Shapes
        {
            get
            {
                if (_context != null)
                {
                    return Figure != null ? Figure.Segments : _context.Shapes;
                }
                else
                {
                    throw new Exception("Context is not set.");
                }
            }
            set
            {
                if (_context != null)
                {
                    if (Figure != null)
                    {
                        Figure.Segments = value;
                    }
                    else
                    {
                        _context.Shapes = value;
                    }
                }
                else
                {
                    throw new Exception("Context is not set.");
                }
            }
        }

        public HashSet<ShapeBase> Selected
        {
            get
            {
                if (_context != null)
                {
                    return _context.Selected;
                }
                else
                {
                    throw new Exception("Context is not set.");
                }
            }
            set
            {
                if (_context != null)
                {
                    _context.Selected = value;
                }
                else
                {
                    throw new Exception("Context is not set.");
                }
            }
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
            SubTools = new ObservableCollection<ToolBase>();
            SubTools.Add(new LineTool() { Name = "Line" });
            SubTools.Add(new CubicBezierTool() { Name = "CubicBezier" });
            SubTools.Add(new QuadraticBezierTool() { Name = "QuadraticBezier" });
            SubTools.Add(new MoveTool(this) { Name = "Move" });
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
            if (_context != null)
            {
                _context.Capture();
            }
            else
            {
                throw new Exception("Context is not set.");
            }
        }

        public void Release()
        {
            if (_context != null)
            {
                _context.Release();
            }
            else
            {
                throw new Exception("Context is not set.");
            }
        }

        public void Invalidate()
        {
            if (_context != null)
            {
                _context.Invalidate();
            }
            else
            {
                throw new Exception("Context is not set.");
            }
        }

        private PointShape GetLastPoint()
        {
            var segments = Path.Figures[Path.Figures.Count - 1].Segments;
            var lastSegment = segments[segments.Count - 1];
            if (lastSegment is LineShape)
            {
                var line = lastSegment as LineShape;
                return line.Point;
            }
            else if (lastSegment is CubicBezierShape)
            {
                var cubicBezier = lastSegment as CubicBezierShape;
                return cubicBezier.Point3;
            }
            else if (lastSegment is QuadraticBezierShape)
            {
                var quadraticBezier = lastSegment as QuadraticBezierShape;
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
                Segments = new ObservableCollection<ShapeBase>(),
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

            if (CurrentSubTool is LineTool)
            {
                var lineTool = CurrentSubTool as LineTool;
                if (lineTool.CurrentState == LineTool.LineToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, point);
                    NextPoint = null;
                }
            }
            else if (CurrentSubTool is CubicBezierTool)
            {
                var cubicBezierTool = CurrentSubTool as CubicBezierTool;
                if (cubicBezierTool.CurrentState == CubicBezierTool.CubicBezierToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, point);
                    NextPoint = null;
                }
            }
            else if (CurrentSubTool is QuadraticBezierTool)
            {
                var quadraticBezierTool = CurrentSubTool as QuadraticBezierTool;
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
