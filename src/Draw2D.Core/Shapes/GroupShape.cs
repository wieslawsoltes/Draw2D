// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class GroupShape : ConnectableShape, ICopyable<GroupShape>
    {
        private ObservableCollection<ShapeObject> _shapes;

        public ObservableCollection<ShapeObject> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public GroupShape()
            : base()
        {
            _shapes = new ObservableCollection<ShapeObject>();
        }

        public GroupShape(ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public GroupShape(string name)
            : this()
        {
            this.Name = name;
        }

        public GroupShape(string name, ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            foreach (var point in Points)
            {
                yield return point;
            }

            foreach (var shape in Shapes)
            {
                foreach (var point in shape.GetPoints())
                {
                    yield return point;
                }
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

            foreach (var point in Points)
            {
                point.Invalidate(r);
            }
            
            foreach (var shape in Shapes)
            {
                shape.Invalidate(r);
            }
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            foreach (var shape in Shapes)
            {
                shape.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            var points = GetPoints().Distinct();

            foreach (var point in points)
            {
                if (!selected.Contains(point))
                {
                    point.Move(selected, dx, dy);
                }
            }

            base.Move(selected, dx, dy);
        }

        public GroupShape Copy()
        {
            return new GroupShape()
            {
                Style = this.Style,
                Transform = this.Transform?.Copy()
            };
        }
    }
}
