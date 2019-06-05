// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Tools
{
    public partial class PathTool : IToolContext
    {
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
                get => _context.ContainerView.Title;
                set => throw new InvalidOperationException($"Can not set {Title} property value.");
            }

            public double Width
            {
                get => _context.ContainerView.Width;
                set => throw new InvalidOperationException($"Can not set {Width} property value.");
            }

            public double Height
            {
                get => _context.ContainerView.Width;
                set => throw new InvalidOperationException($"Can not set {Height} property value.");
            }

            public ArgbColor PrintBackground
            {
                get => _context.ContainerView.PrintBackground;
                set => throw new InvalidOperationException($"Can not set {PrintBackground} property value.");
            }

            public ArgbColor WorkBackground
            {
                get => _context.ContainerView.WorkBackground;
                set => throw new InvalidOperationException($"Can not set {WorkBackground} property value.");
            }

            public ArgbColor InputBackground
            {
                get => _context.ContainerView.InputBackground;
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

            public IDrawContainerView DrawContainerView
            {
                get => _context.ContainerView.DrawContainerView;
                set => throw new InvalidOperationException($"Can not set {DrawContainerView} property value.");
            }

            public ISelectionState SelectionState
            {
                get => _context.ContainerView.SelectionState;
                set => throw new InvalidOperationException($"Can not set {SelectionState} property value.");
            }

            public IZoomServiceState ZoomServiceState
            {
                get => _context.ContainerView.ZoomServiceState;
                set => throw new InvalidOperationException($"Can not set {ZoomServiceState} property value.");
            }

            public IInputService InputService
            {
                get => _context.ContainerView?.InputService;
                set => throw new InvalidOperationException($"Can not set {InputService} property value.");
            }

            public IZoomService ZoomService
            {
                get => _context.ContainerView.ZoomService;
                set => throw new InvalidOperationException($"Can not set {ZoomService} property value.");
            }

            public IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius)
            {
                if (_nextPoint != null)
                {
                    var nextPointTemp = _nextPoint;
                    _nextPoint = null;
                    return nextPointTemp;
                }
                return _context.ContainerView.GetNextPoint(_context, x, y, connect, radius);
            }

            public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
            {
                _context.ContainerView.Draw(context, width, height, dx, dy, zx, zy);
            }

            public void Add(IBaseShape shape)
            {
                _context.ContainerView.Add(shape);
            }

            public void Remove(IBaseShape shape)
            {
                _context.ContainerView.Remove(shape);
            }

            public void Reference(GroupShape group)
            {
                _context.ContainerView.Reference(group);
            }

            public void Style(string styleId)
            {
                _context.ContainerView.Style(styleId);
            }

            public object Copy(Dictionary<object, object> shared)
            {
                return null;
            }
        }

        internal IToolContext _context;
        internal FigureContainerView _containerView;

        [IgnoreDataMember]
        public IStyleLibrary StyleLibrary
        {
            get => _context.StyleLibrary;
            set => throw new InvalidOperationException($"Can not set {StyleLibrary} property value.");
        }

        [IgnoreDataMember]
        public IGroupLibrary GroupLibrary
        {
            get => _context.GroupLibrary;
            set => throw new InvalidOperationException($"Can not set {GroupLibrary} property value.");
        }

        [IgnoreDataMember]
        public IBaseShape PointTemplate
        {
            get => _context.PointTemplate;
            set => throw new InvalidOperationException($"Can not set {PointTemplate} property value.");
        }

        [IgnoreDataMember]
        public IHitTest HitTest
        {
            get => _context.HitTest;
            set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
        }

        [IgnoreDataMember]
        public IList<IContainerView> ContainerViews
        {
            get => _context.ContainerViews;
            set => throw new InvalidOperationException($"Can not set {ContainerViews} property value.");
        }

        [IgnoreDataMember]
        public IContainerView ContainerView
        {
            get => _containerView;
            set => throw new InvalidOperationException($"Can not set {ContainerView} property value.");
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

        public void SetTool(string name) => _context.SetTool(name);

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
        public string Title => "Path";

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

            _path = new PathShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                FillRule = Settings.FillRule,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };

            context.ContainerView?.WorkingContainer.Shapes.Add(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_path);
        }

        internal void Move(IToolContext context)
        {
            _figure = new FigureShape()
            {
                Shapes = new ObservableCollection<IBaseShape>(),
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };
            _path.Shapes.Add(_figure);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

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
                context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_path);

                if (_path.Validate(true) == true)
                {
                    context.ContainerView?.CurrentContainer.Shapes.Add(_path);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                }

                Settings.PreviousTool = null;
                SetNextPoint(null);
                SetContext(null);

                _path = null;
                _figure = null;
                _containerView = null;
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
}
