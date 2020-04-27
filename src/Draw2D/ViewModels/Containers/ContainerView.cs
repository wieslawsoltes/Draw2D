using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.UI.Zoom.Input;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class ContainerView : ViewModelBase, IContainerView
    {
        private double _width;
        private double _height;
        private IPaint _printBackground;
        private IPaint _workBackground;
        private IPaint _inputBackground;
        private ICanvasContainer _currentContainer;
        private ICanvasContainer _workingContainer;
        private ISelectionState _selectionState;
        private IZoomServiceState _zoomServiceState;
        private IContainerPresenter _containerPresenter;
        private IInputService _inputService;
        private IZoomService _zoomService;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaint PrintBackground
        {
            get => _printBackground;
            set => Update(ref _printBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaint WorkBackground
        {
            get => _workBackground;
            set => Update(ref _workBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaint InputBackground
        {
            get => _inputBackground;
            set => Update(ref _inputBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ICanvasContainer CurrentContainer
        {
            get => _currentContainer;
            set => Update(ref _currentContainer, value);
        }

        [IgnoreDataMember]
        public ICanvasContainer WorkingContainer
        {
            get => _workingContainer;
            set => Update(ref _workingContainer, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISelectionState SelectionState
        {
            get => _selectionState;
            set => Update(ref _selectionState, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IZoomServiceState ZoomServiceState
        {
            get => _zoomServiceState;
            set => Update(ref _zoomServiceState, value);
        }

        [IgnoreDataMember]
        public IContainerPresenter ContainerPresenter
        {
            get => _containerPresenter;
            set => Update(ref _containerPresenter, value);
        }

        [IgnoreDataMember]
        public IInputService InputService
        {
            get => _inputService;
            set => Update(ref _inputService, value);
        }

        [IgnoreDataMember]
        public IZoomService ZoomService
        {
            get => _zoomService;
            set => Update(ref _zoomService, value);
        }

        private bool IsAcceptedShape(IBaseShape shape)
        {
            return !(shape is IPointShape || shape is FigureShape);
        }

        public virtual IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius, double scale, Modifier modifier)
        {
            if (connect == true)
            {
                var point = context.HitTest?.TryToGetPoint(_currentContainer.Shapes, new Point2(x, y), radius, scale, modifier, null);
                if (point != null)
                {
                    return point;
                }
            }
            return new PointShape(x, y, context?.DocumentContainer?.PointTemplate);
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _containerPresenter?.Draw(context, width, height, dx, dy, zx, zy);
        }

        public void Add(IBaseShape shape)
        {
            if (shape != null)
            {
                _currentContainer.Shapes.Add(shape);
                _currentContainer.MarkAsDirty(true);
                _inputService?.Redraw?.Invoke();
            }
        }

        public void Remove(IBaseShape shape)
        {
            if (shape != null)
            {
                _currentContainer.Shapes.Remove(shape);
                _currentContainer.MarkAsDirty(true);
                _inputService?.Redraw?.Invoke();
            }
        }

        public void Reference(IBaseShape shape)
        {
            if (shape != null)
            {
                _selectionState?.Clear();
                var title = shape is GroupShape group ? group.Title : "Reference";
                var reference = new ReferenceShape(title, 0.0, 0.0, shape);
                reference.Select(_selectionState);
                reference.Owner = _currentContainer;
                _currentContainer.Shapes.Add(reference);
                _currentContainer.MarkAsDirty(true);
                _inputService?.Redraw?.Invoke();
            }
        }

        public void Style(string styleId)
        {
            if (_selectionState?.Shapes != null && !string.IsNullOrEmpty(styleId))
            {
                foreach (var shape in _selectionState.Shapes)
                {
                    if (IsAcceptedShape(shape))
                    {
                        shape.StyleId = styleId;
                    }
                }
                _currentContainer.MarkAsDirty(true);
                _inputService?.Redraw?.Invoke();
            }
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new ContainerView()
            {
                Name = this.Name,
                Title = this.Title + "_copy",
                Width = this.Width,
                Height = this.Height,
                PrintBackground = (IPaint)this.PrintBackground?.Copy(shared),
                WorkBackground = (IPaint)this.WorkBackground?.Copy(shared),
                InputBackground = (IPaint)this.InputBackground?.Copy(shared),
                CurrentContainer = (ICanvasContainer)this.CurrentContainer?.Copy(shared),
                WorkingContainer = null,
                SelectionState = new SelectionState()
                {
                    Hovered = null,
                    Selected = null,
                    Shapes = new HashSet<IBaseShape>()
                },
                ZoomServiceState = new ZoomServiceState()
                {
                    ZoomSpeed = 1.2,
                    ZoomX = double.NaN,
                    ZoomY = double.NaN,
                    OffsetX = double.NaN,
                    OffsetY = double.NaN,
                    IsPanning = false,
                    IsZooming = false,
                    InitFitMode = FitMode.Center,
                    AutoFitMode = FitMode.None
                },
                ContainerPresenter = null,
                InputService = null,
                ZoomService = null
            };

            return copy;
        }
    }
}
