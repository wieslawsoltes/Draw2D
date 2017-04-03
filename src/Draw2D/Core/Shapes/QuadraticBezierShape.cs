// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class QuadraticBezierShape : ConnectableShape
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
    }
}
