using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools;

[DataContract(IsReference = true)]
public class TextTool : BaseTool, ITool
{
    private TextToolSettings _settings;
    private TextShape _text = null;

    public enum State
    {
        StartPoint,
        Point
    }

    [IgnoreDataMember]
    public State CurrentState { get; set; } = State.StartPoint;

    [IgnoreDataMember]
    public new string Title => "Text";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TextToolSettings Settings
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

        _text = new TextShape()
        {
            Points = new ObservableCollection<IPointShape>(),
            StartPoint = startPoint,
            Point = point,
            Text = new Text("Text"),
            StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
        };
        _text.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
        if (_text.StartPoint.Owner == null)
        {
            _text.StartPoint.Owner = _text;
        }
        if (_text.Point.Owner == null)
        {
            _text.Point.Owner = _text;
        }
        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_text);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_text);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_text.StartPoint);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_text.Point);

        context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

        CurrentState = State.Point;
    }

    private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        CurrentState = State.StartPoint;

        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_text);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_text.Point);

        var radius = Settings?.HitTestRadius ?? 7.0;
        var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

        IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

        _text.Point = point;
        _text.Point.Y = y;
        if (_text.Point.Owner == null)
        {
            _text.Point.Owner = _text;
        }
        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_text);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_text.StartPoint);
        _text.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
        context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_text);
        context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
        _text = null;

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

        _text.Point.X = x;
        _text.Point.Y = y;

        context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
    }

    private void CleanInternal(IToolContext context)
    {
        CurrentState = State.StartPoint;

        FiltersClear(context);

        if (_text != null)
        {
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_text);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_text);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_text.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_text.Point);
            _text = null;
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