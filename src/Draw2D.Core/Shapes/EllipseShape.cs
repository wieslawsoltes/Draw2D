// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class EllipseShape : ConnectableShape, ICopyable<EllipseShape>
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;

        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(PointShape topLeft, PointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return TopLeft;
            yield return BottomRight;
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = base.Invalidate(r, dx, dy);

            result |= _topLeft?.Invalidate(r, dx, dy) ?? false;
            result |= _bottomRight?.Invalidate(r, dx, dy) ?? false;

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
            base.BeginTransform(dc, r);

            if (Style != null)
            {
                r.DrawEllipse(dc, this, Style, dx, dy);
            }

            if (r.Selected.Contains(_topLeft))
            {
                _topLeft.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(_bottomRight))
            {
                _bottomRight.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_topLeft))
            {
                _topLeft.Move(selected, dx, dy);
            }

            if (!selected.Contains(_bottomRight))
            {
                _bottomRight.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            TopLeft.Select(selected);
            BottomRight.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            TopLeft.Deselect(selected);
            BottomRight.Deselect(selected);
        }

        private bool CanConnect(PointShape point)
        {
            return TopLeft != point
                && BottomRight != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (TopLeft == target)
                {
                    Debug.WriteLine($"{nameof(EllipseShape)}: Connected to {nameof(TopLeft)}");
                    this.TopLeft = point;
                    return true;
                }
                else if (BottomRight == target)
                {
                    Debug.WriteLine($"{nameof(EllipseShape)}: Connected to {nameof(BottomRight)}");
                    this.BottomRight = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (TopLeft == point)
            {
                Debug.WriteLine($"{nameof(EllipseShape)}: Disconnected from {nameof(TopLeft)}");
                result = point.Copy();
                this.TopLeft = result;
                return true;
            }
            else if (BottomRight == point)
            {
                Debug.WriteLine($"{nameof(EllipseShape)}: Disconnected from {nameof(BottomRight)}");
                result = point.Copy();
                this.BottomRight = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.TopLeft != null)
            {
                Debug.WriteLine($"{nameof(EllipseShape)}: Disconnected from {nameof(TopLeft)}");
                this.TopLeft = this.TopLeft.Copy();
                result = true;
            }

            if (this.BottomRight != null)
            {
                Debug.WriteLine($"{nameof(EllipseShape)}: Disconnected from {nameof(BottomRight)}");
                this.BottomRight = this.BottomRight.Copy();
                result = true;
            }

            return result;
        }

        public EllipseShape Copy()
        {
            return new EllipseShape()
            {
                Style = this.Style,
                Transform = this.Transform?.Copy()
            };
        }
    }
}
