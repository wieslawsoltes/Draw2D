// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Spatial;

namespace Draw2D.ViewModels.Shapes
{
    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this PointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static PointShape FromPoint2(this Point2 point, BaseShape template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }
    }

    public class PointShape : BaseShape, ICopyable
    {
        private MatrixObject _templateTransform = MatrixObject.Identity;
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

        public override bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            bool result = base.Invalidate(renderer, dx, dy);

            _template?.Invalidate(renderer, dx, dy);

            return this.IsDirty | result;
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (_template != null)
            {
                var pointState = base.BeginTransform(dc, renderer);

                double offsetX = X + dx;
                double offsetY = Y + dy;

                if (_templateTransform.OffsetX != offsetX
                    || _templateTransform.OffsetY != offsetY)
                {
                    _templateTransform.OffsetX = offsetX;
                    _templateTransform.OffsetY = offsetY;
                    _templateTransform.Invalidate(renderer);
                }

                var templateState = renderer.PushMatrix(dc, _templateTransform);

                _template.Draw(dc, renderer, 0, 0, db, r);

                renderer.PopMatrix(dc, templateState);

                base.EndTransform(dc, renderer, pointState);
            }
        }

        public override void Move(ISelection selection, double dx, double dy)
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
