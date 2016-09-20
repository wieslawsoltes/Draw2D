using System;
using Draw2D.Editor.Selection;
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
                        var result = SelectionHelper.TryToSelect(
                            context,
                            Settings.Mode, 
                            Settings.Targets, 
                            target, 
                            Settings.HitTestRadius);
                        if (result)
                        {
                            _haveSelection = true;
                            _state = State.Move;
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
                        var result = SelectionHelper.TryToSelect(
                            context, 
                            Settings.Mode, 
                            Settings.Targets, 
                            target, 
                            Settings.HitTestRadius);
                        if (result)
                        {
                            _haveSelection = true;
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
                            var target = new Point2(x, y);
                            SelectionHelper.TryToHover(
                                context, 
                                Settings.Mode, 
                                Settings.Targets, 
                                target, 
                                Settings.HitTestRadius);
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
