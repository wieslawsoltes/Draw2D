using System;
using System.Diagnostics;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public class SelectionTool : ToolBase
    {
        private enum State { TopLeft, BottomRight, Move };
        private State _state = State.TopLeft;
        private RectangleShape _rectangle;
        private double _startX;
        private double _startY;
        private bool _haveSelection;

        public override string Name { get { return "Selection"; } }

        public SelectionToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y)
        {
            base.LeftDown(context, x, y);

            switch (_state)
            {
                case State.TopLeft:
                    {
                        _startX = x;
                        _startY = y;

                        Filters.ForEach(f => f.Clear(context));
                        //Filters.Any(f => f.Process(context, ref _startX, ref _startY));

                        var target = new Point2(x, y);

                        var shapePoint = 
                            Settings.Mode.HasFlag(SelectionToolMode.Point) 
                            && Settings.Targets.HasFlag(SelectionToolTargets.Shapes) ?
                            context.HitTest.TryToGetPoint(context.Container.Shapes, target, Settings.HitTestRadius) : null;

                        var shape = 
                            Settings.Mode.HasFlag(SelectionToolMode.Shape) 
                            && Settings.Targets.HasFlag(SelectionToolTargets.Shapes) ?
                            context.HitTest.TryToGetShape(context.Container.Shapes, target, Settings.HitTestRadius) : null;

                        var guidePoint = 
                            Settings.Mode.HasFlag(SelectionToolMode.Point) 
                            && Settings.Targets.HasFlag(SelectionToolTargets.Guides) ?
                            context.HitTest.TryToGetPoint(context.Container.Guides, target, Settings.HitTestRadius) : null;

                        var guide = 
                            Settings.Mode.HasFlag(SelectionToolMode.Shape) 
                            && Settings.Targets.HasFlag(SelectionToolTargets.Guides) ?
                            context.HitTest.TryToGetShape(context.Container.Guides, target, Settings.HitTestRadius) : null;

                        if (shapePoint != null || shape != null || guidePoint != null || guide != null)
                        {
                            bool haveNewSelection = 
                                (shapePoint != null && !context.Renderer.Selected.Contains(shapePoint))
                                || (shape != null && !context.Renderer.Selected.Contains(shape))
                                || (guidePoint != null && !context.Renderer.Selected.Contains(guidePoint))
                                || (guide != null && !context.Renderer.Selected.Contains(guide));

                            if (context.Renderer.Selected.Count >= 1 && !haveNewSelection)
                            {
                                _haveSelection = true;
                                _state = State.Move;
                            }
                            else
                            {
                                if (shapePoint != null)
                                {
                                    _haveSelection = true;
                                    context.Renderer.Selected.Clear();
                                    Debug.WriteLine("Selected Shape Point: {0}", shapePoint.GetType());
                                    shapePoint.Select(context.Renderer.Selected);
                                    _state = State.Move;
                                }
                                else if (shape != null)
                                {
                                    _haveSelection = true;
                                    context.Renderer.Selected.Clear();
                                    Debug.WriteLine("Selected Shape: {0}", shape.GetType());
                                    shape.Select(context.Renderer.Selected);
                                    _state = State.Move;
                                }
                                else if (guidePoint != null)
                                {
                                    _haveSelection = true;
                                    context.Renderer.Selected.Clear();
                                    Debug.WriteLine("Selected Guide Point: {0}", guidePoint.GetType());
                                    guidePoint.Select(context.Renderer.Selected);
                                    _state = State.Move;
                                }
                                else if (guide != null)
                                {
                                    _haveSelection = true;
                                    context.Renderer.Selected.Clear();
                                    Debug.WriteLine("Selected Guide: {0}", guide.GetType());
                                    guide.Select(context.Renderer.Selected);
                                    _state = State.Move;
                                }
                            }
                        }
                        else
                        {
                            _haveSelection = false;
                            context.Renderer.Selected.Clear();

                            if (_rectangle == null)
                            {
                                _rectangle = new RectangleShape(new PointShape(), new PointShape());
                            }
                            _rectangle.TopLeft.X = x;
                            _rectangle.TopLeft.Y = y;
                            _rectangle.BottomRight.X = x;
                            _rectangle.BottomRight.Y = y;
                            _rectangle.Style = Settings.SelectionStyle;
                            context.WorkingContainer.Shapes.Add(_rectangle);
                            _state = State.BottomRight;
                        }
                    }
                    break;
                case State.BottomRight:
                    {
                        _state = State.TopLeft;
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                    }
                    break;
            }
        }

        public override void LeftUp(IToolContext context, double x, double y)
        {
            base.LeftUp(context, x, y);

            Filters.ForEach(f => f.Clear(context));

            switch (_state)
            {
                case State.BottomRight:
                    {
                        var target = Rect2.FromPoints(
                            _rectangle.TopLeft.X, 
                            _rectangle.TopLeft.Y, 
                            _rectangle.BottomRight.X, 
                            _rectangle.BottomRight.Y);

                        var shapes = 
                            Settings.Mode.HasFlag(SelectionToolMode.Shape) 
                            && Settings.Targets.HasFlag(SelectionToolTargets.Shapes) ?
                            context.HitTest.TryToGetShapes(context.Container.Shapes, target, Settings.HitTestRadius) : null;

                        var guides = 
                            Settings.Mode.HasFlag(SelectionToolMode.Shape) 
                            && Settings.Targets.HasFlag(SelectionToolTargets.Guides) ?
                            context.HitTest.TryToGetShapes(context.Container.Guides, target, Settings.HitTestRadius) : null;

                        if (shapes != null || guides != null)
                        {
                            if (shapes != null)
                            {
                                Debug.WriteLine("Selected Shapes: {0}", shapes != null ? shapes.Count : 0);
                                context.Renderer.Selected.Clear();
                                foreach (var shape in shapes)
                                {
                                    shape.Select(context.Renderer.Selected);
                                }
                                _haveSelection = true;
                            }
                            else if (guides != null)
                            {
                                Debug.WriteLine("Selected Guides: {0}", guides != null ? guides.Count : 0);
                                context.Renderer.Selected.Clear();
                                foreach (var guide in guides)
                                {
                                    guide.Select(context.Renderer.Selected);
                                }
                                _haveSelection = true;
                            }
                        }

                        context.WorkingContainer.Shapes.Remove(_rectangle);
                        _rectangle = null;
                        _state = State.TopLeft;
                    }
                    break;
                case State.Move:
                    {
                        _state = State.TopLeft;
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y)
        {
            base.RightDown(context, x, y);

            switch (_state)
            {
                case State.BottomRight:
                    {
                        context.WorkingContainer.Shapes.Remove(_rectangle);
                        _rectangle = null;
                        _state = State.TopLeft;
                    }
                    break;
                case State.Move:
                    {
                        _state = State.TopLeft;
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y)
        {
            base.Move(context, x, y);

            switch (_state)
            {
                case State.TopLeft:
                    {
                        if (!_haveSelection)
                        {
                            Hover(context, new Point2(x, y));
                        }
                    }
                    break;
                case State.BottomRight:
                    {
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                    }
                    break;
                case State.Move:
                    {
                        Filters.ForEach(f => f.Clear(context));
                        //Filters.Any(f => f.Process(context, ref x, ref y));

                        double dx = x - _startX;
                        double dy = y - _startY;
                        _startX = x;
                        _startY = y;

                        foreach (var shape in context.Renderer.Selected)
                        {
                            shape.Move(context.Renderer.Selected, dx, dy);
                        }
                    }
                    break;
            }
        }

        private void Hover(IToolContext context, Point2 target)
        {
            var shapePoint = 
                Settings.Mode.HasFlag(SelectionToolMode.Point) 
                && Settings.Targets.HasFlag(SelectionToolTargets.Shapes) ?
                context.HitTest.TryToGetPoint(context.Container.Shapes, target, Settings.HitTestRadius) : null;

            var shape = 
                Settings.Mode.HasFlag(SelectionToolMode.Shape) 
                && Settings.Targets.HasFlag(SelectionToolTargets.Shapes) ?
                context.HitTest.TryToGetShape(context.Container.Shapes, target, Settings.HitTestRadius) : null;

            var guidePoint = 
                Settings.Mode.HasFlag(SelectionToolMode.Point) 
                && Settings.Targets.HasFlag(SelectionToolTargets.Guides) ?
                context.HitTest.TryToGetPoint(context.Container.Guides, target, Settings.HitTestRadius) : null;

            var guide = 
                Settings.Mode.HasFlag(SelectionToolMode.Shape)
                && Settings.Targets.HasFlag(SelectionToolTargets.Guides) ?
                context.HitTest.TryToGetShape(context.Container.Guides, target, Settings.HitTestRadius) : null;

            if (shapePoint != null || shape != null || guide != null)
            {
                if (shapePoint != null)
                {
                    Debug.WriteLine("Hover Shape Point: {0}", shapePoint.GetType());
                    context.Renderer.Selected.Clear();
                    shapePoint.Select(context.Renderer.Selected);
                }
                else if (shape != null)
                {
                    Debug.WriteLine("Hover Shape: {0}", shape.GetType());
                    context.Renderer.Selected.Clear();
                    shape.Select(context.Renderer.Selected);
                }
                else if (guidePoint != null)
                {
                    Debug.WriteLine("Hover Guide Point: {0}", guidePoint.GetType());
                    context.Renderer.Selected.Clear();
                    guidePoint.Select(context.Renderer.Selected);
                }
                else if (guide != null)
                {
                    Debug.WriteLine("Hover Guide: {0}", guide.GetType());
                    context.Renderer.Selected.Clear();
                    guide.Select(context.Renderer.Selected);
                }
            }
            else
            {
                Debug.WriteLine("No Hover");
                context.Renderer.Selected.Clear();
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);
            _haveSelection = false;
            context.WorkingContainer.Shapes.Remove(_rectangle);
            context.Renderer.Selected.Clear();
            Filters.ForEach(f => f.Clear(context));
        }
    }
}
