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
        private ToolBase _currentSubTool;

        public ISet<ShapeObject> Selected
        {
            get => Context?.Selected;
            set
            { 
                if (Context != null) 
                {
                    Context.Selected = value;
                }
            }
        }

        public ObservableCollection<ShapeObject> Shapes
        {
            get => Figure?.Segments ?? Context?.Shapes;
            set
            { 
                if (Figure != null) 
                {
                    Figure.Segments = value;
                }
                else if (Context != null)
                {
                    Context.Shapes = value;
                }
            }
        }

        public ObservableCollection<ToolBase> SubTools { get; set; }

        public ToolBase CurrentSubTool
        {
            get => _currentSubTool;
            set
            {
                if (_currentSubTool is MoveTool)
                {
                    PreviousSubTool = _currentSubTool;
                }
                _currentSubTool = value;
            }
        }

        public ToolBase PreviousSubTool { get; set; }

        public PointShape NextPoint { get; set; }

        public PathShape Path { get; set; }

        public FigureShape Figure { get; set; }

        private IToolContext Context { get; set; }

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

        public PointShape GetNextPoint(double x, double y) => NextPoint ?? Context?.GetNextPoint(x, y);

        public void Capture() => Context?.Capture();

        public void Release() => Context?.Release();

        public void Invalidate() => Context?.Invalidate();

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

        private void SetCurrentContext(IToolContext context) => Context = context;

        public override void LeftDown(IToolContext context, Point point)
        {
            if (Path == null)
            {
                NewPath(context);
                NewFigure();
            }

            SetCurrentContext(context);

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

            SetCurrentContext(null);
        }

        public override void RightDown(IToolContext context, Point point)
        {
            SetCurrentContext(context);
            CurrentSubTool.RightDown(this, point);
            SetCurrentContext(null);
            context.Selected.Remove(Path);
            Path = null;
            Figure = null;
        }

        public override void Move(IToolContext context, Point point)
        {
            SetCurrentContext(context);
            CurrentSubTool.Move(this, point);
            SetCurrentContext(null);
        }
    }
}
