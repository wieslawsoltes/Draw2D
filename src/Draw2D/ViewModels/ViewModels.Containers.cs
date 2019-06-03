// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

namespace Draw2D.ViewModels.Containers
{
    public interface IStyleLibrary : IDirty
    {
        IList<ShapeStyle> Styles { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        void UpdateCache();
        void Add(ShapeStyle value);
        void Remove(ShapeStyle value);
        ShapeStyle Get(string styleId);
    }

    public interface IGroupLibrary : IDirty
    {
        IList<GroupShape> Groups { get; set; }
        GroupShape CurrentGroup { get; set; }
        void UpdateCache();
        void Add(GroupShape value);
        void Remove(GroupShape value);
        GroupShape Get(string groupId);
    }

    public interface ICanvasContainer : IBaseShape
    {
        IList<IBaseShape> Shapes { get; set; }
    }

    public interface IDrawContainerView : IDisposable
    {
        void Draw(object context, double width, double height, double dx, double dy, double zx, double zy);
    }

    public interface IContainerView : IDrawTarget, IHitTestable, ICopyable
    {
        string Title { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        ArgbColor PrintBackground { get; set; }
        ArgbColor WorkBackground { get; set; }
        ArgbColor InputBackground { get; set; }
        ICanvasContainer CurrentContainer { get; set; }
        ICanvasContainer WorkingContainer { get; set; }
        ISelectionState SelectionState { get; set; }
        IZoomServiceState ZoomServiceState { get; set; }
        IDrawContainerView DrawContainerView { get; set; }
        void Add(IBaseShape shape);
        void Remove(IBaseShape shape);
        void Reference(GroupShape group);
        void Style(string styleId);
    }
}

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class StyleLibrary : ViewModelBase, IStyleLibrary
    {
        private Dictionary<string, ShapeStyle> _styleLibraryCache;
        private IList<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ShapeStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShapeStyle CurrentStyle
        {
            get => _currentStyle;
            set => Update(ref _currentStyle, value);
        }

        public override void Invalidate()
        {
            if (_styles != null)
            {
                foreach (var style in _styles)
                {
                    style.Invalidate();
                    style.Stroke?.Invalidate();
                    style.Fill?.Invalidate();
                    style.TextStyle?.Invalidate();
                    style.TextStyle?.Stroke?.Invalidate();
                }
            }

            _currentStyle?.Invalidate();

            base.Invalidate();
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new StyleLibrary()
            {
                Name = this.Name,
                CurrentStyle = (ShapeStyle)this.CurrentStyle?.Copy(shared),
                Styles = new ObservableCollection<ShapeStyle>()
            };

            foreach (var style in this.Styles)
            {
                if (style is ICopyable copyable)
                {
                    copy.Styles.Add((ShapeStyle)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

        public void UpdateCache()
        {
            if (_styles != null)
            {
                if (_styleLibraryCache == null)
                {
                    _styleLibraryCache = new Dictionary<string, ShapeStyle>();
                }
                else
                {
                    _styleLibraryCache.Clear();
                }

                foreach (var style in _styles)
                {
                    _styleLibraryCache[style.Title] = style;
                }
            }
        }

        public void Add(ShapeStyle value)
        {
            if (value != null)
            {
                _styles.Add(value);

                if (_styleLibraryCache == null)
                {
                    _styleLibraryCache = new Dictionary<string, ShapeStyle>();
                }

                _styleLibraryCache[value.Title] = value;
            }
        }

        public void Remove(ShapeStyle value)
        {
            if (value != null)
            {
                _styles.Remove(value);

                if (_styleLibraryCache != null)
                {
                    _styleLibraryCache.Remove(value.Title);
                }
            }
        }

        public ShapeStyle Get(string styleId)
        {
            if (_styleLibraryCache == null)
            {
                UpdateCache();
            }

            if (!_styleLibraryCache.TryGetValue(styleId, out var value))
            {
                foreach (var style in _styles)
                {
                    if (style.Title == styleId)
                    {
                        _styleLibraryCache[style.Title] = style;
                        return style;
                    }
                }
                return null;
            }

            return value;
        }
    }

    [DataContract(IsReference = true)]
    public class GroupLibrary : ViewModelBase, IGroupLibrary
    {
        private Dictionary<string, GroupShape> _groupLibraryCache;
        private IList<GroupShape> _groups;
        private GroupShape _currentGroup;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<GroupShape> Groups
        {
            get => _groups;
            set => Update(ref _groups, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GroupShape CurrentGroup
        {
            get => _currentGroup;
            set => Update(ref _currentGroup, value);
        }

        public override void Invalidate()
        {
            if (_groups != null)
            {
                foreach (var group in _groups)
                {
                    group.Invalidate();
                }
            }

            _currentGroup?.Invalidate();

            base.Invalidate();
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new GroupLibrary()
            {
                Name = this.Name,
                CurrentGroup = (GroupShape)this.CurrentGroup?.Copy(shared),
                Groups = new ObservableCollection<GroupShape>()
            };

            foreach (var style in this.Groups)
            {
                if (style is ICopyable copyable)
                {
                    copy.Groups.Add((GroupShape)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

        public void UpdateCache()
        {
            if (_groups != null)
            {
                if (_groupLibraryCache == null)
                {
                    _groupLibraryCache = new Dictionary<string, GroupShape>();
                }
                else
                {
                    _groupLibraryCache.Clear();
                }

                foreach (var style in _groups)
                {
                    _groupLibraryCache[style.Title] = style;
                }
            }
        }

        public void Add(GroupShape value)
        {
            if (value != null)
            {
                _groups.Add(value);

                if (_groupLibraryCache == null)
                {
                    _groupLibraryCache = new Dictionary<string, GroupShape>();
                }

                _groupLibraryCache[value.Title] = value;
            }
        }

        public void Remove(GroupShape value)
        {
            if (value != null)
            {
                _groups.Remove(value);

                if (_groupLibraryCache != null)
                {
                    _groupLibraryCache.Remove(value.Title);
                }
            }
        }

        public GroupShape Get(string groupId)
        {
            if (_groupLibraryCache == null)
            {
                UpdateCache();
            }

            if (!_groupLibraryCache.TryGetValue(groupId, out var value))
            {
                foreach (var style in _groups)
                {
                    if (style.Title == groupId)
                    {
                        _groupLibraryCache[style.Title] = style;
                        return style;
                    }
                }
                return null;
            }

            return value;
        }
    }

    [DataContract(IsReference = true)]
    public class CanvasContainer : GroupShape, ICanvasContainer
    {
        internal static new IBounds s_bounds = new ContainerBounds();
        internal static new IShapeDecorator s_decorator = new ContainerDecorator();

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        public CanvasContainer()
        {
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new CanvasContainer()
            {
                Name = this.Name,
                Title = this.Title + "_copy",
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                StyleId = this.StyleId
            };

            if (shared != null)
            {
                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                foreach (var shape in this.Shapes)
                {
                    if (shape is ICopyable copyable)
                    {
                        copy.Shapes.Add((IBaseShape)(copyable.Copy(shared)));
                    }
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class SelectionState : ViewModelBase, ISelectionState
    {
        private IBaseShape _hovered;
        private IBaseShape _selected;
        private ISet<IBaseShape> _shapes;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Hovered
        {
            get => _hovered;
            set => Update(ref _hovered, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISet<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public void Hover(IBaseShape shape)
        {
            if (shape != null)
            {
                shape.Select(this);
                Hovered = shape;
                this.MarkAsDirty(true);
            }
        }

        public void Dehover()
        {
            if (_hovered != null)
            {
                _hovered.Deselect(this);
                Hovered = null;
                this.MarkAsDirty(true);
            }
        }

        public bool IsSelected(IBaseShape shape)
        {
            if (shape != null && _shapes.Contains(shape))
            {
                return true;
            }
            return false;
        }

        public void Select(IBaseShape shape)
        {
            if (shape != null)
            {
                if (_shapes.Count == 0)
                {
                    Selected = shape;
                }
                _shapes.Add(shape);
                this.MarkAsDirty(true);
            }
        }

        public void Deselect(IBaseShape shape)
        {
            if (shape != null)
            {
                _shapes.Remove(shape);
                if (_shapes.Count == 0)
                {
                    Selected = null;
                }
                this.MarkAsDirty(true);
            }
        }

        public void Clear()
        {
            _shapes.Clear();
            Selected = null;
            this.MarkAsDirty(true);
        }
    }

    [DataContract(IsReference = true)]
    public class ZoomServiceState : ViewModelBase, IZoomServiceState
    {
        private double _zoomSpeed;
        private double _zoomX;
        private double _zoomY;
        private double _offsetX;
        private double _offsetY;
        private bool _isPanning;
        private bool _isZooming;
        private FitMode _initFitMode;
        private FitMode _autoFitMode;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomSpeed
        {
            get => _zoomSpeed;
            set => Update(ref _zoomSpeed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomX
        {
            get => _zoomX;
            set => Update(ref _zoomX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomY
        {
            get => _zoomY;
            set => Update(ref _zoomY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsPanning
        {
            get => _isPanning;
            set => Update(ref _isPanning, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsZooming
        {
            get => _isZooming;
            set => Update(ref _isZooming, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FitMode InitFitMode
        {
            get => _initFitMode;
            set => Update(ref _initFitMode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FitMode AutoFitMode
        {
            get => _autoFitMode;
            set => Update(ref _autoFitMode, value);
        }
    }

    [DataContract(IsReference = true)]
    public class ContainerView : ViewModelBase, IContainerView
    {
        private string _title;
        private double _width;
        private double _height;
        private ArgbColor _printBackground;
        private ArgbColor _workBackground;
        private ArgbColor _inputBackground;
        private ICanvasContainer _currentContainer;
        private ICanvasContainer _workingContainer;
        private ISelectionState _selectionState;
        private IZoomServiceState _zoomServiceState;
        private IDrawContainerView _drawContainerView;
        private IInputService _inputService;
        private IZoomService _zoomService;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

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
        public IDrawContainerView DrawContainerView
        {
            get => _drawContainerView;
            set => Update(ref _drawContainerView, value);
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

        public virtual IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius)
        {
            if (connect == true)
            {
                var point = context.HitTest?.TryToGetPoint(_currentContainer.Shapes, new Point2(x, y), radius, null);
                if (point != null)
                {
                    return point;
                }
            }
            return new PointShape(x, y, context.PointTemplate);
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _drawContainerView?.Draw(context, width, height, dx, dy, zx, zy);
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

        public void Reference(GroupShape group)
        {
            if (group != null)
            {
                _selectionState?.Clear();
                group.GetBox(out double ax, out double ay, out _, out _);
                var reference = new ReferenceShape(group.Title, ax, ay, group);
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
                DrawContainerView = null,
                InputService = null,
                ZoomService = null
            };

            return copy;
        }
    }
}
