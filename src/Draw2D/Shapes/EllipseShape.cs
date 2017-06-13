﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Renderer;

namespace Draw2D.Core.Shapes
{
    public class EllipseShape : BoxShape, ICopyable
    {
        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = base.Invalidate(r, dx, dy);

            if (this.IsDirty || result == true)
            {
                r.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            var state = base.BeginTransform(dc, r);

            if (Style != null)
            {
                r.DrawEllipse(dc, this, Style, dx, dy);
            }

            if (r.Selected.Contains(TopLeft))
            {
                TopLeft.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(BottomRight))
            {
                BottomRight.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r, state);
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new EllipseShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }
}
