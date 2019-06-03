// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define USE_CONTAINER_POINTS
#define USE_SERIALIZE_STYLES
#define USE_SERIALIZE_GROUPS
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

namespace Draw2D.ViewModels.Tools
{
    public interface ISelection : IDirty
    {
        void Cut(IToolContext context);
        void Copy(IToolContext context);
        void Paste(IToolContext context);
        void Delete(IToolContext context);
        void Group(IToolContext context);
        void Reference(IToolContext context);
        void SelectAll(IToolContext context);
        void Connect(IToolContext context, IPointShape point);
        void Disconnect(IToolContext context, IPointShape point);
        void Disconnect(IToolContext context, IBaseShape shape);
    }

    public interface IPointFilter : INode, IDirty
    {
        string Title { get; }
        IList<IBaseShape> Guides { get; set; }
        bool Process(IToolContext context, ref double x, ref double y);
        void Clear(IToolContext context);
    }

    public interface IPointIntersection : INode, IDirty
    {
        string Title { get; }
        IList<IPointShape> Intersections { get; set; }
        void Find(IToolContext context, IBaseShape shape);
        void Clear(IToolContext context);
    }

    public interface ITool
    {
        string Title { get; }
        IList<IPointIntersection> Intersections { get; set; }
        IPointIntersection CurrentIntersection { get; set; }
        IList<IPointFilter> Filters { get; set; }
        IPointFilter CurrentFilter { get; set; }
        void LeftDown(IToolContext context, double x, double y, Modifier modifier);
        void LeftUp(IToolContext context, double x, double y, Modifier modifier);
        void RightDown(IToolContext context, double x, double y, Modifier modifier);
        void RightUp(IToolContext context, double x, double y, Modifier modifier);
        void Move(IToolContext context, double x, double y, Modifier modifier);
        void Clean(IToolContext context);
    }

    public interface IToolContext : IInputTarget, IDisposable
    {
        IStyleLibrary StyleLibrary { get; set; }
        IGroupLibrary GroupLibrary { get; set; }
        IBaseShape PointTemplate { get; set; }
        IHitTest HitTest { get; set; }
        IList<IContainerView> ContainerViews { get; set; }
        IContainerView ContainerView { get; set; }
        IList<ITool> Tools { get; set; }
        ITool CurrentTool { get; set; }
        EditMode Mode { get; set; }
        void SetTool(string name);
    }

    public interface IContainerFactory
    {
        IStyleLibrary CreateStyleLibrary();
        IGroupLibrary CreateGroupLibrary();
        IToolContext CreateToolContext();
        IContainerView CreateContainerView(string title);
    }
}

namespace Draw2D.ViewModels.Tools
{
    [Flags]
    public enum SelectionMode
    {
        None = 0,
        Point = 1,
        Shape = 2,
        All = Point | Shape
    }

    [Flags]
    public enum SelectionTargets
    {
        None = 0,
        Shapes = 1,
        Guides = 2,
        All = Shapes | Guides
    }

    [DataContract(IsReference = true)]
    public class ConicToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private double _weight;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Weight
        {
            get => _weight;
            set => Update(ref _weight, value);
        }
    }

    [DataContract(IsReference = true)]
    public class CubicBezierToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class LineToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private bool _splitIntersections;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool SplitIntersections
        {
            get => _splitIntersections;
            set => Update(ref _splitIntersections, value);
        }
    }

    [DataContract(IsReference = true)]
    public class MoveToolSettings : Settings
    {
    }

    [DataContract(IsReference = true)]
    public class NoneToolSettings : Settings
    {
    }

    [DataContract(IsReference = true)]
    public class PathToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private ITool _previousTool;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                PreviousTool = _currentTool;
                Update(ref _currentTool, value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool PreviousTool
        {
            get => _previousTool;
            set => Update(ref _previousTool, value);
        }
    }

    [DataContract(IsReference = true)]
    public class PointToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class PolyLineToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class ScribbleToolSettings : Settings
    {
        private bool _simplify;
        private double _epsilon;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }
    }

    [DataContract(IsReference = true)]
    public class SelectionToolSettings : Settings
    {
        private SelectionMode _mode;
        private SelectionTargets _targets;
        private Modifier _selectionModifier;
        private Modifier _connectionModifier;
        private string _selectionStyle;
        private bool _clearSelectionOnClean;
        private double _hitTestRadius;
        private bool _connectPoints;
        private double _connectTestRadius;
        private bool _disconnectPoints;
        private double _disconnectTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionTargets Targets
        {
            get => _targets;
            set => Update(ref _targets, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Modifier SelectionModifier
        {
            get => _selectionModifier;
            set => Update(ref _selectionModifier, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Modifier ConnectionModifier
        {
            get => _connectionModifier;
            set => Update(ref _connectionModifier, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string SelectionStyle
        {
            get => _selectionStyle;
            set => Update(ref _selectionStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ClearSelectionOnClean
        {
            get => _clearSelectionOnClean;
            set => Update(ref _clearSelectionOnClean, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ConnectTestRadius
        {
            get => _connectTestRadius;
            set => Update(ref _connectTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool DisconnectPoints
        {
            get => _disconnectPoints;
            set => Update(ref _disconnectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double DisconnectTestRadius
        {
            get => _disconnectTestRadius;
            set => Update(ref _disconnectTestRadius, value);
        }
    }

    [DataContract(IsReference = true)]
    public class TextToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }
}

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public abstract class BaseTool : ViewModelBase
    {
        private IList<IPointIntersection> _intersections;
        private IPointIntersection _currentIntersection;
        private IList<IPointFilter> _filters;
        private IPointFilter _currentFilter;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointIntersection CurrentIntersection
        {
            get => _currentIntersection;
            set => Update(ref _currentIntersection, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointFilter CurrentFilter
        {
            get => _currentFilter;
            set => Update(ref _currentFilter, value);
        }

        internal void FiltersProcess(IToolContext context, ref double x, ref double y)
        {
            if (_filters != null)
            {
                foreach (var filter in _filters)
                {
                    if (filter.Process(context, ref x, ref y))
                    {
                        return;
                    }
                }
            }
        }

        internal void FiltersClear(IToolContext context)
        {
            if (_filters != null)
            {
                foreach (var filter in _filters)
                {
                    filter.Clear(context);
                }
            }
        }

        internal void IntersectionsFind(IToolContext context, IBaseShape shape)
        {
            if (_intersections != null)
            {
                foreach (var intersection in _intersections)
                {
                    intersection.Find(context, shape);
                }
            }
        }

        internal void IntersectionsClear(IToolContext context)
        {
            if (_intersections != null)
            {
                foreach (var intersection in _intersections)
                {
                    intersection.Clear(context);
                }
            }
        }

        internal bool HaveIntersections()
        {
            if (_intersections != null)
            {
                foreach (var intersection in _intersections)
                {
                    if (intersection.Intersections.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

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
        public string Title => "Conic";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ConicToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);
            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _conic = new ConicShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point1 = point1,
                Point2 = point2,
                Weight = Settings.Weight,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
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
            context.ContainerView?.WorkingContainer.Shapes.Add(_conic);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_conic);
            context.ContainerView?.SelectionState?.Select(_conic.StartPoint);
            context.ContainerView?.SelectionState?.Select(_conic.Point1);
            context.ContainerView?.SelectionState?.Select(_conic.Point2);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_conic);
            context.ContainerView?.SelectionState?.Deselect(_conic.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_conic.Point1);
            context.ContainerView?.SelectionState?.Deselect(_conic.Point2);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_conic);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _conic.Point1 = point1;
            if (_conic.Point1.Owner == null)
            {
                _conic.Point1.Owner = _conic;
            }
            context.ContainerView?.CurrentContainer.Shapes.Add(_conic);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _conic = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _conic.Point1.X = x;
            _conic.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_conic.Point2);

            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _conic.Point2 = point2;
            if (_conic.Point2.Owner == null)
            {
                _conic.Point2.Owner = _conic;
            }
            context.ContainerView?.SelectionState?.Select(_conic.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _conic.Point1.X = x;
            _conic.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _conic.Point1.X = x;
            _conic.Point1.Y = y;
            _conic.Point2.X = x;
            _conic.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_conic != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_conic);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_conic);
                context.ContainerView?.SelectionState?.Deselect(_conic.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_conic.Point1);
                context.ContainerView?.SelectionState?.Deselect(_conic.Point2);
                _conic = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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

    [DataContract(IsReference = true)]
    public class CubicBezierTool : BaseTool, ITool
    {
        private CubicBezierToolSettings _settings;
        private CubicBezierShape _cubicBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2,
            Point3
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "CubicBezier";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public CubicBezierToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);
            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);
            IPointShape point3 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _cubicBezier = new CubicBezierShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_cubicBezier.StartPoint.Owner == null)
            {
                _cubicBezier.StartPoint.Owner = _cubicBezier;
            }
            if (_cubicBezier.Point1.Owner == null)
            {
                _cubicBezier.Point1.Owner = _cubicBezier;
            }
            if (_cubicBezier.Point2.Owner == null)
            {
                _cubicBezier.Point2.Owner = _cubicBezier;
            }
            if (_cubicBezier.Point3.Owner == null)
            {
                _cubicBezier.Point3.Owner = _cubicBezier;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_cubicBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_cubicBezier);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.StartPoint);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point1);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point2);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point3);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point3;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_cubicBezier);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point1);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_cubicBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _cubicBezier.Point1 = point1;
            if (_cubicBezier.Point1.Owner == null)
            {
                _cubicBezier.Point1.Owner = _cubicBezier;
            }
            context.ContainerView?.CurrentContainer.Shapes.Add(_cubicBezier);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _cubicBezier = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);

            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _cubicBezier.Point2 = point2;
            if (_cubicBezier.Point2.Owner == null)
            {
                _cubicBezier.Point2.Owner = _cubicBezier;
            }
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);

            IPointShape point3 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _cubicBezier.Point3 = point3;
            if (_cubicBezier.Point3.Owner == null)
            {
                _cubicBezier.Point3.Owner = _cubicBezier;
            }
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point3);

            CurrentState = State.Point2;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;
            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;
            _cubicBezier.Point3.X = x;
            _cubicBezier.Point3.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_cubicBezier != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_cubicBezier);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point1);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
                _cubicBezier = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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
                case State.Point3:
                    {
                        Point3Internal(context, x, y, modifier);
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
                case State.Point3:
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
                case State.Point3:
                    {
                        MovePoint3Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseTool : BaseTool, ITool
    {
        private EllipseToolSettings _settings;
        private EllipseShape _ellipse = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Ellipse";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EllipseToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape topLeft = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _ellipse = new EllipseShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_ellipse.TopLeft.Owner == null)
            {
                _ellipse.TopLeft.Owner = _ellipse;
            }
            if (_ellipse.BottomRight.Owner == null)
            {
                _ellipse.BottomRight.Owner = _ellipse;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_ellipse);
            context.ContainerView?.SelectionState?.Select(_ellipse.TopLeft);
            context.ContainerView?.SelectionState?.Select(_ellipse.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_ellipse.BottomRight);

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);

            _ellipse.BottomRight = bottomRight;
            if (_ellipse.BottomRight.Owner == null)
            {
                _ellipse.BottomRight.Owner = _ellipse;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_ellipse);
            context.ContainerView?.SelectionState?.Deselect(_ellipse.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_ellipse);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _ellipse = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _ellipse.BottomRight.X = x;
            _ellipse.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            FiltersClear(context);

            if (_ellipse != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_ellipse);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.BottomRight);
                _ellipse = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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

    [DataContract(IsReference = true)]
    public class LineTool : BaseTool, ITool
    {
        private LineToolSettings _settings;
        private LineShape _line = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public static IList<LineShape> SplitByIntersections(IToolContext context, IEnumerable<IPointIntersection> intersections, LineShape target)
        {
            var points = new List<IPointShape>(intersections.SelectMany(i => i.Intersections));
            points.Insert(0, target.StartPoint);
            points.Insert(points.Count, target.Point);

            var unique = new List<IPointShape>(
                points.Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                      .Select(p => new PointShape(p.X, p.Y, context.PointTemplate)));

            var lines = new ObservableCollection<LineShape>();
            for (int i = 0; i < unique.Count - 1; i++)
            {
                var startPoint = unique[i];
                var point = unique[i + 1];
                var line = new LineShape(startPoint, point)
                {
                    Points = new ObservableCollection<IPointShape>(),
                    StyleId = context.StyleLibrary?.CurrentStyle?.Title
                };
                line.StartPoint.Owner = line;
                line.Point.Owner = line;
                context.ContainerView?.CurrentContainer.Shapes.Add(line);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                lines.Add(line);
            }

            return lines;
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = point,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_line.StartPoint.Owner == null)
            {
                _line.StartPoint.Owner = _line;
            }
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_line);
            context.ContainerView?.SelectionState?.Select(_line.StartPoint);
            context.ContainerView?.SelectionState?.Select(_line.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_line);
            context.ContainerView?.SelectionState?.Deselect(_line.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_line.Point);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _line.Point = point;
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }

            IntersectionsClear(context);
            IntersectionsFind(context, _line);

            if ((Settings?.SplitIntersections ?? false) && HaveIntersections())
            {
                SplitByIntersections(context, Intersections, _line);
            }
            else
            {
                context.ContainerView?.CurrentContainer.Shapes.Add(_line);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _line = null;

            IntersectionsClear(context);
            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _line.Point.X = x;
            _line.Point.Y = y;

            IntersectionsClear(context);
            IntersectionsFind(context, _line);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            IntersectionsClear(context);
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_line);
                context.ContainerView?.SelectionState?.Deselect(_line.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_line.Point);
                _line = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
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
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class MoveTool : BaseTool, ITool
    {
        private MoveToolSettings _settings;
        private PathTool _pathTool;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathTool PathTool
        {
            get => _pathTool;
            set => Update(ref _pathTool, value);
        }

        [IgnoreDataMember]
        public string Title => "Move";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public MoveToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public MoveTool(PathTool pathTool)
        {
            PathTool = pathTool;
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            PathTool.Move(context);
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Clean(IToolContext context)
        {
        }
    }

    [DataContract(IsReference = true)]
    public class NoneTool : BaseTool, ITool
    {
        private NoneToolSettings _settings;

        [IgnoreDataMember]
        public string Title => "None";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public NoneToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Clean(IToolContext context)
        {
        }
    }

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
        public EditMode Mode
        {
            get => _context.Mode;
            set => throw new InvalidOperationException($"Can not set {Mode} property value.");
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

    [DataContract(IsReference = true)]
    public class PointTool : BaseTool, ITool
    {
        private PointToolSettings _settings;

        [IgnoreDataMember]
        public string Title => "Point";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            var shape = context.HitTest?.TryToGetShape(
                context.ContainerView?.CurrentContainer.Shapes,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0);
            if (shape != null && (Settings?.ConnectPoints ?? false))
            {
                if (shape is ConnectableShape connectable)
                {
                    var point = new PointShape(x, y, context.PointTemplate);
                    point.Owner = connectable;
                    connectable.Points.Add(point);
                    context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                    context.ContainerView?.SelectionState?.Select(point);
                    context.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
#if USE_CONTAINER_POINTS
            else
            {
                if (context.ContainerView?.CurrentContainer != null)
                {
                    var point = new PointShape(x, y, context.PointTemplate);
                    point.Owner = context.ContainerView?.CurrentContainer;

                    context.ContainerView?.CurrentContainer.Shapes.Add(point);
                    context.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
#endif
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            PointInternal(context, x, y, modifier);
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
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

    [DataContract(IsReference = true)]
    public class PolyLineTool : BaseTool, ITool
    {
        private PolyLineToolSettings _settings;
        private LineShape _line = null;
        private IList<IPointShape> _points = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "PolyLine";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PolyLineToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            IPointShape point = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _points = new ObservableCollection<IPointShape>();
            _line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = point,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_line.StartPoint.Owner == null)
            {
                _line.StartPoint.Owner = _line;
            }
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            _points.Add(_line.StartPoint);
            _points.Add(_line.Point);
            context.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_line);
            context.ContainerView?.SelectionState?.Select(_line.StartPoint);
            context.ContainerView?.SelectionState?.Select(_line.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.SelectionState?.Deselect(_line);
            context.ContainerView?.SelectionState?.Deselect(_line.Point);

            IPointShape firstPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _line.Point = firstPoint;
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            _points[_points.Count - 1] = _line.Point;

            if (!context.ContainerView?.SelectionState?.IsSelected(_line.Point) ?? false)
            {
                context.ContainerView?.SelectionState?.Select(_line.Point);
            }

            context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.CurrentContainer.Shapes.Add(_line);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);

            IPointShape startPoint = _points.Last();
            IPointShape nextPoint = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = nextPoint,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_line.Point.Owner == null)
            {
                _line.Point.Owner = _line;
            }
            _points.Add(_line.Point);
            context.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_line);
            context.ContainerView?.SelectionState?.Select(_line.Point);

            IntersectionsClear(context);
            FiltersClear(context);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _line.Point.X = x;
            _line.Point.Y = y;

            IntersectionsClear(context);
            IntersectionsFind(context, _line);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            IntersectionsClear(context);
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
                context.ContainerView?.SelectionState?.Deselect(_line);
                _line = null;
            }

            if (_points != null)
            {
                foreach (var point in _points)
                {
                    context.ContainerView?.SelectionState?.Deselect(point);
                }
                _points = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
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
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierTool : BaseTool, ITool
    {
        private QuadraticBezierToolSettings _settings;
        private QuadraticBezierShape _quadraticBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "QuadraticBezier";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public QuadraticBezierToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape startPoint = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);
            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _quadraticBezier = new QuadraticBezierShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point1 = point1,
                Point2 = point2,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_quadraticBezier.StartPoint.Owner == null)
            {
                _quadraticBezier.StartPoint.Owner = _quadraticBezier;
            }
            if (_quadraticBezier.Point1.Owner == null)
            {
                _quadraticBezier.Point1.Owner = _quadraticBezier;
            }
            if (_quadraticBezier.Point2.Owner == null)
            {
                _quadraticBezier.Point2.Owner = _quadraticBezier;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_quadraticBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.StartPoint);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point1);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point2);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point1);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_quadraticBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            IPointShape point1 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _quadraticBezier.Point1 = point1;
            if (_quadraticBezier.Point1.Owner == null)
            {
                _quadraticBezier.Point1.Owner = _quadraticBezier;
            }
            context.ContainerView?.CurrentContainer.Shapes.Add(_quadraticBezier);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _quadraticBezier = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);

            IPointShape point2 = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            _quadraticBezier.Point2 = point2;
            if (_quadraticBezier.Point2.Owner == null)
            {
                _quadraticBezier.Point2.Owner = _quadraticBezier;
            }
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;
            _quadraticBezier.Point2.X = x;
            _quadraticBezier.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);

            CurrentState = State.StartPoint;

            if (_quadraticBezier != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_quadraticBezier);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point1);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
                _quadraticBezier = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
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

    [DataContract(IsReference = true)]
    public class RectangleTool : BaseTool, ITool
    {
        private RectangleToolSettings _settings;
        private RectangleShape _rectangle = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Rectangle";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public RectangleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape topLeft = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _rectangle = new RectangleShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_rectangle.TopLeft.Owner == null)
            {
                _rectangle.TopLeft.Owner = _rectangle;
            }
            if (_rectangle.BottomRight.Owner == null)
            {
                _rectangle.BottomRight.Owner = _rectangle;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_rectangle);
            context.ContainerView?.SelectionState?.Select(_rectangle.TopLeft);
            context.ContainerView?.SelectionState?.Select(_rectangle.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_rectangle);
            context.ContainerView?.SelectionState?.Deselect(_rectangle.BottomRight);

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);

            _rectangle.BottomRight = bottomRight;
            _rectangle.BottomRight.Y = y;
            if (_rectangle.BottomRight.Owner == null)
            {
                _rectangle.BottomRight.Owner = _rectangle;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_rectangle.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_rectangle);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _rectangle = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            FiltersClear(context);

            if (_rectangle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_rectangle);
                context.ContainerView?.SelectionState?.Deselect(_rectangle.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_rectangle.BottomRight);
                _rectangle = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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

    [DataContract(IsReference = true)]
    public class ScribbleTool : BaseTool, ITool
    {
        private ScribbleToolSettings _settings;
        private PathShape _path = null;
        private FigureShape _figure = null;
        private IPointShape _previousPoint = null;
        private IPointShape _nextPoint = null;

        public enum State
        {
            Start,
            Points
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.Start;

        [IgnoreDataMember]
        public string Title => "Scribble";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ScribbleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void StartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            _path = new PathShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                FillRule = Settings.FillRule,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };

            _figure = new FigureShape()
            {
                Shapes = new ObservableCollection<IBaseShape>(),
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };

            _path.Shapes.Add(_figure);

            _previousPoint = new PointShape(x, y, context.PointTemplate);
            _previousPoint.Owner = null;

            context.ContainerView?.WorkingContainer.Shapes.Add(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Points;
        }

        private void PointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.Start;

            if (Settings?.Simplify ?? true)
            {
                var points = new List<IPointShape>();
                _path.GetPoints(points);
                var distinct = new List<IPointShape>(points.Distinct());
                IList<Vector2> vectors = new List<Vector2>(distinct.Select(p => new Vector2((float)p.X, (float)p.Y)));
                int count = vectors.Count;
                RDP rdp = new RDP();
                BitArray accepted = rdp.DouglasPeucker(vectors, 0, count - 1, Settings?.Epsilon ?? 1.0);
                int removed = 0;
                for (int i = 0; i <= count - 1; ++i)
                {
                    if (!accepted[i])
                    {
                        distinct.RemoveAt(i - removed);
                        ++removed;
                    }
                }

                _figure.Shapes.Clear();
                _figure.MarkAsDirty(true);

                if (distinct.Count >= 2)
                {
                    for (int i = 0; i < distinct.Count - 1; i++)
                    {
                        var line = new LineShape()
                        {
                            Points = new ObservableCollection<IPointShape>(),
                            StartPoint = distinct[i],
                            Point = distinct[i + 1],
                            Text = new Text(),
                            StyleId = context.StyleLibrary?.CurrentStyle?.Title
                        };
                        _figure.Shapes.Add(line);
                    }
                }
            }

            context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            if (_path.Validate(true) == true)
            {
                context.ContainerView?.CurrentContainer.Shapes.Add(_path);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _path = null;
            _figure = null;
            _previousPoint = null;
            _nextPoint = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _nextPoint = new PointShape(x, y, context.PointTemplate);

            var line = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = _previousPoint,
                Point = _nextPoint,
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (line.StartPoint.Owner == null)
            {
                line.StartPoint.Owner = line;
            }
            if (line.Point.Owner == null)
            {
                line.Point.Owner = line;
            }

            _figure.Shapes.Add(line);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);

            _previousPoint = _nextPoint;
            _nextPoint = null;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.Start;

            FiltersClear(context);

            if (_path != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _path = null;
                _figure = null;
                _previousPoint = null;
                _nextPoint = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Start:
                    {
                        StartInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Points:
                    {
                        PointsInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Points:
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
                case State.Start:
                    {
                        MoveStartInternal(context, x, y, modifier);
                    }
                    break;
                case State.Points:
                    {
                        MovePointsInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class SelectionTool : BaseTool, ITool, ISelection
    {
        private SelectionToolSettings _settings;
        private RectangleShape _rectangle;
        private double _originX;
        private double _originY;
        private double _previousX;
        private double _previousY;
        private IList<IBaseShape> _shapesToCopy = null;
        private bool _disconnected = false;

        public enum State
        {
            None,
            Selection,
            Move
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.None;

        [IgnoreDataMember]
        public string Title => "Selection";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _disconnected = false;

            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            FiltersClear(context);
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            context.ContainerView?.SelectionState?.Dehover();

            var selected = TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0,
                modifier);
            if (selected == true)
            {
                context.ContainerView?.InputService?.Capture?.Invoke();

                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Settings?.SelectionModifier ?? Modifier.Control))
                {
                    context.ContainerView?.SelectionState?.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        Points = new ObservableCollection<IPointShape>(),
                        TopLeft = new PointShape(),
                        BottomRight = new PointShape()
                    };
                    _rectangle.TopLeft.Owner = _rectangle;
                    _rectangle.BottomRight.Owner = _rectangle;
                }

                _rectangle.TopLeft.X = x;
                _rectangle.TopLeft.Y = y;
                _rectangle.BottomRight.X = x;
                _rectangle.BottomRight.Y = y;
                _rectangle.StyleId = Settings?.SelectionStyle;
                context.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);

                context.ContainerView?.InputService?.Capture?.Invoke();
                context.ContainerView?.InputService?.Redraw?.Invoke();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.ContainerView?.SelectionState?.Dehover();

            TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                _rectangle.ToRect2(),
                Settings?.HitTestRadius ?? 7.0,
                modifier);

            context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            _rectangle = null;

            CurrentState = State.None;

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null && !(context.ContainerView.SelectionState?.Hovered == null && context.ContainerView.SelectionState?.Shapes.Count > 0))
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    var previous = context.ContainerView?.SelectionState?.Hovered;
                    var target = new Point2(x, y);
                    var shape = TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        Settings?.HitTestRadius ?? 7.0);
                    if (shape != null)
                    {
                        if (shape != previous)
                        {
                            context.ContainerView?.SelectionState?.Dehover();
                            context.ContainerView?.SelectionState?.Hover(shape);
                            context.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.ContainerView?.SelectionState?.Dehover();
                            context.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            double dx = x - _previousX;
            double dy = y - _previousY;

            _previousX = x;
            _previousY = y;

            if (context.ContainerView?.SelectionState != null)
            {
                if (context.ContainerView.SelectionState?.Shapes.Count == 1)
                {
                    var shape = context.ContainerView.SelectionState?.Shapes.FirstOrDefault();

                    if (shape is IPointShape source)
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

                    shape.Move(context.ContainerView.SelectionState, dx, dy);
                }
                else
                {
                    var selectedToDisconnect = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);
                    foreach (var shape in selectedToDisconnect)
                    {
                        if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            if (!(shape is IPointShape) && _disconnected == false)
                            {
                                Disconnect(context, shape);
                            }
                        }
                    }

                    var selectedToMove = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);
                    foreach (var shape in selectedToMove)
                    {
                        shape.Move(context.ContainerView.SelectionState, dx, dy);
                    }
                }

                context.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;

            context.ContainerView?.SelectionState?.Dehover();

            if (_rectangle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.ContainerView?.SelectionState?.Dehover();
                context.ContainerView?.SelectionState?.Clear();
            }

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
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

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
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

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
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

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
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

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }

        public void Cut(IToolContext context)
        {
            Copy(context);
            Delete(context);
        }

        public void Copy(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    _shapesToCopy = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);
                }
            }
        }

        public void Paste(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                if (_shapesToCopy != null)
                {
                    lock (context.ContainerView.SelectionState?.Shapes)
                    {
                        context.ContainerView?.SelectionState?.Dehover();
                        context.ContainerView?.SelectionState?.Clear();

                        Copy(context.ContainerView?.CurrentContainer, _shapesToCopy, context.ContainerView.SelectionState);

                        context.ContainerView?.InputService?.Redraw?.Invoke();

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void Delete(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    Delete(context.ContainerView?.CurrentContainer, context.ContainerView.SelectionState);

                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Group(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes.Reverse());

                    Delete(context);

                    var group = new GroupShape()
                    {
                        Title = "Group",
                        Points = new ObservableCollection<IPointShape>(),
                        Shapes = new ObservableCollection<IBaseShape>()
                    };

                    foreach (var shape in shapes)
                    {
                        if (!(shape is IPointShape))
                        {
                            group.Shapes.Add(shape);
                        }
                    }

                    group.Select(context.ContainerView.SelectionState);
                    context.ContainerView?.CurrentContainer.Shapes.Add(group);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Reference(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = new List<IBaseShape>(context.ContainerView.SelectionState?.Shapes);

                    foreach (var shape in shapes)
                    {
                        if (shape is GroupShape group)
                        {
                            context.ContainerView?.Reference(group);
                        }
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void SelectAll(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState?.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    foreach (var shape in context.ContainerView?.CurrentContainer.Shapes)
                    {
                        shape.Select(context.ContainerView.SelectionState);
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Connect(IToolContext context, IPointShape point)
        {
            var target = context.HitTest?.TryToGetPoint(
                context.ContainerView?.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                point);
            if (target != point)
            {
                foreach (var item in context.ContainerView?.CurrentContainer.Shapes)
                {
                    if (item is ConnectableShape connectable)
                    {
                        if (connectable.Connect(point, target))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, IPointShape point)
        {
            foreach (var shape in context.ContainerView?.CurrentContainer.Shapes)
            {
                if (shape is ConnectableShape connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.ContainerView?.SelectionState?.Deselect(point);
                            context.ContainerView?.SelectionState?.Select(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, IBaseShape shape)
        {
            if (shape is ConnectableShape connectable)
            {
                if (context.ContainerView?.SelectionState != null)
                {
                    connectable.Deselect(context.ContainerView.SelectionState);
                }
                _disconnected = connectable.Disconnect();
                if (context.ContainerView?.SelectionState != null)
                {
                    connectable.Select(context.ContainerView.SelectionState);
                }
            }
        }

        internal Dictionary<object, object> GetPointsCopyDict(IEnumerable<IBaseShape> shapes)
        {
            var copy = new Dictionary<object, object>();

            var points = new List<IPointShape>();

            foreach (var shape in shapes)
            {
                shape.GetPoints(points);
            }

            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                copy[point] = point.Copy(null);
            }

            return copy;
        }

        internal void Copy(ICanvasContainer container, IEnumerable<IBaseShape> shapes, ISelectionState selectionState)
        {
            var shared = GetPointsCopyDict(shapes);
            var points = new List<IPointShape>();

            foreach (var shape in shapes)
            {
                if (shape is ICopyable copyable)
                {
                    var copy = (IBaseShape)(copyable.Copy(shared));
                    if (copy != null && !(copy is IPointShape))
                    {
                        copy.GetPoints(points);
                        copy.Select(selectionState);
                        container.Shapes.Add(copy);
                    }
                }
            }

            foreach (var point in points)
            {
                if (point.Owner != null)
                {
                    if (shared.TryGetValue(point.Owner, out var owner))
                    {
                        point.Owner = owner;
                    }
                    else
                    {
#if DEBUG
                        Console.WriteLine($"Failed to find owner shape: {point.Owner} for point: {point}.");
#endif
                    }
                }
            }
        }

        internal void Delete(ICanvasContainer container, ISelectionState selectionState)
        {
            // TODO: Very slow when using Contains.
            //var paths = new List<PathShape>(container.Shapes.OfType<PathShape>());
            //var groups = new List<GroupShape>(container.Shapes.OfType<GroupShape>());
            //var connectables = new List<ConnectableShape>(container.Shapes.OfType<ConnectableShape>());

            var shapesHash = new HashSet<IBaseShape>(container.Shapes);

            foreach (var shape in selectionState.Shapes)
            {
                if (shapesHash.Contains(shape))
                {
                    container.Shapes.Remove(shape);
                    container.MarkAsDirty(true);
                }
                /*
                else
                {
                    if (shape is IPointShape point)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(connectables, point);
                    }

                    if (paths.Count > 0)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(container, paths, shape);
                    }

                    if (groups.Count > 0)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(container, groups, shape);
                    }
                }
                */
            }
        }

        internal bool TryToDelete(IReadOnlyList<ConnectableShape> connectables, IPointShape point)
        {
            foreach (var connectable in connectables)
            {
                if (connectable.Points.Contains(point))
                {
                    connectable.Points.Remove(point);
                    connectable.MarkAsDirty(true);

                    return true;
                }
            }

            return false;
        }

        internal bool TryToDelete(ICanvasContainer container, IReadOnlyList<PathShape> paths, IBaseShape shape)
        {
            foreach (var path in paths)
            {
                foreach (var pathShape in path.Shapes)
                {
                    if (pathShape is FigureShape figure)
                    {
                        if (figure.Shapes.Contains(shape))
                        {
                            figure.Shapes.Remove(shape);
                            figure.MarkAsDirty(true);

                            if (figure.Shapes.Count <= 0)
                            {
                                path.Shapes.Remove(figure);
                                path.MarkAsDirty(true);

                                if (path.Shapes.Count <= 0)
                                {
                                    container.Shapes.Remove(path);
                                    container.MarkAsDirty(true);
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal bool TryToDelete(ICanvasContainer container, IReadOnlyList<GroupShape> groups, IBaseShape shape)
        {
            foreach (var group in groups)
            {
                if (group.Shapes.Contains(shape))
                {
                    group.Shapes.Remove(shape);
                    group.MarkAsDirty(true);

                    if (group.Shapes.Count <= 0)
                    {
                        container.Shapes.Remove(group);
                        container.MarkAsDirty(true);
                    }

                    return true;
                }
            }

            return false;
        }

        internal IBaseShape TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetPoint(context.ContainerView?.CurrentContainer.Shapes, target, radius, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.HitTest?.TryToGetShape(context.ContainerView?.CurrentContainer.Shapes, target, radius) : null;

            if (shapePoint != null || shape != null)
            {
                if (shapePoint != null)
                {
                    return shapePoint;
                }
                else if (shape != null)
                {
                    return shape;
                }
            }

            return null;
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Point2 point, double radius, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                var shapePoint =
                    mode.HasFlag(SelectionMode.Point)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetPoint(context.ContainerView?.CurrentContainer.Shapes, point, radius, null) : null;

                var shape =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetShape(context.ContainerView?.CurrentContainer.Shapes, point, radius) : null;

                if (shapePoint != null || shape != null)
                {
                    bool haveNewSelection =
                        (shapePoint != null && !(context.ContainerView.SelectionState?.IsSelected(shapePoint) ?? false))
                        || (shape != null && !(context.ContainerView.SelectionState?.IsSelected(shape) ?? false));

                    if (context.ContainerView.SelectionState?.Shapes.Count >= 1
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
                                if (context.ContainerView.SelectionState?.IsSelected(shapePoint) ?? false)
                                {
                                    shapePoint.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shapePoint.Select(context.ContainerView.SelectionState);
                                }
                                return context.ContainerView.SelectionState?.Shapes.Count > 0;
                            }
                            else
                            {
                                context.ContainerView.SelectionState?.Clear();
                                shapePoint.Select(context.ContainerView.SelectionState);
                                return true;
                            }
                        }
                        else if (shape != null)
                        {
                            if (modifier.HasFlag(selectionModifier))
                            {
                                if (context.ContainerView.SelectionState?.IsSelected(shape) ?? false)
                                {
                                    shape.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.ContainerView.SelectionState);
                                }
                                return context.ContainerView.SelectionState?.Shapes.Count > 0;
                            }
                            else
                            {
                                context.ContainerView.SelectionState?.Clear();
                                shape.Select(context.ContainerView.SelectionState);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Rect2 rect, double radius, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                var shapes =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.HitTest?.TryToGetShapes(context.ContainerView?.CurrentContainer.Shapes, rect, radius) : null;

                if (shapes != null)
                {
                    if (shapes != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            foreach (var shape in shapes)
                            {
                                if (context.ContainerView.SelectionState?.IsSelected(shape) ?? false)
                                {
                                    shape.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.ContainerView.SelectionState);
                                }
                            }
                            return context.ContainerView.SelectionState?.Shapes.Count > 0;
                        }
                        else
                        {
                            context.ContainerView.SelectionState?.Clear();
                            foreach (var shape in shapes)
                            {
                                shape.Select(context.ContainerView.SelectionState);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    [DataContract(IsReference = true)]
    public class TextTool : BaseTool, ITool
    {
        private TextToolSettings _settings;
        private TextShape _text = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Text";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TextToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            IPointShape topLeft = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, false, 0.0);

            _text = new TextShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = new Text("Text"),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            if (_text.TopLeft.Owner == null)
            {
                _text.TopLeft.Owner = _text;
            }
            if (_text.BottomRight.Owner == null)
            {
                _text.BottomRight.Owner = _text;
            }
            context.ContainerView?.WorkingContainer.Shapes.Add(_text);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_text);
            context.ContainerView?.SelectionState?.Select(_text.TopLeft);
            context.ContainerView?.SelectionState?.Select(_text.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_text);
            context.ContainerView?.SelectionState?.Deselect(_text.BottomRight);

            IPointShape bottomRight = context.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);

            _text.BottomRight = bottomRight;
            _text.BottomRight.Y = y;
            if (_text.BottomRight.Owner == null)
            {
                _text.BottomRight.Owner = _text;
            }
            context.ContainerView?.WorkingContainer.Shapes.Remove(_text);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_text.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_text);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _text = null;

            FiltersClear(context);

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _text.BottomRight.X = x;
            _text.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            FiltersClear(context);

            if (_text != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_text);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_text);
                context.ContainerView?.SelectionState?.Deselect(_text.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_text.BottomRight);
                _text = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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
                case State.BottomRight:
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
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
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
}

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class ToolContext : ViewModelBase, IToolContext
    {
        private IStyleLibrary _styleLibrary;
        private IGroupLibrary _groupLibrary;
        private IBaseShape _pointTemplate;
        private IHitTest _hitTest;
        private IList<IContainerView> _containerViews;
        private IContainerView _containerView;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private EditMode _mode;

#if USE_SERIALIZE_STYLES
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
#else
        [IgnoreDataMember]
#endif
        public IStyleLibrary StyleLibrary
        {
            get => _styleLibrary;
            set => Update(ref _styleLibrary, value);
        }

#if USE_SERIALIZE_GROUPS
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
#else
        [IgnoreDataMember]
#endif
        public IGroupLibrary GroupLibrary
        {
            get => _groupLibrary;
            set => Update(ref _groupLibrary, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape PointTemplate
        {
            get => _pointTemplate;
            set => Update(ref _pointTemplate, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IHitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IContainerView> ContainerViews
        {
            get => _containerViews;
            set => Update(ref _containerViews, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IContainerView ContainerView
        {
            get => _containerView;
            set => Update(ref _containerView, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool?.Clean(this);
                Update(ref _currentTool, value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EditMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public void Dispose()
        {
            if (_containerViews != null)
            {
                foreach (var containerView in _containerViews)
                {
                    containerView.DrawContainerView?.Dispose();
                    containerView.DrawContainerView = null;
                    containerView.SelectionState = null;
                    containerView.WorkingContainer = null;
                }
            }
        }

        public void SetTool(string title)
        {
            if (CurrentTool is PathTool pathTool && pathTool.Settings.CurrentTool.Title != title)
            {
                pathTool.CleanCurrentTool(this);
                var tool = pathTool.Settings.Tools.Where(t => t.Title == title).FirstOrDefault();
                if (tool != null)
                {
                    pathTool.Settings.CurrentTool = tool;
                }
                else
                {
                    CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
                }
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
            }
        }

        public void LeftDown(double x, double y, Modifier modifier)
        {
            _currentTool.LeftDown(this, x, y, modifier);
        }

        public void LeftUp(double x, double y, Modifier modifier)
        {
            if (_mode == EditMode.Mouse)
            {
                _currentTool.LeftUp(this, x, y, modifier);
            }
            else if (_mode == EditMode.Touch)
            {
                _currentTool.LeftDown(this, x, y, modifier);
            }
        }

        public void RightDown(double x, double y, Modifier modifier)
        {
            _currentTool.RightDown(this, x, y, modifier);
        }

        public void RightUp(double x, double y, Modifier modifier)
        {
            _currentTool.RightUp(this, x, y, modifier);
        }

        public void Move(double x, double y, Modifier modifier)
        {
            _currentTool.Move(this, x, y, modifier);
        }

        public double GetWidth()
        {
            return ContainerView?.Width ?? 0.0;
        }

        public double GetHeight()
        {
            return ContainerView?.Height ?? 0.0;
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new ToolContext()
            {
                Name = this.Name
            };

            return copy;
        }
    }
}
