using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public class ConicTool : BaseTool, ITool
{
    private ConicToolSettings _settings;
    private ConicShape _conic = null;

    public enum State
    {
        StartPoint,
        Point1,
        Point2
    }

    [IgnoreDataMember]
    public State CurrentState { get; set; } = State.StartPoint;

    [IgnoreDataMember]
    public new string Title => "Conic";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ConicToolSettings Settings
    {
        get => _settings;
        set => Update(ref _settings, value);
    }

    private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        var radius = Settings?.HitTestRadius ?? 7.0;
        var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

        IPointShape startPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);
        IPointShape point1 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);
        IPointShape point2 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

        _conic = new ConicShape()
        {
            Points = new ObservableCollection<IPointShape>(),
            StartPoint = startPoint,
            Point1 = point1,
            Point2 = point2,
            Weight = Settings?.Weight ?? 1.0,
            Text = new Text(),
            StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
        };
        _conic.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
        if (_conic.StartPoint.Owner == null)
        {
            _conic.StartPoint.Owner = _conic;
        }
        if (_conic.Point1.Owner == null)
        {
            _conic.Point1.Owner = _conic;
        }
        if (_conic.Point2.Owner == null)
        {
            _conic.Point2.Owner = _conic;
        }
        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_conic);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_conic);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_conic.StartPoint);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_conic.Point1);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_conic.Point2);

        context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

        CurrentState = State.Point2;
    }

    private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        CurrentState = State.StartPoint;

        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.StartPoint);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.Point1);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.Point2);
        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_conic);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

        var radius = Settings?.HitTestRadius ?? 7.0;
        var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

        IPointShape point1 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

        _conic.Point1 = point1;
        if (_conic.Point1.Owner == null)
        {
            _conic.Point1.Owner = _conic;
        }
        _conic.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
        context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_conic);
        context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
        _conic = null;

        FiltersClear(context);

        context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        _conic.Point1.X = x;
        _conic.Point1.Y = y;

        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.Point2);

        var radius = Settings?.HitTestRadius ?? 7.0;
        var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

        IPointShape point2 = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0, scale, modifier);

        _conic.Point2 = point2;
        if (_conic.Point2.Owner == null)
        {
            _conic.Point2.Owner = _conic;
        }
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_conic.Point2);

        CurrentState = State.Point1;

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersClear(context);
        FiltersProcess(context, ref x, ref y);

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersClear(context);
        FiltersProcess(context, ref x, ref y);

        _conic.Point1.X = x;
        _conic.Point1.Y = y;

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersClear(context);
        FiltersProcess(context, ref x, ref y);

        _conic.Point1.X = x;
        _conic.Point1.Y = y;
        _conic.Point2.X = x;
        _conic.Point2.Y = y;

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void CleanInternal(IToolContext context)
    {
        FiltersClear(context);

        CurrentState = State.StartPoint;

        if (_conic != null)
        {
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_conic);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.Point1);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_conic.Point2);
            _conic = null;
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
                StartPointInternal(context, x, y, modifier);
            }
                break;
            case State.Point1:
            {
                Point1Internal(context, x, y, modifier);
            }
                break;
            case State.Point2:
            {
                Point2Internal(context, x, y, modifier);
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
            case State.Point1:
            case State.Point2:
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
                MoveStartPointInternal(context, x, y, modifier);
            }
                break;
            case State.Point1:
            {
                MovePoint1Internal(context, x, y, modifier);
            }
                break;
            case State.Point2:
            {
                MovePoint2Internal(context, x, y, modifier);
            }
                break;
        }
    }

    public void Clean(IToolContext context)
    {
        CleanInternal(context);
    }
}