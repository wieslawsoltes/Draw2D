// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shape;

namespace Core2D.Shapes
{
    public class PointShape : BaseShape, ICopyable
    {
        private double _x;
        private double _y;
        private BaseShape _template;

        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        public BaseShape Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public PointShape()
        {
        }

        public PointShape(double x, double y, BaseShape template)
        {
            this.X = x;
            this.Y = y;
            this.Template = template;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return this;
        }

        public override bool Invalidate(ShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            _template?.Invalidate(renderer, dx, dy);

            return this.IsDirty | result;
        }

        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            if (_template != null)
            {
                var state = base.BeginTransform(dc, renderer);

                _template.Draw(dc, renderer, X + dx, Y + dy, db, r);

                base.EndTransform(dc, renderer, state);
            }
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public object Copy(IDictionary<object, object> shared)
        {
            return new PointShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared),
                X = this.X,
                Y = this.Y,
                Template = this.Template
            };
        }
    }
}
