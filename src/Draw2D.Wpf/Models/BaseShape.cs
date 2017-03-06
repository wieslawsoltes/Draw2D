using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;
using Draw2D.Models.Style;

namespace Draw2D.Models
{
    public abstract class BaseShape : BaseObject
    {
        private DrawStyle _style;
        private MatrixObject _transform;

        public DrawStyle Style
        {
            get { return _style; }
            set
            {
                if (value != _style)
                {
                    _style = value;
                    Notify("Style");
                }
            }
        }

        public MatrixObject Transform
        {
            get { return _transform; }
            set
            {
                if (value != _transform)
                {
                    _transform = value;
                    Notify("Transform");
                }
            }
        }

        public abstract void Draw(object dc, ShapeRenderer r, double dx, double dy);

        public abstract void Move(ISet<BaseShape> selected, double dx, double dy);

        public virtual void Select(ISet<BaseShape> selected)
        {
            if (!selected.Contains(this))
            {
                selected.Add(this);
            }
        }

        public virtual void Deselect(ISet<BaseShape> selected)
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
