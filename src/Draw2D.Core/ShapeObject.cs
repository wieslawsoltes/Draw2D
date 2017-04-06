// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core
{
    public abstract class ShapeObject : IdObject
    {
        private DrawStyle _style;
        private MatrixObject _transform;

        public DrawStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        public MatrixObject Transform
        {
            get => _transform;
            set => Update(ref _transform, value);
        }

        public abstract IEnumerable<PointShape> GetPoints();

        public abstract void Draw(object dc, ShapeRenderer r, double dx, double dy);

        public abstract void Move(ISet<ShapeObject> selected, double dx, double dy);

        public virtual void Select(ISet<ShapeObject> selected)
        {
            if (!selected.Contains(this))
            {
                selected.Add(this);
            }
        }

        public virtual void Deselect(ISet<ShapeObject> selected)
        {
            if (selected.Contains(this))
            {
                selected.Remove(this);
            }
        }

        public virtual void BeginTransform(object dc, ShapeRenderer r)
        {
            if (Transform != null)
            {
                r.PushMatrix(dc, Transform);
            }
        }

        public virtual void EndTransform(object dc, ShapeRenderer r)
        {
            if (Transform != null)
            {
                r.PopMatrix(dc, null);
            }
        }
    }
}
