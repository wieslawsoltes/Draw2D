// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public abstract class ConnectableShape : ShapeObject
    {
        private ObservableCollection<PointShape> _points;

        public ObservableCollection<PointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ConnectableShape()
        {
            _points = new ObservableCollection<PointShape>();
        }

        public ConnectableShape(ObservableCollection<PointShape> points)
        {
            this.Points = points;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            foreach (var point in Points)
            {
                if (r.Selected.Contains(point))
                {
                    point.Draw(dc, r, dx, dy);
                }
            }
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
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

            foreach (var point in Points)
            {
                point.Select(selected);
            }
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);

            foreach (var point in Points)
            {
                point.Deselect(selected);
            }
        }

        public virtual bool Connect(PointShape point, PointShape target)
        {
            if (_points.Contains(point) == true)
            {
                return false;
            }

            int index = _points.IndexOf(target);
            if (index >= 0)
            {
                Debug.WriteLine($"ConnectableShape Connected to Points");
                _points[index] = point;
                return true;
            }

            return false;
        }

        public virtual bool Disconnect(PointShape point, out PointShape result)
        {
            result = null;
            return false;
        }

        public virtual bool Disconnect()
        {
            bool result = false;

            for (int i = 0; i < _points.Count; i++)
            {
                Debug.WriteLine($"ConnectableShape Disconnected Point {i}");
                _points[i] = _points[i].Copy();
                result = true;
            }

            return result;
        }
    }
}
