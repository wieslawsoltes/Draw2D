// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class LineShape : ConnectableShape, ICopyable<LineShape>
    {
        private PointShape _startPoint;
        private PointShape _point;

        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        public PointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        public LineShape()
            : base()
        {
        }

        public LineShape(PointShape startPoint, PointShape point)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point = point;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return StartPoint;
            yield return Point;
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (Style != null)
            {
                r.DrawLine(dc, this, Style, dx, dy);
            }

            if (r.Selected.Contains(_startPoint))
            {
                _startPoint.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(_point))
            {
                _point.Draw(dc, r, dx, dy);
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

            if (!selected.Contains(_point))
            {
                _point.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            StartPoint.Select(selected);
            Point.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            StartPoint.Deselect(selected);
            Point.Deselect(selected);
        }

        public override bool Connect(PointShape source, PointShape target)
        {
            if (base.Connect(source, target))
            {
                return true;
            }
            else if (StartPoint != source && Point != source)
            { 
                if (StartPoint == target)
                {
                    Debug.WriteLine($"LineShape Connected to StartPoint");
                    this.StartPoint = source;
                    return true;
                }
                else if (Point == target)
                {
                    Debug.WriteLine($"LineShape Connected to Point");
                    this.Point = source;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape source, out PointShape copy)
        {
            if (base.Disconnect(source, out copy))
            {
                return true;
            }
            else if (StartPoint == source)
            {
                Debug.WriteLine($"LineShape Disconnected from StartPoint");
                copy = source.Copy();
                this.StartPoint = copy;
                return true;
            }
            else if (Point == source)
            {
                Debug.WriteLine($"LineShape Disconnected from Point");
                copy = source.Copy();
                this.Point = copy;
                return true;
            }
            copy = null;
            return false;
        }

        public LineShape Copy()
        {
            return new LineShape()
            {
                Style = this.Style,
                Transform = this.Transform?.Copy()
            };
        }
    }
}
