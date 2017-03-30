using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;
using Draw2D.Models.Style;

namespace Draw2D.Models
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
                r.PopMatrix(dc);
            }
        }
    }
}
