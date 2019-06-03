// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define DEBUG_CONNECTORS
//#define USE_POINT_DECORATOR
//#define USE_GROUP_SHAPES
//#define USE_PATH_FIGURES
//#define USE_CONTAINER_POINTS
//#define USE_CONTAINER_SHAPES
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

namespace Draw2D.ViewModels.Shapes
{
    public enum PathFillRule
    {
        EvenOdd,
        Nonzero
    }

    [DataContract(IsReference = true)]
    public abstract class BaseShape : ViewModelBase, IBaseShape
    {
        internal static IBounds s_bounds = null;
        internal static IShapeDecorator s_decorator = null;

        private string _styleId;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string StyleId
        {
            get => _styleId;
            set => Update(ref _styleId, value);
        }

        [IgnoreDataMember]
        public virtual IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public virtual IShapeDecorator Decorator { get; } = s_decorator;

        public abstract void GetPoints(IList<IPointShape> points);

        public abstract void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r);

        public abstract void Move(ISelectionState selectionState, double dx, double dy);

        public virtual void Select(ISelectionState selectionState)
        {
            if (!selectionState.IsSelected(this))
            {
                selectionState.Select(this);
            }
        }

        public virtual void Deselect(ISelectionState selectionState)
        {
            if (selectionState.IsSelected(this))
            {
                selectionState.Deselect(this);
            }
        }

        public abstract object Copy(Dictionary<object, object> shared);
    }

    [DataContract(IsReference = true)]
    public abstract class ConnectableShape : BaseShape, IConnectable
    {
        internal static new IBounds s_bounds = null;
        internal static new IShapeDecorator s_decorator = null;

        private IList<IPointShape> _points;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ConnectableShape()
        {
        }

        public ConnectableShape(IList<IPointShape> points)
        {
            this.Points = points;
        }

        public override void Invalidate()
        {
            if (_points != null)
            {
                foreach (var point in _points)
                {
                    point.Invalidate();
                }
            }

            base.Invalidate();
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            if (_points != null)
            {
                foreach (var point in _points)
                {
                    points.Add(point);
                }
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var point in Points)
                {
                    if (renderer.SelectionState?.IsSelected(point) ?? false)
                    {
                        point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                    }
                }
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            foreach (var point in Points)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);

            foreach (var point in Points)
            {
                point.Select(selectionState);
            }
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);

            foreach (var point in Points)
            {
                point.Deselect(selectionState);
            }
        }

        private bool CanConnect(IPointShape point)
        {
            return _points.Contains(point) == false;
        }

        public virtual bool Connect(IPointShape point, IPointShape target)
        {
            if (CanConnect(point))
            {
                int index = _points.IndexOf(target);
                if (index >= 0)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(ConnectableShape)} Connected to Points");
#endif
                    _points[index] = point;
                    return true;
                }
            }
            return false;
        }

        public virtual bool Disconnect(IPointShape point, out IPointShape result)
        {
            result = null;
            return false;
        }

        public virtual bool Disconnect()
        {
            bool result = false;

            for (int i = 0; i < _points.Count; i++)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConnectableShape)}: Disconnected from {nameof(Points)} #{i}");
#endif
                _points[i] = (IPointShape)_points[i].Copy(null);
                result = true;
            }

            return result;
        }
    }

    [DataContract(IsReference = true)]
    public abstract class BoxShape : ConnectableShape
    {
        internal static new IBounds s_bounds = null;
        internal static new IShapeDecorator s_decorator = null;

        private IPointShape _topLeft;
        private IPointShape _bottomRight;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        public BoxShape()
            : base()
        {
        }

        public BoxShape(IPointShape topLeft, IPointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(TopLeft);
            points.Add(BottomRight);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            _topLeft?.Invalidate();

            _bottomRight?.Invalidate();

            base.Invalidate();
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_topLeft))
            {
                _topLeft.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_bottomRight))
            {
                _bottomRight.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            TopLeft.Select(selectionState);
            BottomRight.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            TopLeft.Deselect(selectionState);
            BottomRight.Deselect(selectionState);
        }

        private bool CanConnect(IPointShape point)
        {
            return TopLeft != point
                && BottomRight != point;
        }

        public override bool Connect(IPointShape point, IPointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (TopLeft == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(BoxShape)}: Connected to {nameof(TopLeft)}");
#endif
                    this.TopLeft = point;
                    return true;
                }
                else if (BottomRight == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(BoxShape)}: Connected to {nameof(BottomRight)}");
#endif
                    this.BottomRight = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(IPointShape point, out IPointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (TopLeft == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(TopLeft)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.TopLeft = result;
                return true;
            }
            else if (BottomRight == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(BottomRight)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.BottomRight = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.TopLeft != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(TopLeft)}");
#endif
                this.TopLeft = (IPointShape)(this.TopLeft.Copy(null));
                result = true;
            }

            if (this.BottomRight != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(BottomRight)}");
#endif
                this.BottomRight = (IPointShape)(this.BottomRight.Copy(null));
                result = true;
            }

            return result;
        }
    }

    [DataContract(IsReference = true)]
    public class ConicShape : ConnectableShape
    {
        internal static new IBounds s_bounds = new ConicBounds();
        internal static new IShapeDecorator s_decorator = new ConicDecorator();

        private IPointShape _startPoint;
        private IPointShape _point1;
        private IPointShape _point2;
        private double _weight;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Weight
        {
            get => _weight;
            set => Update(ref _weight, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public ConicShape()
            : base()
        {
        }

        public ConicShape(IPointShape startPoint, IPointShape point1, IPointShape point2, double weight)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
            this.Weight = weight;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point1);
            points.Add(Point2);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            _startPoint?.Invalidate();

            _point1?.Invalidate();

            _point2?.Invalidate();

            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawConic(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(_startPoint) ?? false)
                {
                    _startPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point1) ?? false)
                {
                    _point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point2) ?? false)
                {
                    _point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point1))
            {
                _point1.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point2))
            {
                _point2.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point1.Select(selectionState);
            Point2.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point1.Deselect(selectionState);
            Point2.Deselect(selectionState);
        }

        private bool CanConnect(IPointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point;
        }

        public override bool Connect(IPointShape point, IPointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(ConicShape)}: Connected to {nameof(StartPoint)}");
#endif
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(ConicShape)}: Connected to {nameof(Point1)}");
#endif
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(ConicShape)}: Connected to {nameof(Point2)}");
#endif
                    this.Point2 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(IPointShape point, out IPointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point1)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point2)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point2 = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point1 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point1)}");
#endif
                this.Point1 = (IPointShape)(this.Point1.Copy(null));
                result = true;
            }

            if (this.Point2 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point2)}");
#endif
                this.Point2 = (IPointShape)(this.Point2.Copy(null));
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new ConicShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared),
                Weight = this.Weight
            };

            if (shared != null)
            {
                copy.StartPoint = (IPointShape)shared[this.StartPoint];
                copy.Point1 = (IPointShape)shared[this.Point1];
                copy.Point2 = (IPointShape)shared[this.Point2];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class CubicBezierShape : ConnectableShape
    {
        internal static new IBounds s_bounds = new CubicBezierBounds();
        internal static new IShapeDecorator s_decorator = new CubicBezierDecorator();

        private IPointShape _startPoint;
        private IPointShape _point1;
        private IPointShape _point2;
        private IPointShape _point3;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public CubicBezierShape()
            : base()
        {
        }

        public CubicBezierShape(IPointShape startPoint, IPointShape point1, IPointShape point2, IPointShape point3)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
            this.Point3 = point3;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point1);
            points.Add(Point2);
            points.Add(Point3);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            _startPoint?.Invalidate();

            _point1?.Invalidate();

            _point2?.Invalidate();

            _point3?.Invalidate();

            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawCubicBezier(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(_startPoint) ?? false)
                {
                    _startPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point1) ?? false)
                {
                    _point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point2) ?? false)
                {
                    _point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point3) ?? false)
                {
                    _point3.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point1))
            {
                _point1.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point2))
            {
                _point2.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point3))
            {
                _point3.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point1.Select(selectionState);
            Point2.Select(selectionState);
            Point3.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point1.Deselect(selectionState);
            Point2.Deselect(selectionState);
            Point3.Deselect(selectionState);
        }

        private bool CanConnect(IPointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point
                && Point3 != point;
        }

        public override bool Connect(IPointShape point, IPointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(StartPoint)}");
#endif
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(Point1)}");
#endif
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(Point2)}");
#endif
                    this.Point2 = point;
                    return true;
                }
                else if (Point3 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(Point3)}");
#endif
                    this.Point3 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(IPointShape point, out IPointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point1)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point2)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point2 = result;
                return true;
            }
            else if (Point3 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point3)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point3 = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point1 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point1)}");
#endif
                this.Point1 = (IPointShape)(this.Point1.Copy(null));
                result = true;
            }

            if (this.Point2 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point2)}");
#endif
                this.Point2 = (IPointShape)(this.Point2.Copy(null));
                result = true;
            }

            if (this.Point3 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point3)}");
#endif
                this.Point3 = (IPointShape)this.Point3.Copy(null);
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new CubicBezierShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (IPointShape)shared[this.StartPoint];
                copy.Point1 = (IPointShape)shared[this.Point1];
                copy.Point2 = (IPointShape)shared[this.Point2];
                copy.Point3 = (IPointShape)shared[this.Point3];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseShape : BoxShape
    {
        internal static new IBounds s_bounds = new EllipseBounds();
        internal static new IShapeDecorator s_decorator = new EllipseDecorator();

        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(IPointShape topLeft, IPointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override void Invalidate()
        {
            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawEllipse(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(TopLeft) ?? false)
                {
                    TopLeft.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(BottomRight) ?? false)
                {
                    BottomRight.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new EllipseShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (IPointShape)shared[this.TopLeft];
                copy.BottomRight = (IPointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class FigureShape : GroupShape, ICanvasContainer
    {
        internal static new IBounds s_bounds = new FigureBounds();
        internal static new IShapeDecorator s_decorator = new FigureDecorator();

        private bool _isFilled;
        private bool _isClosed;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

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

        public FigureShape()
            : base()
        {
        }

        public FigureShape(IList<IBaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public FigureShape(string title)
            : this()
        {
            this.Title = title;
        }

        public FigureShape(string title, IList<IBaseShape> shapes)
            : base()
        {
            this.Title = title;
            this.Shapes = shapes;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new FigureShape()
            {
                Name = this.Name,
                Title = this.Title,
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                StyleId = this.StyleId,
                IsFilled = this.IsFilled,
                IsClosed = this.IsClosed
            };

            if (shared != null)
            {
                if (this.Points != null)
                {
                    foreach (var point in this.Points)
                    {
                        copy.Points.Add((IPointShape)shared[point]);
                    }
                }

                if (this.Shapes != null)
                {
                    foreach (var shape in this.Shapes)
                    {
                        if (shape is ICopyable copyable)
                        {
                            copy.Shapes.Add((IBaseShape)(copyable.Copy(shared)));
                        }
                    }
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class GroupShape : ConnectableShape, ICanvasContainer
    {
        internal static new IBounds s_bounds = new GroupBounds();
        internal static new IShapeDecorator s_decorator = new GroupDecorator();

        private string _title;
        private IList<IBaseShape> _shapes;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public GroupShape()
            : base()
        {
        }

        public GroupShape(IList<IBaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public GroupShape(string title)
            : this()
        {
            this.Title = title;
        }

        public GroupShape(string title, IList<IBaseShape> shapes)
            : base()
        {
            this.Title = title;
            this.Shapes = shapes;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            base.GetPoints(points);

            foreach (var shape in Shapes)
            {
                shape.GetPoints(points);
            }
        }

        public override void Invalidate()
        {
            foreach (var shape in Shapes)
            {
                shape.Invalidate();
            }

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (Shapes != null)
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            var points = new List<IPointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }

            base.Move(selectionState, dx, dy);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new GroupShape()
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
    public class LineShape : ConnectableShape
    {
        internal static new IBounds s_bounds = new LineBounds();
        internal static new IShapeDecorator s_decorator = new LineDecorator();

        private IPointShape _startPoint;
        private IPointShape _point;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public LineShape()
            : base()
        {
        }

        public LineShape(IPointShape startPoint, IPointShape point)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point = point;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            _startPoint?.Invalidate();

            _point?.Invalidate();

            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawLine(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(_startPoint) ?? false)
                {
                    _startPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point) ?? false)
                {
                    _point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point))
            {
                _point.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point.Deselect(selectionState);
        }

        private bool CanConnect(IPointShape point)
        {
            return StartPoint != point
                && Point != point;
        }

        public override bool Connect(IPointShape point, IPointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(LineShape)}: Connected to {nameof(StartPoint)}");
#endif
                    this.StartPoint = point;
                    return true;
                }
                else if (Point == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(LineShape)}: Connected to {nameof(Point)}");
#endif
                    this.Point = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(IPointShape point, out IPointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(Point)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(Point)}");
#endif
                this.Point = (IPointShape)(this.Point.Copy(null));
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (IPointShape)shared[this.StartPoint];
                copy.Point = (IPointShape)shared[this.Point];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class PathShape : GroupShape, ICanvasContainer
    {
        internal static new IBounds s_bounds = new PathBounds();
        internal static new IShapeDecorator s_decorator = new PathDecorator();

        private PathFillRule _fillRule;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public PathShape()
            : base()
        {
        }

        public PathShape(IList<IBaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public PathShape(string title, IList<IBaseShape> shapes)
            : base()
        {
            this.Title = title;
            this.Shapes = shapes;
        }

        public IPointShape GetFirstPoint()
        {
            if (Shapes.Count > 0)
            {
                var lastShape = Shapes[Shapes.Count - 1];
                if (lastShape is FigureShape lastFigure)
                {
                    var shapes = lastFigure.Shapes;
                    if (shapes.Count > 0)
                    {
                        switch (shapes[0])
                        {
                            case LineShape line:
                                return line.StartPoint;
                            case CubicBezierShape cubicBezier:
                                return cubicBezier.StartPoint;
                            case QuadraticBezierShape quadraticBezier:
                                return quadraticBezier.StartPoint;
                            case ConicShape conic:
                                return conic.StartPoint;
                            default:
                                throw new Exception("Could not find last path point.");
                        }
                    }
                }
            }
            return null;
        }

        public IPointShape GetLastPoint()
        {
            if (Shapes.Count > 0)
            {
                var shape = Shapes[Shapes.Count - 1];
                if (shape is FigureShape lastFigure)
                {
                    var lastFigureShapes = lastFigure.Shapes;
                    if (lastFigureShapes.Count > 0)
                    {
                        switch (lastFigureShapes[lastFigureShapes.Count - 1])
                        {
                            case LineShape line:
                                return line.Point;
                            case CubicBezierShape cubicBezier:
                                return cubicBezier.Point3;
                            case QuadraticBezierShape quadraticBezier:
                                return quadraticBezier.Point2;
                            case ConicShape conic:
                                return conic.Point2;
                            default:
                                throw new Exception("Could not find last path point.");
                        }
                    }
                }
            }
            return null;
        }

        public override void Invalidate()
        {
            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            var isPathSelected = renderer.SelectionState?.IsSelected(this) ?? false;

            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawPath(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var shape in Shapes)
                {
                    if (shape is FigureShape figure)
                    {
                        DrawPoints(dc, renderer, dx, dy, scale, mode, db, r, figure, isPathSelected);
                    }
                }
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var point in Points)
                {
                    if (renderer.SelectionState?.IsSelected(point) ?? false)
                    {
                        point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                    }
                }
            }
        }

        private void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r, FigureShape figure, bool isPathSelected)
        {
            foreach (var shape in figure.Shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(line) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(line.StartPoint) ?? false))
                            {
                                line.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(line.Point) ?? false))
                            {
                                line.Point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in line.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                    case CubicBezierShape cubic:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(cubic) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.StartPoint) ?? false))
                            {
                                cubic.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.Point1) ?? false))
                            {
                                cubic.Point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.Point2) ?? false))
                            {
                                cubic.Point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(cubic.Point3) ?? false))
                            {
                                cubic.Point3.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in cubic.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                    case QuadraticBezierShape quadratic:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(quadratic) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(quadratic.StartPoint) ?? false))
                            {
                                quadratic.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(quadratic.Point1) ?? false))
                            {
                                quadratic.Point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(quadratic.Point2) ?? false))
                            {
                                quadratic.Point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in quadratic.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                    case ConicShape conic:
                        {
                            var isSelected = renderer.SelectionState?.IsSelected(conic) ?? false;

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(conic.StartPoint) ?? false))
                            {
                                conic.StartPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(conic.Point1) ?? false))
                            {
                                conic.Point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(conic.Point2) ?? false))
                            {
                                conic.Point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                            }

                            foreach (var point in conic.Points)
                            {
                                if (isPathSelected || isSelected || (renderer.SelectionState?.IsSelected(point) ?? false))
                                {
                                    point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);

            var points = new List<IPointShape>();
            GetPoints(points);

            foreach (var point in points)
            {
                point.Select(selectionState);
            }
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);

            var points = new List<IPointShape>();
            GetPoints(points);

            foreach (var point in points)
            {
                point.Deselect(selectionState);
            }
        }

        public bool Validate(bool removeEmptyFigures)
        {
            var figures = new List<FigureShape>();

            foreach (var shape in Shapes)
            {
                if (shape is FigureShape figure)
                {
                    figures.Add(figure);
                }
            }

            if (figures.Count > 0 && figures[0].Shapes.Count > 0)
            {
                if (removeEmptyFigures == true)
                {
                    foreach (var figure in figures)
                    {
                        if (figure.Shapes.Count <= 0)
                        {
                            Shapes.Remove(figure);
                            this.MarkAsDirty(true);
                        }
                    }
                }

                if (Shapes.Count > 0 && Shapes[0] is FigureShape figureShape && figureShape.Shapes.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new PathShape()
            {
                Name = this.Name,
                Title = this.Title,
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>(),
                StyleId = this.StyleId,
                FillRule = this.FillRule,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                foreach (var shape in this.Shapes)
                {
                    copy.Shapes.Add((IBaseShape)(shape.Copy(shared)));
                }

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class PointShape : BaseShape, IPointShape
    {
        internal static new IBounds s_bounds = new PointBounds();
        internal static new IShapeDecorator s_decorator = new PointDecorator();

        private double _x;
        private double _y;
        private IBaseShape _template;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public PointShape()
        {
        }

        public PointShape(double x, double y, IBaseShape template)
        {
            this.X = x;
            this.Y = y;
            this.Template = template;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(this);
        }

        public override void Invalidate()
        {
            if (_template != null)
            {
                _template.Invalidate();
            }

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (_template != null)
            {
                double offsetX = X;
                double offsetY = Y;
                _template.Draw(dc, renderer, dx + offsetX, dy + offsetY, scale, DrawMode.Shape, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new PointShape()
            {
                StyleId = this.StyleId,
                Owner = this.Owner,
                X = this.X,
                Y = this.Y,
                Template = this.Template
            };
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierShape : ConnectableShape
    {
        internal static new IBounds s_bounds = new QuadraticBezierBounds();
        internal static new IShapeDecorator s_decorator = new QuadraticBezierDecorator();

        private IPointShape _startPoint;
        private IPointShape _point1;
        private IPointShape _point2;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public QuadraticBezierShape()
            : base()
        {
        }

        public QuadraticBezierShape(IPointShape startPoint, IPointShape point1, IPointShape point2)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point1);
            points.Add(Point2);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            _startPoint?.Invalidate();

            _point1?.Invalidate();

            _point2?.Invalidate();

            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawQuadraticBezier(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(_startPoint) ?? false)
                {
                    _startPoint.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point1) ?? false)
                {
                    _point1.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(_point2) ?? false)
                {
                    _point2.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point1))
            {
                _point1.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point2))
            {
                _point2.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point1.Select(selectionState);
            Point2.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point1.Deselect(selectionState);
            Point2.Deselect(selectionState);
        }

        private bool CanConnect(IPointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point;
        }

        public override bool Connect(IPointShape point, IPointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(StartPoint)}");
#endif
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point1)}");
#endif
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
#if DEBUG_CONNECTORS
                    Console.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point2)}");
#endif
                    this.Point2 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(IPointShape point, out IPointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point2 = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point1 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
#endif
                this.Point1 = (IPointShape)(this.Point1.Copy(null));
                result = true;
            }

            if (this.Point2 != null)
            {
#if DEBUG_CONNECTORS
                Console.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
#endif
                this.Point2 = (IPointShape)(this.Point2.Copy(null));
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new QuadraticBezierShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (IPointShape)shared[this.StartPoint];
                copy.Point1 = (IPointShape)shared[this.Point1];
                copy.Point2 = (IPointShape)shared[this.Point2];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleShape : BoxShape
    {
        internal static new IBounds s_bounds = new RectangleBounds();
        internal static new IShapeDecorator s_decorator = new RectangleDecorator();

        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public RectangleShape()
            : base()
        {
        }

        public RectangleShape(IPointShape topLeft, IPointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override void Invalidate()
        {
            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawRectangle(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(TopLeft) ?? false)
                {
                    TopLeft.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(BottomRight) ?? false)
                {
                    BottomRight.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new RectangleShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = (Text)this.Text?.Copy(shared),
                StyleId = this.StyleId
            };

            if (shared != null)
            {
                copy.TopLeft = (IPointShape)shared[this.TopLeft];
                copy.BottomRight = (IPointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class ReferencePointShape : BaseShape, IPointShape
    {
        internal static new IBounds s_bounds = new PointBounds();
        internal static new IShapeDecorator s_decorator = new PointDecorator();

        private IPointShape _point;
        private ReferenceShape _reference;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [IgnoreDataMember]
        public double X
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.X + _reference.X;
                }
                return double.NaN;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.X = value;
                }
            }
        }

        [IgnoreDataMember]
        public double Y
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.Y + _reference.Y;
                }
                return double.NaN;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.Y = value;
                }
            }
        }

        [IgnoreDataMember]
        public IBaseShape Template
        {
            get
            {
                if (_point != null && _reference != null)
                {
                    return _point.Template;
                }
                return null;
            }
            set
            {
                if (_point != null && _reference != null)
                {
                    _point.Template = value;
                }
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReferenceShape Reference
        {
            get => _reference;
            set => Update(ref _reference, value);
        }

        public ReferencePointShape()
        {
        }

        public ReferencePointShape(IPointShape point, ReferenceShape reference)
        {
            this.Point = point;
            this.Reference = reference;
            this.Owner = reference;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(this);
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (_point != null && _reference != null)
            {
                double offsetX = _reference.X;
                double offsetY = _reference.Y;
                _point.Draw(dc, renderer, dx + offsetX, dy + offsetY, scale, DrawMode.Shape, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new ReferencePointShape()
            {
                StyleId = this.StyleId,
                Owner = this.Owner,
                Point = this.Point,
                Reference = this.Reference,
            };
        }
    }

    [DataContract(IsReference = true)]
    public class ReferenceShape : ConnectableShape
    {
        internal static new IBounds s_bounds = new ReferenceBounds();
        internal static new IShapeDecorator s_decorator = new ReferenceDecorator();

        private string _title;
        private double _x;
        private double _y;
        private IBaseShape _template;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public ReferenceShape()
        {
        }

        public ReferenceShape(string title, double x, double y, IBaseShape template)
        {
            this.Title = title;
            this.X = x;
            this.Y = y;
            this.Template = template;
            this.Points = new ObservableCollection<IPointShape>();

            if (template is IConnectable connectable)
            {
                foreach (var point in connectable.Points)
                {
                    Points.Add(new ReferencePointShape(point, this));
                }
            }
        }

        public override void Invalidate()
        {
            if (_template != null)
            {
                _template.Invalidate();
            }

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (_template != null)
            {
                double offsetX = X;
                double offsetY = Y;
                _template.Draw(dc, renderer, dx + offsetX, dy + offsetY, scale, DrawMode.Shape, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new ReferenceShape()
            {
                StyleId = this.StyleId,
                Owner = this.Owner,
                Title = this.Title,
                X = this.X,
                Y = this.Y,
                Template = this.Template,
                Points = new ObservableCollection<IPointShape>()
            };

            if (shared != null)
            {
                foreach (var point in this.Points)
                {
                    if (point is ReferencePointShape referencePoint)
                    {
                        var referencePointCopy = (ReferencePointShape)shared[referencePoint];
                        referencePointCopy.Owner = copy;
                        referencePointCopy.Reference = copy;
                        copy.Points.Add(referencePointCopy);
                    }
                    else
                    {
                        copy.Points.Add((IPointShape)shared[point]);
                    }
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class TextShape : BoxShape
    {
        internal static new IBounds s_bounds = new TextBounds();
        internal static new IShapeDecorator s_decorator = new TextDecorator();

        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public TextShape()
            : base()
        {
        }

        public TextShape(IPointShape topLeft, IPointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public TextShape(Text text, IPointShape topLeft, IPointShape bottomRight)
            : base(topLeft, bottomRight)
        {
            this.Text = text;
        }

        public override void Invalidate()
        {
            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (StyleId != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawText(dc, this, StyleId, dx, dy, scale);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState?.IsSelected(TopLeft) ?? false)
                {
                    TopLeft.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }

                if (renderer.SelectionState?.IsSelected(BottomRight) ?? false)
                {
                    BottomRight.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, scale, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new TextShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (IPointShape)shared[this.TopLeft];
                copy.BottomRight = (IPointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }
}

namespace Draw2D.ViewModels.Shapes
{
    public static class BaseShapeExtensions
    {
        public static void GetBox(this IList<IPointShape> points, out double ax, out double ay, out double bx, out double by)
        {
            ax = double.MaxValue;
            ay = double.MaxValue;
            bx = double.MinValue;
            by = double.MinValue;

            foreach (var point in points)
            {
                ax = Math.Min(ax, point.X);
                ay = Math.Min(ay, point.Y);
                bx = Math.Max(bx, point.X);
                by = Math.Max(by, point.Y);
            }
        }

        public static void GetBox(this IBaseShape shape, out double ax, out double ay, out double bx, out double by)
        {
            var points = new List<IPointShape>();
            shape.GetPoints(points);
            GetBox(points, out ax, out ay, out bx, out by);
        }
    }

    public static class EllipseShapeExtensions
    {
        public static Rect2 ToRect2(this EllipseShape ellipse, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                ellipse.TopLeft.X, ellipse.TopLeft.Y,
                ellipse.BottomRight.X, ellipse.BottomRight.Y,
                dx, dy);
        }

        public static EllipseShape FromRect2(this Rect2 rect)
        {
            return new EllipseShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2())
            {
                Points = new ObservableCollection<IPointShape>()
            };
        }
    }

    public static class LineShapeExtensions
    {
        public static Line2 ToLine2(this LineShape line, double dx = 0.0, double dy = 0.0)
        {
            return Line2.FromPoints(
                line.StartPoint.X, line.StartPoint.Y,
                line.Point.X, line.Point.Y,
                dx, dy);
        }

        public static LineShape FromLine2(this Line2 line)
        {
            return new LineShape(line.A.FromPoint2(), line.B.FromPoint2())
            {
                Points = new ObservableCollection<IPointShape>()
            };
        }
    }

    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this IPointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static IPointShape FromPoint2(this Point2 point, IBaseShape template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }
    }

    public static class RectangleShapeExtensions
    {
        public static Rect2 ToRect2(this RectangleShape rectangle, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                rectangle.TopLeft.X, rectangle.TopLeft.Y,
                rectangle.BottomRight.X, rectangle.BottomRight.Y,
                dx, dy);
        }

        public static RectangleShape FromRect2(this Rect2 rect)
        {
            return new RectangleShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2())
            {
                Points = new ObservableCollection<IPointShape>()
            };
        }
    }
}
