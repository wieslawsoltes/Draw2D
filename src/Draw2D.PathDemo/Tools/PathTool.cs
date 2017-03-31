using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Editor;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.PathDemo.Tools
{
    public class PathTool : ToolBase, IToolContext
    {
        public override string Name { get { return "Path"; } }

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

        public IShapesContainer CurrentContainer
        {
            get => Figure;
            set => throw new InvalidCastException("Can't cast current container as a figure.");
        }

        public IShapesContainer WorkingContainer { get; set; }

        public DrawStyle CurrentStyle { get; set; }

        public ShapeObject PointShape { get; set; }

        public HitTest HitTest { get; set; }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius) 
        {
            return NextPoint ?? Context?.GetNextPoint(x, y, connect, radius);
        }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }

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
            Capture = () => Context?.Capture();
            Release = () => Context?.Release();
            Invalidate = () => Context?.Invalidate();

            SubTools = new ObservableCollection<ToolBase>
            {
                new LineTool(),
                new CubicBezierTool(),
                new QuadraticBezierTool(),
                new MoveTool(this)
            };

            CurrentSubTool = SubTools[0];
        }

        private PointShape GetLastPoint()
        {
            if (Path.Figures.Count > 0)
            {
                var shapes = Path.Figures[Path.Figures.Count - 1].Shapes;
                if (shapes.Count > 0)
                {
                    var shape = shapes[shapes.Count - 1];
                    if (shape is LineShape line)
                    {
                        return line.Point;
                    }
                    else if (shape is CubicBezierShape cubicBezier)
                    {
                        return cubicBezier.Point3;
                    }
                    else if (shape is QuadraticBezierShape quadraticBezier)
                    {
                        return quadraticBezier.Point2;
                    }
                    throw new Exception("Could not find last path point.");
                } 
            }
            return null;
        }

        public void NewPath(IToolContext context)
        {
            Path = new PathShape()
            {
                Figures = new ObservableCollection<FigureShape>(),
                FillRule = PathFillRule.EvenOdd,
                Style = context.CurrentStyle
            };
            context.CurrentContainer.Shapes.Add(Path);
            context.Selected.Add(Path);
        }

        public void NewFigure()
        {
            Figure = new FigureShape()
            {
                Shapes = new ObservableCollection<ShapeObject>(),
                IsFilled = true,
                IsClosed = true
            };
            Path.Figures.Add(Figure);
        }

        private void SetCurrentContext(IToolContext context) => Context = context;

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            if (Path == null)
            {
                NewPath(context);
                NewFigure();
            }

            SetCurrentContext(context);

            CurrentSubTool.LeftDown(this, x, y, modifier);

            if (CurrentSubTool is LineTool lineTool)
            {
                if (lineTool.CurrentState == LineTool.LineToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, x, y, modifier);
                    NextPoint = null;
                }
            }
            else if (CurrentSubTool is CubicBezierTool cubicBezierTool)
            {
                if (cubicBezierTool.CurrentState == CubicBezierTool.CubicBezierToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, x, y, modifier);
                    NextPoint = null;
                }
            }
            else if (CurrentSubTool is QuadraticBezierTool quadraticBezierTool)
            {
                if (quadraticBezierTool.CurrentState == QuadraticBezierTool.QuadraticBezierToolState.StartPoint)
                {
                    NextPoint = GetLastPoint();
                    CurrentSubTool.LeftDown(this, x, y, modifier);
                    NextPoint = null;
                }
            }

            SetCurrentContext(null);
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            SetCurrentContext(context);
            CurrentSubTool.RightDown(this, x, y, modifier);
            SetCurrentContext(null);
            context.Selected.Remove(Path);
            Path = null;
            Figure = null;
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            SetCurrentContext(context);
            CurrentSubTool.Move(this, x, y, modifier);
            SetCurrentContext(null);
        }
    }
}
