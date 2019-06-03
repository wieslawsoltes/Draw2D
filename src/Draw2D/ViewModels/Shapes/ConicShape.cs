// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;

namespace Draw2D.ViewModels.Shapes
{
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
                    Log.WriteLine($"{nameof(ConicShape)}: Connected to {nameof(StartPoint)}");
#endif
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
#if DEBUG_CONNECTORS
                    Log.WriteLine($"{nameof(ConicShape)}: Connected to {nameof(Point1)}");
#endif
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
#if DEBUG_CONNECTORS
                    Log.WriteLine($"{nameof(ConicShape)}: Connected to {nameof(Point2)}");
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
                Log.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point1)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point2)}");
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
                Log.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(StartPoint)}");
#endif
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point1 != null)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point1)}");
#endif
                this.Point1 = (IPointShape)(this.Point1.Copy(null));
                result = true;
            }

            if (this.Point2 != null)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(ConicShape)}: Disconnected from {nameof(Point2)}");
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
}
