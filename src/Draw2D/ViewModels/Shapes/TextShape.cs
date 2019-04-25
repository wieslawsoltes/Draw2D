// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels.Shapes
{
    public class TextShape : BoxShape, ICopyable
    {
        private TextObject _text;

        public TextObject Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public TextShape()
            : base()
        {
        }

        public TextShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public TextShape(TextObject text, PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
            this.Text = text;
        }

        public override bool Invalidate(IShapeRenderer renderer, double dx, double dy, double zx, double zy)
        {
            bool result = base.Invalidate(renderer, dx, dy, zx, zy);

            if (_text?.IsDirty ?? false)
            {
                _text.IsDirty = false;
                result |= true;
            }

            if (this.IsDirty || result == true)
            {
                renderer.InvalidateCache(this, Style, dx, dy, zx, zy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double zx, double zy, DrawMode mode, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawText(dc, this, Style, dx, dy, zx, zy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.Selection.Selected.Contains(TopLeft))
                {
                    TopLeft.Draw(dc, renderer, dx, dy, zx, zy, mode, db, r);
                }
    
                if (renderer.Selection.Selected.Contains(BottomRight))
                {
                    BottomRight.Draw(dc, renderer, dx, dy, zx, zy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, zx, zy, mode, db, r);
            base.EndTransform(dc, renderer, state);
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new TextShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared),
                Text = (TextObject)this.Text?.Copy(shared)
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
