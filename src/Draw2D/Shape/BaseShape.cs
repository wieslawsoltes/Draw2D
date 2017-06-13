// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderer;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Shape
{
    public abstract class BaseShape : ObservableObject, IDrawable, ISelectable
    {
        private ShapeStyle _style;
        private MatrixObject _transform;

        public ShapeStyle Style
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

        public virtual bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = false;
            result |= _style?.Invalidate(r) ?? false;
            result |= _transform?.Invalidate(r) ?? false;
            return result;
        }

        public abstract void Draw(object dc, ShapeRenderer r, double dx, double dy);

        public virtual object BeginTransform(object dc, ShapeRenderer r)
        {
            if (Transform != null)
            {
                return r.PushMatrix(dc, Transform);
            }
            return null;
        }

        public virtual void EndTransform(object dc, ShapeRenderer r, object state)
        {
            if (Transform != null)
            {
                r.PopMatrix(dc, state);
            }
        }

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
    }
}
