using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class CubicBezierShape : BaseShape
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

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (StyleId != null)
            {
                renderer.DrawCubicBezier(dc, this, StyleId, dx, dy, scale);
            }
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
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
                    this.Point2 = point;
                    return true;
                }
                else if (Point3 == target)
                {
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
                result = (IPointShape)(point.Copy(null));
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
                result = (IPointShape)(point.Copy(null));
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
                result = (IPointShape)(point.Copy(null));
                this.Point2 = result;
                return true;
            }
            else if (Point3 == point)
            {
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
                this.StartPoint = (IPointShape)(this.StartPoint.Copy(null));
                result = true;
            }

            if (this.Point1 != null)
            {
                this.Point1 = (IPointShape)(this.Point1.Copy(null));
                result = true;
            }

            if (this.Point2 != null)
            {
                this.Point2 = (IPointShape)(this.Point2.Copy(null));
                result = true;
            }

            if (this.Point3 != null)
            {
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
                Effects = (IPaintEffects)this.Effects?.Copy(shared),
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (IPointShape)shared[this.StartPoint];
                copy.Point1 = (IPointShape)shared[this.Point1];
                copy.Point2 = (IPointShape)shared[this.Point2];
                copy.Point3 = (IPointShape)shared[this.Point3];

                copy.StartPoint.Owner = copy;
                copy.Point1.Owner = copy;
                copy.Point2.Owner = copy;
                copy.Point3.Owner = copy;

                foreach (var point in this.Points)
                {
                    var pointCopy = (IPointShape)shared[point];
                    pointCopy.Owner = copy;
                    copy.Points.Add(pointCopy);
                }

                shared[this] = copy;
            }

            return copy;
        }
    }
}
