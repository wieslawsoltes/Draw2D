// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class ScribbleShape : ShapeObject
    {
        private PointShape _start;
        private ObservableCollection<PointShape> _points;

        public PointShape Start
        {
            get => _start;
            set => Update(ref _start, value);
        }

        public ObservableCollection<PointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ScribbleShape()
            : base()
        {
        }

        public ScribbleShape(PointShape start, ObservableCollection<PointShape> points)
            : base()
        {
            this.Start = start;
            this.Points = points;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return Start;
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (_points.Count >= 1 && Style != null)
            {
                r.DrawPolyLine(dc, _start, _points, Style, dx, dy);
            }

            if (r.Selected.Contains(_start))
            {
                _start.Draw(dc, r, dx, dy);
            }

            foreach (var point in _points)
            {
                if (r.Selected.Contains(point))
                {
                    point.Draw(dc, r, dx, dy);
                }
            }

            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_start))
            {
                _start.Move(selected, dx, dy);
            }

            foreach (var point in Points)
            {
                if (!selected.Contains(point))
                {
                    point.Move(selected, dx, dy);
                }
            }
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);

            Start.Select(selected);

            foreach (var point in Points)
            {
                point.Select(selected);
            }
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);

            Start.Deselect(selected);

            foreach (var point in Points)
            {
                point.Deselect(selected);
            }
        }
    }
}
