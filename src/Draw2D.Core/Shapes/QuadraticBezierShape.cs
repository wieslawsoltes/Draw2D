// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class QuadraticBezierShape : ConnectableShape, ICopyable<QuadraticBezierShape>
    {
        private PointShape _startPoint;
        private PointShape _point1;
        private PointShape _point2;

        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        public PointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        public PointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        public QuadraticBezierShape()
            : base()
        {
        }

        public QuadraticBezierShape(PointShape startPoint, PointShape point1, PointShape point2)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return StartPoint;
            yield return Point1;
            yield return Point2;
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override void Invalidate(ShapeRenderer r)
        {
            base.Invalidate(r);

            if (this.IsDirty)
            {
                r.InvalidateCache(this);
                this.IsDirty = false;
            }

            _startPoint?.Invalidate(r);
            _point1?.Invalidate(r);
            _point2?.Invalidate(r);
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (Style != null)
            {
                r.DrawQuadraticBezier(dc, this, Style, dx, dy);
            }

            if (r.Selected.Contains(_startPoint))
            {
                _startPoint.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(_point1))
            {
                _point1.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(_point2))
            {
                _point2.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_startPoint))
            {
                _startPoint.Move(selected, dx, dy);
            }

            if (!selected.Contains(_point1))
            {
                _point1.Move(selected, dx, dy);
            }

            if (!selected.Contains(_point2))
            {
                _point2.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            StartPoint.Select(selected);
            Point1.Select(selected);
            Point2.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            StartPoint.Deselect(selected);
            Point1.Deselect(selected);
            Point2.Deselect(selected);
        }

        private bool CanConnect(PointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(StartPoint)}");
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point1)}");
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point2)}");
                    this.Point2 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
                result = point.Copy();
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
                result = point.Copy();
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
                result = point.Copy();
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
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
                this.StartPoint = this.StartPoint.Copy();
                result = true;
            }

            if (this.Point1 != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
                this.Point1 = this.Point1.Copy();
                result = true;
            }

            if (this.Point2 != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
                this.Point2 = this.Point2.Copy();
                result = true;
            }

            return result;
        }

        public QuadraticBezierShape Copy()
        {
            return new QuadraticBezierShape()
            {
                Style = this.Style,
                Transform = this.Transform?.Copy(),
            };
        }
    }
}
