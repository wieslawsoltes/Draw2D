using System;
using System.Collections.Generic;
using Draw2D.Models.Renderers;

namespace Draw2D.Models.Shapes
{
    public class RectangleShape : ConnectableShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;

        public PointShape TopLeft
        {
            get { return _topLeft; }
            set
            {
                if (value != _topLeft)
                {
                    _topLeft = value;
                    Notify("TopLeft");
                }
            }
        }

        public PointShape BottomRight
        {
            get { return _bottomRight; }
            set
            {
                if (value != _bottomRight)
                {
                    _bottomRight = value;
                    Notify("BottomRight");
                }
            }
        }

        public RectangleShape()
            : base()
        {
        }

        public RectangleShape(PointShape topLeft, PointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            r.DrawRectangle(dc, this, Style, dx, dy);

            _topLeft.Draw(dc, r, dx, dy);
            _bottomRight.Draw(dc, r, dx, dy);

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<BaseShape> selected, double dx, double dy)
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

        public override void Select(ISet<BaseShape> selected)
        {
            base.Select(selected);
            TopLeft.Select(selected);
            BottomRight.Select(selected);
        }

        public override void Deselect(ISet<BaseShape> selected)
        {
            base.Deselect(selected);
            TopLeft.Deselect(selected);
            BottomRight.Deselect(selected);
        }
    }
}
