// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Spatial;

namespace Draw2D.ViewModels.Tools
{
    [Flags]
    public enum SelectionMode
    {
        None = 0,
        Point = 1,
        Shape = 2
    }

    [Flags]
    public enum SelectionTargets
    {
        None = 0,
        Shapes = 1,
        Guides = 2
    }

    public class SelectionToolSettings : SettingsBase
    {
        private SelectionMode _mode;
        private SelectionTargets _targets;
        private Modifier _selectionModifier;
        private Modifier _connectionModifier;
        private ShapeStyle _selectionStyle;
        private bool _clearSelectionOnClean;
        private double _hitTestRadius;
        private bool _connectPoints;
        private double _connectTestRadius;
        private bool _disconnectPoints;
        private double _disconnectTestRadius;

        public SelectionMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public SelectionTargets Targets
        {
            get => _targets;
            set => Update(ref _targets, value);
        }

        public Modifier SelectionModifier
        {
            get => _selectionModifier;
            set => Update(ref _selectionModifier, value);
        }

        public Modifier ConnectionModifier
        {
            get => _connectionModifier;
            set => Update(ref _connectionModifier, value);
        }

        public ShapeStyle SelectionStyle
        {
            get => _selectionStyle;
            set => Update(ref _selectionStyle, value);
        }

        public bool ClearSelectionOnClean
        {
            get => _clearSelectionOnClean;
            set => Update(ref _clearSelectionOnClean, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double ConnectTestRadius
        {
            get => _connectTestRadius;
            set => Update(ref _connectTestRadius, value);
        }

        public bool DisconnectPoints
        {
            get => _disconnectPoints;
            set => Update(ref _disconnectPoints, value);
        }

        public double DisconnectTestRadius
        {
            get => _disconnectTestRadius;
            set => Update(ref _disconnectTestRadius, value);
        }
    }

    public static class SelectHelper
    {
        public static BaseShape TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Shapes, target, radius, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Shapes, target, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Guides, target, radius, null) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Guides, target, radius) : null;

            if (shapePoint != null || shape != null || guide != null)
            {
                if (shapePoint != null)
                {
                    return shapePoint;
                }
                else if (shape != null)
                {
                    return shape;
                }
                else if (guidePoint != null)
                {
                    return guidePoint;
                }
                else if (guide != null)
                {
                    return guide;
                }
            }

            return null;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Point2 point, double radius, Modifier modifier)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Shapes, point, radius, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Shapes, point, radius) : null;

            var guidePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetPoint(context.CurrentContainer.Guides, point, radius, null) : null;

            var guide =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShape(context.CurrentContainer.Guides, point, radius) : null;

            if (shapePoint != null || shape != null || guidePoint != null || guide != null)
            {
                bool haveNewSelection =
                    (shapePoint != null && !context.Renderer.Selected.Contains(shapePoint))
                    || (shape != null && !context.Renderer.Selected.Contains(shape))
                    || (guidePoint != null && !context.Renderer.Selected.Contains(guidePoint))
                    || (guide != null && !context.Renderer.Selected.Contains(guide));

                if (context.Renderer.Selected.Count >= 1
                    && !haveNewSelection
                    && !modifier.HasFlag(selectionModifier))
                {
                    return true;
                }
                else
                {
                    if (shapePoint != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(shapePoint))
                            {
                                shapePoint.Deselect(context.Renderer);
                            }
                            else
                            {
                                shapePoint.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            shapePoint.Select(context.Renderer);
                            return true;
                        }
                    }
                    else if (shape != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(shape))
                            {
                                shape.Deselect(context.Renderer);
                            }
                            else
                            {
                                shape.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            shape.Select(context.Renderer);
                            return true;
                        }
                    }
                    else if (guidePoint != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(guidePoint))
                            {
                                guidePoint.Deselect(context.Renderer);
                            }
                            else
                            {
                                guidePoint.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            guidePoint.Select(context.Renderer);
                            return true;
                        }
                    }
                    else if (guide != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            if (context.Renderer.Selected.Contains(guide))
                            {
                                guide.Deselect(context.Renderer);
                            }
                            else
                            {
                                guide.Select(context.Renderer);
                            }
                            return context.Renderer.Selected.Count > 0;
                        }
                        else
                        {
                            context.Renderer.Selected.Clear();
                            guide.Select(context.Renderer);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Rect2 rect, double radius, Modifier modifier)
        {
            var shapes =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShapes(context.CurrentContainer.Shapes, rect, radius) : null;

            var guides =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Guides) ?
                context.HitTest?.TryToGetShapes(context.CurrentContainer.Guides, rect, radius) : null;

            if (shapes != null || guides != null)
            {
                if (shapes != null)
                {
                    if (modifier.HasFlag(selectionModifier))
                    {
                        foreach (var shape in shapes)
                        {
                            if (context.Renderer.Selected.Contains(shape))
                            {
                                shape.Deselect(context.Renderer);
                            }
                            else
                            {
                                shape.Select(context.Renderer);
                            }
                        }
                        return context.Renderer.Selected.Count > 0;
                    }
                    else
                    {
                        context.Renderer.Selected.Clear();
                        foreach (var shape in shapes)
                        {
                            shape.Select(context.Renderer);
                        }
                        return true;
                    }
                }
                else if (guides != null)
                {
                    if (modifier.HasFlag(selectionModifier))
                    {
                        foreach (var guide in guides)
                        {
                            if (context.Renderer.Selected.Contains(guide))
                            {
                                guide.Deselect(context.Renderer);
                            }
                            else
                            {
                                guide.Select(context.Renderer);
                            }
                        }
                        return context.Renderer.Selected.Count > 0;
                    }
                    else
                    {
                        context.Renderer.Selected.Clear();
                        foreach (var guide in guides)
                        {
                            guide.Select(context.Renderer);
                        }
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public partial class SelectionTool : ToolBase
    {
        private RectangleShape _rectangle;
        private double _originX;
        private double _originY;
        private double _previousX;
        private double _previousY;

        public enum State
        {
            None,
            Selection,
            Move
        }

        public State CurrentState { get; set; } = State.None;

        public override string Title => "Selection";

        public SelectionToolSettings Settings { get; set; }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _disconnected = false;

            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            DeHover(context);

            var selected = SelectHelper.TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0,
                modifier);
            if (selected == true)
            {
                context.Capture?.Invoke();

                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Settings?.SelectionModifier ?? Modifier.Control))
                {
                    context.Renderer.Selected.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        TopLeft = new PointShape(),
                        BottomRight = new PointShape()
                    };
                }

                _rectangle.TopLeft.X = x;
                _rectangle.TopLeft.Y = y;
                _rectangle.BottomRight.X = x;
                _rectangle.BottomRight.Y = y;
                _rectangle.Style = Settings?.SelectionStyle;
                context.WorkingContainer.Shapes.Add(_rectangle);

                context.Capture?.Invoke();
                context.Invalidate?.Invoke();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            DeHover(context);

            SelectHelper.TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                _rectangle.ToRect2(),
                Settings?.HitTestRadius ?? 7.0,
                modifier);

            context.WorkingContainer.Shapes.Remove(_rectangle);
            _rectangle = null;

            CurrentState = State.None;

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (!(_hover == null && context.Renderer.Selected.Count > 0))
            {
                lock (context.Renderer.Selected)
                {
                    var previous = _hover;

                    DeHover(context);

                    var target = new Point2(x, y);
                    var shape = SelectHelper.TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        Settings?.HitTestRadius ?? 7.0);
                    if (shape != null)
                    {
                        Hover(context, shape);
                        context.Invalidate?.Invoke();
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.Invalidate?.Invoke();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.Invalidate?.Invoke();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            double dx = x - _previousX;
            double dy = y - _previousY;

            _previousX = x;
            _previousY = y;

            if (context.Renderer.Selected.Count == 1)
            {
                var shape = context.Renderer.Selected.FirstOrDefault();

                if (shape is PointShape source)
                {
                    if (Settings.ConnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                    {
                        Connect(context, source);
                    }

                    if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                    {
                        if (_disconnected == false)
                        {
                            double treshold = Settings.DisconnectTestRadius;
                            double tx = Math.Abs(_originX - source.X);
                            double ty = Math.Abs(_originY - source.Y);
                            if (tx > treshold || ty > treshold)
                            {
                                Disconnect(context, source);
                            }
                        }
                    }
                }

                shape.Move(context.Renderer, dx, dy);
            }
            else
            {
                foreach (var shape in context.Renderer.Selected.ToList())
                {
                    if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                    {
                        if (!(shape is PointShape) && _disconnected == false)
                        {
                            Disconnect(context, shape);
                        }
                    }
                }

                foreach (var shape in context.Renderer.Selected.ToList())
                {
                    shape.Move(context.Renderer, dx, dy);
                }
            }

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;

            DeHover(context);

            if (_rectangle != null)
            {
                context.WorkingContainer.Shapes.Remove(_rectangle);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.Renderer.Hover = null;
                context.Renderer.Selected.Clear();
            }

            Filters?.ForEach(f => f.Clear(context));

            context.Release?.Invoke();
            context.Invalidate?.Invoke();
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.None:
                    {
                        LeftDownNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        LeftDownSelectionInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftUp(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Selection:
                    {
                        LeftUpSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        LeftUpMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.Selection:
                    {
                        this.Clean(context);
                    }
                    break;
                case State.Move:
                    {
                        RightDownMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.None:
                    {
                        MoveNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        MoveSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        MoveMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
