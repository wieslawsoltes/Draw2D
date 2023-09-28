using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using CommunityToolkit.Mvvm.Input;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Tools;

internal class FigureContainerView : IContainerView
{
    internal IToolContext _context;
    internal PathTool _pathTool;
    internal IPointShape _nextPoint;

    public FigureContainerView(IToolContext context, PathTool pathTool)
    {
        _context = context;
        _pathTool = pathTool;
    }

    public string Title
    {
        get => _context.DocumentContainer.ContainerView.Title;
        set => throw new InvalidOperationException($"Can not set {Title} property value.");
    }

    public double Width
    {
        get => _context.DocumentContainer.ContainerView.Width;
        set => throw new InvalidOperationException($"Can not set {Width} property value.");
    }

    public double Height
    {
        get => _context.DocumentContainer.ContainerView.Width;
        set => throw new InvalidOperationException($"Can not set {Height} property value.");
    }

    public IPaint PrintBackground
    {
        get => _context.DocumentContainer.ContainerView.PrintBackground;
        set => throw new InvalidOperationException($"Can not set {PrintBackground} property value.");
    }

    public IPaint WorkBackground
    {
        get => _context.DocumentContainer.ContainerView.WorkBackground;
        set => throw new InvalidOperationException($"Can not set {WorkBackground} property value.");
    }

    public IPaint InputBackground
    {
        get => _context.DocumentContainer.ContainerView.InputBackground;
        set => throw new InvalidOperationException($"Can not set {InputBackground} property value.");
    }

    public ICanvasContainer CurrentContainer
    {
        get => _pathTool._figure;
        set => throw new InvalidOperationException($"Can not set {CurrentContainer} property value.");
    }

    public ICanvasContainer WorkingContainer
    {
        get => _pathTool._figure;
        set => throw new InvalidOperationException($"Can not set {WorkingContainer} property value.");
    }

    public IContainerPresenter ContainerPresenter
    {
        get => _context.DocumentContainer.ContainerView.ContainerPresenter;
        set => throw new InvalidOperationException($"Can not set {ContainerPresenter} property value.");
    }

    public ISelectionState SelectionState
    {
        get => _context.DocumentContainer.ContainerView.SelectionState;
        set => throw new InvalidOperationException($"Can not set {SelectionState} property value.");
    }

    public IZoomServiceState ZoomServiceState
    {
        get => _context.DocumentContainer.ContainerView.ZoomServiceState;
        set => throw new InvalidOperationException($"Can not set {ZoomServiceState} property value.");
    }

    public IInputService InputService
    {
        get => _context.DocumentContainer.ContainerView?.InputService;
        set => throw new InvalidOperationException($"Can not set {InputService} property value.");
    }

    public IZoomService ZoomService
    {
        get => _context.DocumentContainer.ContainerView.ZoomService;
        set => throw new InvalidOperationException($"Can not set {ZoomService} property value.");
    }

    public IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius, double scale, Modifier modifier)
    {
        if (_nextPoint != null)
        {
            var nextPointTemp = _nextPoint;
            _nextPoint = null;
            return nextPointTemp;
        }
        return _context.DocumentContainer.ContainerView.GetNextPoint(_context, x, y, connect, radius, scale, modifier);
    }

    public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy, double renderScaling)
    {
        _context.DocumentContainer.ContainerView.Draw(context, width, height, dx, dy, zx, zy, renderScaling);
    }

    public void Add(IBaseShape shape)
    {
        _context.DocumentContainer.ContainerView.Add(shape);
    }

    public void Remove(IBaseShape shape)
    {
        _context.DocumentContainer.ContainerView.Remove(shape);
    }

    public void Reference(IBaseShape shape)
    {
        _context.DocumentContainer.ContainerView.Reference(shape);
    }

    public void Style(string styleId)
    {
        _context.DocumentContainer.ContainerView.Style(styleId);
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return null;
    }
}

[DataContract(IsReference = true)]
public class FigureDocumentContainer : IDocumentContainer
{
    internal IToolContext _context;
    internal PathTool _pathTool;

    public FigureDocumentContainer(IToolContext context, PathTool pathTool)
    {
        _context = context;
        _pathTool = pathTool;
    }

    public string Title
    {
        get => _context.DocumentContainer.Title;
        set => throw new InvalidOperationException($"Can not set {Title} property value.");
    }

    public IStyleLibrary StyleLibrary
    {
        get => _context.DocumentContainer.StyleLibrary;
        set => throw new InvalidOperationException($"Can not set {StyleLibrary} property value.");
    }

    public IGroupLibrary GroupLibrary
    {
        get => _context.DocumentContainer.GroupLibrary;
        set => throw new InvalidOperationException($"Can not set {GroupLibrary} property value.");
    }

    public IBaseShape PointTemplate
    {
        get => _context.DocumentContainer.PointTemplate;
        set => throw new InvalidOperationException($"Can not set {PointTemplate} property value.");
    }

    public IList<IContainerView> ContainerViews
    {
        get => _context.DocumentContainer.ContainerViews;
        set => throw new InvalidOperationException($"Can not set {ContainerViews} property value.");
    }

    public IContainerView ContainerView
    {
        get => _pathTool._containerView;
        set => throw new InvalidOperationException($"Can not set {ContainerView} property value.");
    }

    public void Dispose()
    {
    }

    public virtual object Copy(Dictionary<object, object> shared)
    {
        return null;
    }
}

public partial class PathTool : IToolContext
{
    internal IToolContext _context;
    internal FigureContainerView _containerView;
    internal FigureDocumentContainer _documentContainer;

    [IgnoreDataMember]
    public IDocumentContainer DocumentContainer
    {
        get => _documentContainer;
        set => throw new InvalidOperationException($"Can not set {DocumentContainer} property value.");
    }

    [IgnoreDataMember]
    public IHitTest HitTest
    {
        get => _context.HitTest;
        set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
    }

    [IgnoreDataMember]
    public IPathConverter PathConverter
    {
        get => _context.PathConverter;
        set => throw new InvalidOperationException($"Can not set {PathConverter} property value.");
    }

    [IgnoreDataMember]
    public IList<ITool> Tools
    {
        get => _context.Tools;
        set => throw new InvalidOperationException($"Can not set {Tools} property value.");
    }

    [IgnoreDataMember]
    public ITool CurrentTool
    {
        get => _context.CurrentTool;
        set => throw new InvalidOperationException($"Can not set {CurrentTool} property value.");
    }

    [IgnoreDataMember]
    public EditMode EditMode
    {
        get => _context.EditMode;
        set => throw new InvalidOperationException($"Can not set {EditMode} property value.");
    }

    public void Dispose()
    {
    }

    private void SetNextPoint(IPointShape point) => _containerView._nextPoint = point;

    private void SetContext(IToolContext context) => _context = context;

    [RelayCommand]
    public void SetTool(string title) => _context.SetTool(title);

    public double GetWidth() => _context.GetWidth();

    public double GetHeight() => _context.GetHeight();

    public void LeftDown(double x, double y, Modifier modifier) => _context.LeftDown(x, y, modifier);

    public void LeftUp(double x, double y, Modifier modifier) => _context.LeftUp(x, y, modifier);

    public void RightDown(double x, double y, Modifier modifier) => _context.RightDown(x, y, modifier);

    public void RightUp(double x, double y, Modifier modifier) => _context.RightUp(x, y, modifier);

    public void Move(double x, double y, Modifier modifier) => _context.Move(x, y, modifier);
}

[DataContract(IsReference = true)]
public partial class PathTool : BaseTool, ITool
{
    private PathToolSettings _settings;

    internal PathShape _path;
    internal FigureShape _figure;

    [IgnoreDataMember]
    public new string Title => "Path";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PathToolSettings Settings
    {
        get => _settings;
        set => Update(ref _settings, value);
    }

    internal void Create(IToolContext context)
    {
        if (_containerView == null)
        {
            _containerView = new FigureContainerView(context, this);
        }

        if (_documentContainer == null)
        {
            _documentContainer = new FigureDocumentContainer(context, this);
        }

        _path = new PathShape()
        {
            Points = new ObservableCollection<IPointShape>(),
            Shapes = new ObservableCollection<IBaseShape>(),
            FillType = Settings.FillType,
            Text = new Text(),
            StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
        };

        context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_path);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        context.DocumentContainer?.ContainerView?.SelectionState?.Select(_path);
    }

    internal void Move(IToolContext context)
    {
        _figure = new FigureShape()
        {
            Points = new ObservableCollection<IPointShape>(),
            Shapes = new ObservableCollection<IBaseShape>(),
            IsFilled = Settings.IsFilled,
            IsClosed = Settings.IsClosed
        };
        _figure.Owner = _path;
        _path.Shapes.Add(_figure);
        context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);

        if (Settings.PreviousTool != null)
        {
            Settings.CurrentTool = Settings.PreviousTool;
        }
    }

    internal void CleanCurrentTool(IToolContext context)
    {
        SetContext(context);
        Settings.CurrentTool?.Clean(this);
        SetContext(null);
    }

    internal void UpdateCache(IToolContext context)
    {
        if (_path != null)
        {
            _figure.MarkAsDirty(true);
            _figure.MarkAsDirty(true);
        }
    }

    private void DownInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersProcess(context, ref x, ref y);

        if (_path == null)
        {
            Create(context);
            Move(context);
        }

        SetContext(context);
        Settings.CurrentTool?.LeftDown(this, x, y, modifier);

        switch (Settings.CurrentTool)
        {
            case LineTool lineTool:
            {
                if (lineTool.CurrentState == LineTool.State.StartPoint)
                {
                    SetNextPoint(_path?.GetLastPoint());
                    Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                    SetNextPoint(null);
                }
            }
                break;
            case CubicBezierTool cubicBezierTool:
            {
                if (cubicBezierTool.CurrentState == CubicBezierTool.State.StartPoint)
                {
                    SetNextPoint(_path?.GetLastPoint());
                    Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                    SetNextPoint(null);
                }
            }
                break;
            case QuadraticBezierTool quadraticBezierTool:
            {
                if (quadraticBezierTool.CurrentState == QuadraticBezierTool.State.StartPoint)
                {
                    SetNextPoint(_path?.GetLastPoint());
                    Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                    SetNextPoint(null);
                }
            }
                break;
            case ConicTool conicTool:
            {
                if (conicTool.CurrentState == ConicTool.State.StartPoint)
                {
                    SetNextPoint(_path?.GetLastPoint());
                    Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                    SetNextPoint(null);
                }
            }
                break;
        }

        SetContext(null);
    }

    private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
    {
        FiltersClear(context);
        FiltersProcess(context, ref x, ref y);

        if (_containerView == null)
        {
            _containerView = new FigureContainerView(context, this);
        }

        if (_documentContainer == null)
        {
            _documentContainer = new FigureDocumentContainer(context, this);
        }

        SetContext(context);
        Settings.CurrentTool.Move(this, x, y, modifier);
        SetContext(null);
    }

    private void CleanInternal(IToolContext context)
    {
        CleanCurrentTool(context);

        FiltersClear(context);

        if (_path != null)
        {
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_path);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_path);

            if (_path.Validate(true) == true)
            {
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_path);
                context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            Settings.PreviousTool = null;
            SetNextPoint(null);
            SetContext(null);

            _path = null;
            _figure = null;
            _containerView = null;
            _documentContainer = null;
        }
    }

    public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
    {
        DownInternal(context, x, y, modifier);
    }

    public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
    {
    }

    public void RightDown(IToolContext context, double x, double y, Modifier modifier)
    {
        this.Clean(context);
    }

    public void RightUp(IToolContext context, double x, double y, Modifier modifier)
    {
    }

    public void Move(IToolContext context, double x, double y, Modifier modifier)
    {
        MoveInternal(context, x, y, modifier);
    }

    public void Clean(IToolContext context)
    {
        CleanInternal(context);
    }
}
