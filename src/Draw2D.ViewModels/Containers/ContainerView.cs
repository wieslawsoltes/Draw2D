// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.Input;
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
        private ArgbColor _printBackground;
        private ArgbColor _workBackground;
        private ArgbColor _inputBackground;
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
        public ArgbColor PrintBackground
        {
            get => _printBackground;
            set => Update(ref _printBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor WorkBackground
        {
            get => _workBackground;
            set => Update(ref _workBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor InputBackground
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
            return new PointShape(x, y, context.PointTemplate);
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
                    if (!(shape is IPointShape))
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
                PrintBackground = (ArgbColor)this.PrintBackground?.Copy(shared),
                WorkBackground = (ArgbColor)this.WorkBackground?.Copy(shared),
                InputBackground = (ArgbColor)this.InputBackground?.Copy(shared),
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
