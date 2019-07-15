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
    public class CircleShape : BaseShape
    {
        internal static new IBounds s_bounds = new CircleBounds();
        internal static new IShapeDecorator s_decorator = new CircleDecorator();

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

        public CircleShape()
        {
        }

        public CircleShape(IPointShape startPoint, IPointShape point)
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

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (StyleId != null)
            {
                renderer.DrawCircle(dc, this, StyleId, dx, dy, scale);
            }
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
                    this.StartPoint = point;
                    return true;
                }
                else if (Point == target)
                {
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
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point == point)
            {
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
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point != null)
            {
                this.Point = (IPointShape)(this.Point.Copy(null));
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new CircleShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (IPointShape)shared[this.StartPoint];
                copy.Point = (IPointShape)shared[this.Point];

                copy.StartPoint.Owner = copy;
                copy.Point.Owner = copy;

                foreach (var point in this.Points)
                {
                    var pointCopy = (IPointShape)shared[point];
                    pointCopy.Owner = copy;
                    copy.Points.Add(pointCopy);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }
}
