﻿using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public class RectangleTool : BaseTool, ITool
{
    private RectangleToolSettings _settings;
    private RectangleShape _rectangle = null;

    public enum State
    {
        StartPoint,
        Point
    }

    [IgnoreDataMember]
    public State CurrentState { get; set; } = State.StartPoint;

    [IgnoreDataMember]
    public new string Title => "Rectangle";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RectangleToolSettings Settings
    {
        get => _settings;
        set => Update(ref _settings, value);
    }

    private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        var radius = Settings?.HitTestRadius ?? 7.0;
        var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

        IPointShape startPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);
        IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

        _rectangle = new RectangleShape()
        {
            Points = new ObservableCollection<IPointShape>(),
            StartPoint = startPoint,
            Point = point,
            Text = new Text(),
            RadiusX = Settings?.RadiusX ?? 0.0,
            RadiusY = Settings?.RadiusY ?? 0.0,
            StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
        };
        _rectangle.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
        if (_rectangle.StartPoint.Owner == null)
        {
            _rectangle.StartPoint.Owner = _rectangle;
        }
        if (_rectangle.Point.Owner == null)
        {
            _rectangle.Point.Owner = _rectangle;
        }
        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_rectangle);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_rectangle.StartPoint);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_rectangle.Point);

        context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

        CurrentState = State.Point;
    }

    private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        CurrentState = State.StartPoint;

        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_rectangle);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_rectangle.Point);

        var radius = Settings?.HitTestRadius ?? 7.0;
        var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

        IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

        _rectangle.Point = point;
        _rectangle.Point.Y = y;
        if (_rectangle.Point.Owner == null)
        {
            _rectangle.Point.Owner = _rectangle;
        }
        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_rectangle.StartPoint);
        _rectangle.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
        context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_rectangle);
        context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
        _rectangle = null;

        FiltersClear(context);

        context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersClear(context);
        FiltersProcess(context, ref x, ref y);

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersClear(context);
        FiltersProcess(context, ref x, ref y);

        _rectangle.Point.X = x;
        _rectangle.Point.Y = y;

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void CleanInternal(IToolContext context)
    {
        CurrentState = State.StartPoint;

        FiltersClear(context);

        if (_rectangle != null)
        {
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_rectangle);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_rectangle.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_rectangle.Point);
            _rectangle = null;
        }

        context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
    {
        switch (CurrentState)
        {
            case State.StartPoint:
            {
                TopLeftInternal(context, x, y, modifier);
            }
                break;
            case State.Point:
            {
                BottomRightInternal(context, x, y, modifier);
            }
                break;
        }
    }

    public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
    {
    }

    public void RightDown(IToolContext context, double x, double y, Modifier modifier)
    {
        switch (CurrentState)
        {
            case State.Point:
            {
                this.Clean(context);
            }
                break;
        }
    }

    public void RightUp(IToolContext context, double x, double y, Modifier modifier)
    {
    }

    public void Move(IToolContext context, double x, double y, Modifier modifier)
    {
        switch (CurrentState)
        {
            case State.StartPoint:
            {
                MoveTopLeftInternal(context, x, y, modifier);
            }
                break;
            case State.Point:
            {
                MoveBottomRightInternal(context, x, y, modifier);
            }
                break;
        }
    }

    public void Clean(IToolContext context)
    {
        CleanInternal(context);
    }
}